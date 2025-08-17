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

            if (input == null)
            {
                return BadRequest(new { mensaje = "Los datos del Tipo_Devolucion son inválidos." });
            }

            try
            {
                _context.Tipo_Devolucion.Add(input);
                await _context.SaveChangesAsync();
                return Ok(new { mensaje = "Tipo Devolucion creada", value = input });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al Crear el Tipo Devolucion.", detalle = ex.InnerException?.Message ?? ex.Message });
            }


        }

        [HttpPut("Actualizar/{id:int}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] Tipo_Devolucion input)
        {
            var item = await _context.Tipo_Devolucion.FindAsync(id);
            if (item is null)
                return StatusCode(StatusCodes.Status404NotFound, new { mensaje = "Tipo de Devolucion no encontrado" });

            item.Descripcion = input.Descripcion;
            item.Estado = input.Estado;
            item.Fecha_Creacion = input.Fecha_Creacion;


            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Tipo de Devolucion actualizado" });
        }



        [HttpDelete("Eliminar/{id:int}")]
        public async Task<IActionResult> Eliminar(int id)
        {

            var moneda = await _context.Tipo_Devolucion.FindAsync(id);
            if (moneda == null)
                return StatusCode(StatusCodes.Status404NotFound, new { mensaje = "Tipo de Devolucion no encontrada" });

            try
            {
                _context.Tipo_Devolucion.Remove(moneda);
                await _context.SaveChangesAsync();
                return Ok(new { mensaje = "Tipo de Devolucion eliminada", value = moneda });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al eliminar el Tipo de Devolucion.", detalle = ex.InnerException?.Message ?? ex.Message });
            }


        }








        

        
    }
}
