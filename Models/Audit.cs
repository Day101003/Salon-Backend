using System.ComponentModel.DataAnnotations;

namespace Salon_Info.Models
{
    public class Audit
    {
        [Key]
        public int IdAuditoria { get; set; } 

        [Required, MaxLength(255)]
        public string Proceso { get; set; } = string.Empty; // varchar(255)

        public DateTime Fecha { get; set; } 

        [Required, MaxLength(255)]
        public string QuienRealizo { get; set; } = string.Empty; // varchar(255)
    }
}