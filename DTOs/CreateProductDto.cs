using System.ComponentModel.DataAnnotations;

namespace Mi_punto_de_venta.DTOs
{
    public class CreateProductDto
    {
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public int Stock { get; set; }

        public string Size { get; set; }

        public string Color { get; set; }

        public string ImageUrl { get; set; }
    }
}
