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
            if (input == null)
            {
                return BadRequest(new { mensaje = "Los datos del Rol son inválidos." });
            }

            var existingRole = await _context.Rol.FirstOrDefaultAsync(r => r.Descripcion == input.Descripcion);
            if (existingRole != null)
                return StatusCode(StatusCodes.Status400BadRequest, new { mensaje = "El rol ya existe." });

            try { 
               
    
                _context.Rol.Add(input);
                await _context.SaveChangesAsync();
                return Ok(new { mensaje = "Rol creado", value = input });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al verificar el rol.", detalle = ex.InnerException?.Message ?? ex.Message });
            }
            
        }

        [HttpPut("Actualizar/{id:int}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] Rol input)
        {
            var item = await _context.Rol.FindAsync(id);
            if (item is null)
                return StatusCode(StatusCodes.Status404NotFound, new { mensaje = "Rol no encontrado" });

            item.Descripcion = input.Descripcion;
           

            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Rol actualizado" });
        }

        [HttpDelete("Eliminar/{id:int}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var item = await _context.Rol.FindAsync(id);
            if (item is null)
                return StatusCode(StatusCodes.Status404NotFound, new { mensaje = "Rol no encontrado" });

            _context.Rol.Remove(item);
            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Rol eliminado", value = item });
        }


    }
}
