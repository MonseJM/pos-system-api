using System.ComponentModel.DataAnnotations;

namespace Mi_punto_de_venta.DTOs


{
    public class CreateUserDto
    {
        [Required]
        public string FullName { get; set; }

        [Required,EmailAddress]
        public string Email{ get; set; }

        [Required]
        public string Password { get; set; }
     

    }
}
