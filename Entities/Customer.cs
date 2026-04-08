using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Mi_punto_de_venta.Entities
{


    [Table("Customers")]
    public class Customer
    {
        [Key]
        public int Id { get; set; }

      //Foreign Key
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        public string? Name { get; set; }

        public string? Rfc { get; set; }

        public string? FiscalRegime { get; set; }

        public string? CfdiUse { get; set; }

        public string? PostalCode { get; set; }

        public string? Email { get; set; }

        public bool IsActive { get; set; }
    }
}
