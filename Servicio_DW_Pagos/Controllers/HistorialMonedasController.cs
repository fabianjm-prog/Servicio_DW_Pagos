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

        
    }
}
