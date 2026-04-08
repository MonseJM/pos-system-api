using iText.IO.Image;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Mi_punto_de_venta.Data;
using Mi_punto_de_venta.DTOs;
using Mi_punto_de_venta.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mi_punto_de_venta.Controllers
{
    [ApiController]
    [Route("api/invoices")]
    public class InvoicesController : Controller
    {
        private readonly PosDbContext _context;
        private readonly CfdiService _cfdi;
        private readonly IWebHostEnvironment _env;
        private readonly AnalyticsDbContext _analytics;
        public InvoicesController(PosDbContext context, CfdiService cfdi, IWebHostEnvironment env, AnalyticsDbContext analytics)
        {
            _context = context;
            _cfdi = cfdi;
            _env = env;
            _analytics = analytics;
        }

        [HttpPost("from-sale/{saleId}")]
        public IActionResult CreateFromSale(int saleId, CreateInvoiceDto dto)
        {
            var sale = _context.Sales.FirstOrDefault(s => s.Id == saleId);

            if (sale == null)
                return NotFound("Venta no encontrada");


            var customer = _context.Customers
             .FirstOrDefault(c => c.UserId == sale.UserId && c.IsActive);

            if (customer == null)
                return BadRequest("Cliente fiscal inválido");

            // 1️⃣ Crear invoice
            var invoice = new Invoice
            {
                SaleId = sale.Id,
                CustomerId = customer.Id,
                Subtotal = sale.Subtotal,
                Tax = sale.Tax,
                Total = sale.Total,
                Status = "Pendiente",
                CreatedAt = DateTime.Now
            };

            // 2️⃣ GUARDAR PARA GENERAR ID
            _context.Invoices.Add(invoice);
            _context.SaveChanges();

          

            // 3️⃣ generar XML
            var details = _context.SalesDetails
                .Where(d => d.SaleId == sale.Id)
                .ToList();

            var xml = _cfdi.GenerateXml(sale, customer, details);

            // 4️⃣ guardar archivo con ID REAL
            var folder = Path.Combine(_env.ContentRootPath, "invoices");

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            var fileName = $"{invoice.Id}.xml";
            var fullPath = Path.Combine(folder, fileName);

            System.IO.File.WriteAllText(fullPath, xml);

            // 5️⃣ guardar ruta
            invoice.XmlPath = $"invoices/{fileName}";
            _context.SaveChanges();



            var analytics = new SalesAnalytics
            {
                Date = DateTime.UtcNow,
                Total = sale.Total,
                Tax = sale.Tax,
                ProductsCount = details.Sum(d => d.Quantity),
                UserId = sale.UserId
            };

            _analytics.SalesAnalytics.Add(analytics);
            _analytics.SaveChanges();

            return Ok(invoice);


        }

        [HttpGet("{invoiceId}/xml")]
        public IActionResult DescargarXml(int invoiceId)
        {
            var invoice = _context.Invoices.FirstOrDefault(i => i.Id == invoiceId);

            if (invoice == null || string.IsNullOrEmpty(invoice.XmlPath))
                return NotFound("XML no encontrado");

            var fullPath = Path.Combine(_env.ContentRootPath, invoice.XmlPath);

            if (!System.IO.File.Exists(fullPath))
                return NotFound("Archivo no existe");

            var bytes = System.IO.File.ReadAllBytes(fullPath);

            return File(bytes, "application/xml", $"factura_{invoiceId}.xml");
        }

      
        [HttpGet("{invoiceId}/pdf")]
        public IActionResult DescargarPdf(int invoiceId)
        {
            var invoice = _context.Invoices.FirstOrDefault(i => i.Id == invoiceId);

            if (invoice == null)
                return NotFound();

            var customer = _context.Customers.FirstOrDefault(c => c.Id == invoice.CustomerId);

            var details = _context.SalesDetails
                .Where(d => d.SaleId == invoice.SaleId)
                .ToList();

            byte[] pdfBytes;

            using (var stream = new MemoryStream())
            {
                using (var writer = new PdfWriter(stream))
                {
                    using (var pdf = new PdfDocument(writer))
                    {
                        using (var doc = new Document(pdf))
                        {
                            // 🖼️ LOGO
                            var logoPath = Path.Combine(_env.WebRootPath, "imagenes/ae.png");

                            if (System.IO.File.Exists(logoPath))
                            {
                                var imageData = ImageDataFactory.Create(logoPath);
                                var logo = new Image(imageData)
                                    .ScaleToFit(120, 60);

                                doc.Add(logo);
                            }

                            // 🧾 ENCABEZADO
                            doc.Add(new Paragraph("FACTURA")
                                .SetFontSize(20)
                                
                                .SetTextAlignment(TextAlignment.RIGHT));

                            doc.Add(new Paragraph($"Folio: {invoice.Id}")
                                .SetTextAlignment(TextAlignment.RIGHT));

                            doc.Add(new Paragraph($"Fecha: {invoice.CreatedAt}")
                                .SetTextAlignment(TextAlignment.RIGHT));

                            doc.Add(new Paragraph("\n"));

                            // 👤 CLIENTE
                            new Paragraph(new Text("DATOS DEL CLIENTE"));

                            doc.Add(new Paragraph($"Nombre: {customer?.Name ?? "N/A"}"));
                            doc.Add(new Paragraph($"RFC: {customer?.Rfc ?? "N/A"}"));
                            doc.Add(new Paragraph($"Correo: {customer?.Email ?? "N/A"}"));

                            doc.Add(new Paragraph("\n"));

                            // 📦 TABLA
                            var table = new Table(UnitValue.CreatePercentArray(new float[] { 4, 2, 2, 2 }))
                                .UseAllAvailableWidth();

                            table.AddHeaderCell("Producto");
                            table.AddHeaderCell("Cantidad");
                            table.AddHeaderCell("Precio");
                            table.AddHeaderCell("Total");

                            foreach (var item in details)
                            {
                                table.AddCell($"Producto ID: {item.ProductId}");
                                table.AddCell(item.Quantity.ToString());
                                table.AddCell(item.Price.ToString("C"));
                                table.AddCell((item.Quantity * item.Price).ToString("C"));
                            }

                            doc.Add(table);

                            // 💰 TOTALES
                            doc.Add(new Paragraph("\n"));

                            doc.Add(new Paragraph($"Subtotal: {invoice.Subtotal:C}")
                                .SetTextAlignment(TextAlignment.RIGHT));

                            doc.Add(new Paragraph($"IVA: {invoice.Tax:C}")
                                .SetTextAlignment(TextAlignment.RIGHT));

                            doc.Add(new Paragraph($"TOTAL: {invoice.Total:C}")
                                
                                .SetFontSize(14)
                                .SetTextAlignment(TextAlignment.RIGHT));
                        }
                    }
                }

                pdfBytes = stream.ToArray();
            }

            return File(pdfBytes, "application/pdf", $"factura_{invoiceId}.pdf");
        }

        [HttpGet]
        public async Task<ActionResult> GetInvoices()
        {
            var invoices = await _context.Invoices
                .Join(_context.Customers,
                    i => i.CustomerId,
                    c => c.Id,
                    (i, c) => new
                    {
                        i.Id,
                        i.Total,
                        i.Status,
                        i.Subtotal,
                        i.Tax,
                        i.CreatedAt,
                        CustomerName = c.Name // para ver el nombre del cliente 
                    })
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();

            return Ok(invoices);
        }


    }
    }
