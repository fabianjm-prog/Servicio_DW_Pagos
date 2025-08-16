using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Servicio_DW_Pagos.Models;

namespace Servicio_DW_Pagos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HistorialMonedasController : Controller
    {
        private readonly AppDbContext _context;

        public HistorialMonedasController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/HistorialMonedas/Listar
        [HttpGet("Listar")]
        public async Task<IActionResult> Listar()
        {
            var lista = await _context.Historial_Monedas
                .AsNoTracking()
                .OrderByDescending(h => h.Fecha)
                .ToListAsync();

            if (lista == null || lista.Count == 0)
                return NotFound(new { mensaje = "No se encontró historial de monedas" });

            return Ok(new { value = lista });
        }

        // GET: api/HistorialMonedas/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _context.Historial_Monedas.FindAsync(id);

            if (item == null)
                return NotFound(new { mensaje = "Registro no encontrado" });

            return Ok(new { value = item });
        }

        // POST: api/HistorialMonedas/Crear
        [HttpPost("Crear")]
        public async Task<IActionResult> Crear([FromBody] Historial_Monedas input)
        {
            if (input.Fecha == null)
                input.Fecha = DateTime.Now;

            _context.Historial_Monedas.Add(input);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Historial creado", value = input });
        }

        // PUT: api/HistorialMonedas/Actualizar/5
        [HttpPut("Actualizar/{id:int}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] Historial_Monedas input)
        {
            var item = await _context.Historial_Monedas.FindAsync(id);

            if (item == null)
                return NotFound(new { mensaje = "Registro no encontrado" });

            // Actualizar solo con lo que existe en el modelo
            item.Codigo = input.Codigo;
            item.Tipo_Cambio = input.Tipo_Cambio;
            item.Fecha = input.Fecha ?? DateTime.Now;

            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Historial actualizado", value = item });
        }

        // DELETE: api/HistorialMonedas/Eliminar/5
        [HttpDelete("Eliminar/{id:int}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var item = await _context.Historial_Monedas.FindAsync(id);

            if (item == null)
                return NotFound(new { mensaje = "Registro no encontrado" });

            _context.Historial_Monedas.Remove(item);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Historial eliminado", value = item });
        }
    }
}
