using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mi_punto_de_venta.Entities
{
    [Table("SaleDetails")]
    public class SaleDetail
    {
        [Key]
        public int Id { get; set; }

        public int SaleId { get; set; }

        public int ProductId { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public decimal Total { get; set; }
    }
}
