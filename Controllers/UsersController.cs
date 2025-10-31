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
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            if (id != user.IdUsuario)
            {
                return BadRequest();
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

            return NoContent();
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

        [HttpPost("register")]
        [AllowAnonymous]
        public IActionResult Register([FromBody] UserDto dto)
        {
            if (dto == null || string.IsNullOrEmpty(dto.Correo) || string.IsNullOrEmpty(dto.Contrasena))
            {
                return BadRequest(new { message = "Datos de entrada inválidos." });
            }

            // Generar el hash de la contraseña
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Contrasena);

            var user = new User
            {
                Nombre = dto.Nombre,
                Telefono = dto.Telefono,
                Correo = dto.Correo,
                FechaRegistro = DateTime.Now,
                Contrasena = hashedPassword, // Guardar la contraseña cifrada
                RutaImg = dto.RutaImg,
                Tipo = dto.Tipo,
                Preferencias = dto.Preferencias
            };

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
