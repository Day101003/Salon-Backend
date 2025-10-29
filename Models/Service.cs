using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Salon_Info.Models
{
    public class Service
    {
        [Key]
        public int IdServicio { get; set; } 
        [Required, MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        public string Descripcion { get; set; } = string.Empty;

        [Column(TypeName = "decimal(10,2)")]
        public decimal Costo { get; set; } 

        [MaxLength(100)]
        public string DuracionEstimada { get; set; } = string.Empty; 

        public int Estado { get; set; } 

        [ForeignKey("Categorie")]
        public int IdCategoria { get; set; } 
    }
}
