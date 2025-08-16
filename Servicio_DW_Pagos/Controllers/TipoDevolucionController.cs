using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Servicio_DW_Pagos.Models;

namespace Servicio_DW_Pagos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TipoDevolucionController : Controller
    {
        private readonly AppDbContext _context;
        public TipoDevolucionController(AppDbContext context) => _context = context;

        [HttpGet("Listar")]
        public async Task<IActionResult> Listar()
        {
            var data = await _context.Tipo_Devolucion.AsNoTracking().ToListAsync();
            if (data == null || data.Count == 0)
                return StatusCode(StatusCodes.Status404NotFound, new { mensaje = "No hay tipos de devolución" });

            return StatusCode(StatusCodes.Status200OK, new { value = data });
        }

        [HttpPost("Crear")]
        public async Task<IActionResult> Crear([FromBody] Tipo_Devolucion input)
        {
            _context.Tipo_Devolucion.Add(input);
            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Tipo de devolución creado", value = input });
        }

        [HttpPut("Actualizar/{id:int}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] Tipo_Devolucion input)
        {
            var item = await _context.Tipo_Devolucion.FindAsync(id);
            if (item is null)
                return StatusCode(StatusCodes.Status404NotFound, new { mensaje = "Tipo de devolución no encontrado" });

            item.Descripcion = input.Descripcion;
            item.Estado = input.Estado;

            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Tipo de devolución actualizado" });
        }

        [HttpDelete("Eliminar/{id:int}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var item = await _context.Tipo_Devolucion.FindAsync(id);
            if (item is null)
                return StatusCode(StatusCodes.Status404NotFound, new { mensaje = "Tipo de devolución no encontrado" });

            _context.Tipo_Devolucion.Remove(item);
            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Tipo de devolución eliminado", value = item });
        }
    }
}
