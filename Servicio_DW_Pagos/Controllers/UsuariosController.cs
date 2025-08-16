using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Servicio_DW_Pagos.Models;

namespace Servicio_DW_Pagos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : Controller
    {
        private readonly AppDbContext _context;
        public UsuariosController(AppDbContext context) => _context = context;

        [HttpGet("Listar")]
        public async Task<IActionResult> Listar()
        {
            var data = await _context.Usuario.AsNoTracking().ToListAsync();
            if (data == null || data.Count == 0)
                return StatusCode(StatusCodes.Status404NotFound, new { mensaje = "No hay usuarios" });

            return StatusCode(StatusCodes.Status200OK, new { value = data });
        }

        [HttpPost("Crear")]
        public async Task<IActionResult> Crear([FromBody] Usuario input)
        {
            _context.Usuario.Add(input);
            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Usuario creado", value = input });
        }

        [HttpPut("Actualizar/{id:int}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] Usuario input)
        {
            var u = await _context.Usuario.FindAsync(id);
            if (u is null)
                return StatusCode(StatusCodes.Status404NotFound, new { mensaje = "Usuario no encontrado" });

            u.ID_Rol = input.ID_Rol;
            u.Cedula = input.Cedula;
            u.Nombre = input.Nombre;
            u.Apellido1 = input.Apellido1;
            u.Apellido2 = input.Apellido2;
            u.Telefono = input.Telefono;
            u.Correo = input.Correo;
            u.Contrasena = input.Contrasena; // si luego quieres hash, lo cambiamos
            u.Estado = input.Estado;

            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Usuario actualizado" });
        }

        [HttpDelete("Eliminar/{id:int}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var u = await _context.Usuario.FindAsync(id);
            if (u is null)
                return StatusCode(StatusCodes.Status404NotFound, new { mensaje = "Usuario no encontrado" });

            _context.Usuario.Remove(u);
            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Usuario eliminado", value = u });
        }
    }
}
