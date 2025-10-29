using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Salon_Info.Models
{
    public class Cita
    {
        [Key]
        public int IdCita { get; set; } 

        [ForeignKey("User")]
        public int IdUsuarioCliente { get; set; } 

        [ForeignKey("User")]
        public int IdUsuarioEmpleado { get; set; } 

        [ForeignKey("Service")]
        public int IdServicio { get; set; } 

        public DateTime FechaCita { get; set; } 

        public int Estado { get; set; } 

        public string Observacion { get; set; } = string.Empty; 
    }
}