using Microsoft.AspNetCore.Http;

public class UserUpdateDto
{
    public string Nombre { get; set; }
    public string Correo { get; set; }
    public string? Telefono { get; set; }
    public string? Preferencias { get; set; }
    public IFormFile? ImagenPerfil { get; set; }
}