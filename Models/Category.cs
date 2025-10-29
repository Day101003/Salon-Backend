using System.ComponentModel.DataAnnotations;

namespace Salon_Info.Models
{
    public class Category
    {
        [Key]
        public int IdCategoria { get; set; } 
        [Required, MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        public string Descripcion { get; set; } = string.Empty; 

        public int Estado { get; set; }
    }
}

