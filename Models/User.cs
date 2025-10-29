using System.ComponentModel.DataAnnotations;
namespace Salon_Info.Models
{
    public class User
    {
        [Key]
        public int IdUsuario { get; set; } 

        [Required, MaxLength(100)]
        public string Nombre { get; set; } = string.Empty; 

        [MaxLength(100)]
        public string Telefono { get; set; } = string.Empty; 

        [Required, MaxLength(100)]
        public string Correo { get; set; } = string.Empty;

        public DateTime FechaRegistro { get; set; } 

        [Required, MaxLength(255)]
        public string Contrasena { get; set; } = string.Empty; 

        [MaxLength(255)]
        public string RutaImg { get; set; } = string.Empty; 

        public int Tipo { get; set; } 

        public string Preferencias { get; set; } = string.Empty; 
    }
}