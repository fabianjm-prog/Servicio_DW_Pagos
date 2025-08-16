using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Servicio_DW_Pagos.Models;

namespace Servicio_DW_Pagos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MonedasController : Controller
    {
        private readonly AppDbContext _context;
        public MonedasController(AppDbContext context) => _context = context;

        [HttpGet("Listar")]
        public async Task<IActionResult> Listar()
        {
            var data = await _context.Moneda.AsNoTracking().ToListAsync();
            if (data == null || data.Count == 0)
                return StatusCode(StatusCodes.Status404NotFound, new { mensaje = "No se encontró ninguna moneda" });

            return Ok(new { value = data });
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var moneda = await _context.Moneda.FindAsync(id);
            return moneda is null
                ? StatusCode(StatusCodes.Status404NotFound, new { mensaje = "Moneda no encontrada" })
                : Ok(new { value = moneda });
        }

        [HttpPost("Crear")]
        public async Task<IActionResult> Crear([FromBody] Moneda input)
        {
            _context.Moneda.Add(input);
            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Moneda creada", value = input });
        }

        [HttpPut("Actualizar/{id:int}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] Moneda input)
        {
            var moneda = await _context.Moneda.FirstOrDefaultAsync(x => x.ID_Moneda == id);
            if (moneda is null)
                return StatusCode(StatusCodes.Status404NotFound, new { mensaje = "Moneda no encontrada" });

            var tipoCambioAnterior = moneda.Tipo_Cambio;

            moneda.Codigo = input.Codigo;
            moneda.Nombre = input.Nombre;
            moneda.Estado = input.Estado;
            moneda.Tipo_Cambio = input.Tipo_Cambio;

            try
            {
                await _context.SaveChangesAsync();

                // Si cambió el tipo de cambio, registrar en historial
                if (tipoCambioAnterior != input.Tipo_Cambio)
                {
                    var hist = new Historial_Monedas
                    {
                        ID_Moneda = moneda.ID_Moneda,
                        Codigo = moneda.Codigo,
                        Tipo_Cambio = input.Tipo_Cambio,
                        Fecha = DateTime.Now
                    };
                    _context.Historial_Monedas.Add(hist);
                    await _context.SaveChangesAsync();
                }

                return Ok(new { mensaje = "Moneda actualizada correctamente." });
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new { mensaje = "Error al actualizar la moneda.", detalle = ex.InnerException?.Message ?? ex.Message });
            }
        }

        [HttpDelete("Eliminar/{id:int}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var moneda = await _context.Moneda.FindAsync(id);
            if (moneda is null)
                return StatusCode(StatusCodes.Status404NotFound, new { mensaje = "Moneda no encontrada" });

            _context.Moneda.Remove(moneda);
            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Moneda eliminada", value = moneda });
        }
    }
}
