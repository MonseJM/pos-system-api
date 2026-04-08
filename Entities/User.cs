using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mi_punto_de_venta.Entities
{
    [Table("Users")]
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string FullName { get; set; }

        [Required]
        [MaxLength(150)]
        public string Email {  get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required]
        public string Role {  get; set; }
       
        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        // 🔥 Relación
        public Customer Customer { get; set; }


    }
}
