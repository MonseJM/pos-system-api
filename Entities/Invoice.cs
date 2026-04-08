using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mi_punto_de_venta.Entities
{

    [Table("Invoices")]
    public class Invoice
    {
        [Key]
        public int Id { get; set; }

        public int SaleId { get; set; }

        public int CustomerId { get; set; }

        public decimal Subtotal { get; set; }

        public decimal Tax { get; set; }

        public decimal Total { get; set; }

        public string? Uuid { get; set; }

        public string? XmlPath { get; set; }

        public string? Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
