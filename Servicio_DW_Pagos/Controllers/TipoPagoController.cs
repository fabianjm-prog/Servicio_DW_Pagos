using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Servicio_DW_Pagos.Models;

namespace Servicio_DW_Pagos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TipoPagoController : Controller
    {
        private readonly AppDbContext _context;
        public TipoPagoController(AppDbContext context) => _context = context;

        [HttpGet("Listar")]
        public async Task<IActionResult> Listar()
        {
            var data = await _context.Tipo_Pago.AsNoTracking().ToListAsync();
            if (data == null || data.Count == 0)
                return StatusCode(StatusCodes.Status404NotFound, new { mensaje = "No hay tipos de pago" });

            return StatusCode(StatusCodes.Status200OK, new { value = data });
        }

        [HttpPost("Crear")]
        public async Task<IActionResult> Crear([FromBody] Tipo_Pagocs input)
        {

            if (input == null)
            {
                return BadRequest(new { mensaje = "Los datos del Tipo Pago son inválidos." });
            }
            try
            {
                _context.Tipo_Pago.Add(input);
            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Tipo de pago creado", value = input });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al Crear el Tipo Pago.", detalle = ex.InnerException?.Message ?? ex.Message });
            }
        }

        [HttpPut("Actualizar/{id:int}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] Tipo_Pagocs input)
        {
            var item = await _context.Tipo_Pago.FindAsync(id);
            if (item is null)
                return StatusCode(StatusCodes.Status404NotFound, new { mensaje = "Tipo de pago no encontrado" });

            item.Descripcion = input.Descripcion;
            item.Siglas = input.Siglas;
            item.Estado = input.Estado;

            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Tipo de pago actualizado" });
        }

        [HttpDelete("Eliminar/{id:int}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var item = await _context.Tipo_Pago.FindAsync(id);
            if (item is null)
                return StatusCode(StatusCodes.Status404NotFound, new { mensaje = "Tipo de pago no encontrado" });

            _context.Tipo_Pago.Remove(item);
            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Tipo de pago eliminado", value = item });
        }
    }
}
