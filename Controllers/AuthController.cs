using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Salon_Info.Data;
using Salon_Info.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization; // Add this directive

namespace Salon_Info.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly Salon_InfoContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(Salon_InfoContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("login")]
        [AllowAnonymous] // Permitir acceso sin token
        public IActionResult Login([FromBody] LoginDto dto)
        {
            // Validar usuario
            if (dto == null || string.IsNullOrEmpty(dto.Email) || string.IsNullOrEmpty(dto.Password))
            {
                return BadRequest(new { message = "Datos de entrada inválidos." });
            }

            var user = _context.User.FirstOrDefault(u => u.Correo == dto.Email && u.Contrasena == dto.Password);
            if (user == null)
            {
                return Unauthorized(new { message = "Credenciales inválidas." });
            }

            var token = GenerateJwtToken(user);

            // Devolver el token en el cuerpo de la respuesta
            return Ok(new { token });
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Correo),
                new Claim("IdUsuario", user.IdUsuario.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class LoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}