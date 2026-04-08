using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mi_punto_de_venta.Entities
{
    [Table("Products")]
    public class Product
    {
        [Key]
        public int Id  {get; set;}

        [Required,MaxLength(150)]
        public string Name {get; set;}

        public string Description {get; set;}

        public decimal Price {get; set;}

        public int Stock { get; set;}

        public string Size {get; set;}

        public string Color { get; set;}

        public string ImageUrl { get; set; }

        public bool IsActive { get; set;}
        public DateTime CreatedAt { get; set;}

    }
}
