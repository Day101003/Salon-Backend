using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Salon_Info.Models
{
    public class Product
    {
        [Key]
        public int IdProducto { get; set; } 

        [Required, MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Column(TypeName = "decimal(10,2)")]
        public decimal Cantidad { get; set; } 

        [Column(TypeName = "decimal(10,2)")]
        public decimal Costo { get; set; } 

        public int Estado { get; set; } 
    }
}
