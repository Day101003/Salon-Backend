using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Salon_Info.Data;
using Salon_Info.Models;
using BCrypt.Net;
using Microsoft.AspNetCore.Authorization; // Add this directive
using System.IO;

namespace Salon_Info.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly Salon_InfoContext _context;

        public UsersController(Salon_InfoContext context)
        {
            _context = context;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUser()
        {
            return await _context.User.ToListAsync();
        }

        // GET: api/Users/me
        [HttpGet("me")]
        [Authorize] // requiere token válido
        public IActionResult GetCurrentUser()
        {
            var userId = User.FindFirst("IdUsuario")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "Token inválido." });
            }

            var user = _context.User.Find(int.Parse(userId));
            if (user == null)
            {
                return NotFound(new { message = "Usuario no encontrado." });
            }

            // retornar solo datos necesarios (no contraseña)
            return Ok(new
            {
                idUsuario = user.IdUsuario,
                nombre = user.Nombre,
                correo = user.Correo,
                telefono = user.Telefono,
                rutaImg = user.RutaImg,
                tipo = user.Tipo,
                fechaRegistro = user.FechaRegistro,
                preferencias = user.Preferencias
            });
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.User.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // PUT: api/Users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, [FromForm] UserUpdateDto dto)
        {
            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound(new { message = "Usuario no encontrado." });
            }

            // Actualizar datos básicos
            user.Nombre = dto.Nombre;
            user.Correo = dto.Correo;
            user.Telefono = dto.Telefono ?? "";
            user.Preferencias = dto.Preferencias ?? "";

            // Manejar imagen si se envió
            if (dto.ImagenPerfil != null && dto.ImagenPerfil.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "perfiles");

                // Crear directorio si no existe
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Eliminar imagen anterior si existe
                if (!string.IsNullOrEmpty(user.RutaImg))
                {
                    var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", user.RutaImg.TrimStart('/'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        try
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error al eliminar imagen anterior: {ex.Message}");
                        }
                    }
                }

                // Guardar nueva imagen
                var uniqueFileName = $"{Guid.NewGuid()}_{dto.ImagenPerfil.FileName}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.ImagenPerfil.CopyToAsync(stream);
                }

                user.RutaImg = $"/uploads/perfiles/{uniqueFileName}";
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(new { message = "Perfil actualizado exitosamente." });
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            _context.User.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.IdUsuario }, user);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.User.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        // PATCH: api/Users/5/tipo
        [HttpPatch("{id}/tipo")]
        [Authorize]
        public async Task<IActionResult> UpdateUserType(int id, [FromBody] UpdateTipoDto dto)
        {
            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound(new { message = "Usuario no encontrado." });
            }

            user.Tipo = dto.Tipo;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Tipo de usuario actualizado.", tipo = user.Tipo });
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public IActionResult Register([FromForm] UserRegisterDto dto)
        {
            if (dto == null || string.IsNullOrEmpty(dto.Correo) || string.IsNullOrEmpty(dto.Contrasena))
            {
                return BadRequest(new { message = "Datos de entrada inválidos." });
            }

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Contrasena);

            var user = new User
            {
                Nombre = dto.Nombre,
                Telefono = dto.Telefono ?? "",
                Correo = dto.Correo,
                FechaRegistro = DateTime.Now,
                Contrasena = hashedPassword,
                RutaImg = "",
                Tipo = dto.Tipo ?? 2, // Change to int instead of string
                Preferencias = dto.Preferencias ?? ""
            };

            // Manejar imagen si se envió
            if (dto.ImagenPerfil != null && dto.ImagenPerfil.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "perfiles");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = $"{Guid.NewGuid()}_{dto.ImagenPerfil.FileName}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    dto.ImagenPerfil.CopyTo(stream);
                }

                user.RutaImg = $"/uploads/perfiles/{uniqueFileName}";
            }

            _context.User.Add(user);
            _context.SaveChanges();

            return Ok(new { message = "Usuario registrado exitosamente." });
        }

        private bool UserExists(int id)
        {
            return _context.User.Any(e => e.IdUsuario == id);
        }
    }
}
