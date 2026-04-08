using Mi_punto_de_venta.Data;
using Mi_punto_de_venta.DTOs;
using Mi_punto_de_venta.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Mi_punto_de_venta.Controllers
{
    [ApiController]
    [Route ("api/sales")]
    public class SaleController : Controller
    {
        private readonly PosDbContext _context;
        
        public SaleController (PosDbContext context)
        {
            _context = context;
        }


        [HttpPost]
        public IActionResult CreateSale(CreateSaleDto dto)
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);

            int userId;

            if (claim == null)
                userId = 1; // modo prueba
            else
                userId = int.Parse(claim.Value);
            

            decimal subtotal = 0;

            var saleDetails = new List<SaleDetail>();

            foreach (var item in dto.Items)
            {
                var product = _context.Products
                    .FirstOrDefault(p => p.Id == item.ProductId && p.IsActive);

                if (product == null)
                    return BadRequest($"Producto {item.ProductId} no existe");

                if (product.Stock < item.Quantity)
                    return BadRequest($"Stock insuficiente en {product.Name}");

                decimal lineTotal = product.Price * item.Quantity;

                subtotal += lineTotal;

                saleDetails.Add(new SaleDetail
                {
                    ProductId = product.Id,
                    Quantity = item.Quantity,
                    Price = product.Price,
                    Total = lineTotal
                });

                product.Stock -= item.Quantity;
            }

            decimal tax = subtotal * 0.16m;
            decimal total = subtotal + tax;

            var sale = new Sale
            {
                UserId = userId,
                Subtotal = subtotal,
                Tax = tax,
                Total = total,
                CreatedAt = DateTime.Now
            };

            _context.Sales.Add(sale);
            _context.SaveChanges();

            foreach (var detail in saleDetails)
            {
                detail.SaleId = sale.Id;
            }

            _context.SalesDetails.AddRange(saleDetails);
            _context.SaveChanges();

            return Ok(sale);
        }
    }
}
