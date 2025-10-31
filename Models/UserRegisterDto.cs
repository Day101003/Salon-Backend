using Microsoft.AspNetCore.Http;

public class UserRegisterDto
{
    public string Nombre { get; set; }
    public string Telefono { get; set; }
    public string Correo { get; set; }
    public string Contrasena { get; set; }
    public int? Tipo { get; set; }
    public string? Preferencias { get; set; }
    public IFormFile? ImagenPerfil { get; set; }
}