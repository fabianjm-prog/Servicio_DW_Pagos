using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Servicio_DW_Pagos.Models;

namespace Servicio_DW_Pagos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : Controller
    {
        private readonly AppDbContext _context;
        public RolesController(AppDbContext context) => _context = context;

        [HttpGet("Listar")]
        public async Task<IActionResult> Listar()
        {
            var data = await _context.Rol.AsNoTracking().ToListAsync();
            if (data == null || data.Count == 0)
                return StatusCode(StatusCodes.Status404NotFound, new { mensaje = "No hay roles" });

            return StatusCode(StatusCodes.Status200OK, new { value = data });
        }

        [HttpPost("Crear")]
        public async Task<IActionResult> Crear([FromBody] Rol input)
        {
            _context.Rol.Add(input);
            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Rol creado", value = input });
        }

        public class AsignarRolDto { public int ID_Usuario { get; set; } public int ID_Rol { get; set; } }

        [HttpPost("Asignar")]
        public async Task<IActionResult> Asignar([FromBody] AsignarRolDto dto)
        {
            var u = await _context.Usuario.FindAsync(dto.ID_Usuario);
            var r = await _context.Rol.FindAsync(dto.ID_Rol);
            if (u is null || r is null)
                return StatusCode(StatusCodes.Status404NotFound, new { mensaje = "Usuario o Rol no existe." });

            u.ID_Rol = r.ID_Rol;
            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Rol asignado al usuario" });
        }

        [HttpDelete("Eliminar/{id:int}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var r = await _context.Rol.FindAsync(id);
            if (r is null)
                return StatusCode(StatusCodes.Status404NotFound, new { mensaje = "Rol no encontrado" });

            _context.Rol.Remove(r);
            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Rol eliminado", value = r });
        }
    }
}
