using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mi_punto_de_venta.Entities
{
    [Table("Sales")]
    public class Sale
    
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }

        public decimal Subtotal { get; set; }

        public decimal Tax { get; set; }

        public decimal Total { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}

