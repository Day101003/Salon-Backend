using Salon_Info.Models;

public class UserDto
{
    public string Nombre { get; set; }
    public string Telefono { get; set; }
    public string Correo { get; set; } // Add this property for email
    public string Contrasena { get; set; } // Add this property for password
    public string RutaImg { get; set; }
    public int Tipo { get; set; }
    public string Preferencias { get; set; }
}