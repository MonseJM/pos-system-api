using Mi_punto_de_venta.Entities;
using System.Text;
using System.Xml.Linq;

public class CfdiService
{
    public string GenerateXml(Sale sale, Customer customer, List<SaleDetail> details)
    {
        XNamespace cfdi = "http://www.sat.gob.mx/cfd/4";

        var xml = new XDocument(
            new XElement(cfdi + "Comprobante",

                new XAttribute("Version", "4.0"),
                new XAttribute("SubTotal", sale.Subtotal),
                new XAttribute("Total", sale.Total),
                new XAttribute("Moneda", "MXN"),
                new XAttribute("TipoDeComprobante", "I"),

                new XElement(cfdi + "Emisor",
                    new XAttribute("Rfc", "AAA010101AAA"),
                    new XAttribute("Nombre", "Borcelle SA de CV"),
                    new XAttribute("RegimenFiscal", "601")
                ),

                new XElement(cfdi + "Receptor",
                    new XAttribute("Rfc", customer.Rfc),
                    new XAttribute("Nombre", customer.Name),
                    new XAttribute("DomicilioFiscalReceptor", customer.PostalCode),
                    new XAttribute("RegimenFiscalReceptor", customer.FiscalRegime),
                    new XAttribute("UsoCFDI", customer.CfdiUse)
                ),

                new XElement(cfdi + "Conceptos",
                    details.Select(d =>
                        new XElement(cfdi + "Concepto",
                            new XAttribute("Cantidad", d.Quantity),
                            new XAttribute("ValorUnitario", d.Price),
                            new XAttribute("Importe", d.Total),
                            new XAttribute("Descripcion", "Producto Borcelle")
                        )
                    )
                )
            )
        );

        return xml.ToString();
    }
}