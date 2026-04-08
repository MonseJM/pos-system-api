using System.ComponentModel.DataAnnotations;

namespace Mi_punto_de_venta.Entities
{
    public class SalesAnalytics
    {
        [Key]
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public decimal Total { get; set; }

        public decimal Tax { get; set; }

        public int ProductsCount { get; set; }

        public int UserId { get; set; }
    }
}
