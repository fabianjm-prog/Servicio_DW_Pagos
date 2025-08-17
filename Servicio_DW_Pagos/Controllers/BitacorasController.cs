using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Servicio_DW_Pagos.Models;

namespace Servicio_DW_Pagos.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class BitacorasController : Controller
    {

        private readonly AppDbContext _context;

        public BitacorasController(AppDbContext context) => _context = context;



        [HttpGet("Listar")]
        public async Task<IActionResult> Listar()
        {
            var listaBitacora = await _context.Bitacora
        .Include(o => o.Usuario)       
           
        .Select(o => new {
            o.ID_Bitacora,
            o.ID_Orden,
            o.ID_Usuario,
            UsuarioNombre = o.Usuario.Nombre,     
            o.Tabla,
            o.Columna,  
            o.Valor_Antes,
            o.Valor_Despues,
            o.Fecha_Mov,
            o.Transaccion
        })
        .ToListAsync();
            if (listaBitacora == null || listaBitacora.Count == 0)
                return StatusCode(StatusCodes.Status404NotFound, new { mensaje = "No hay Bitacora" });

            return StatusCode(StatusCodes.Status200OK, new { value = listaBitacora });
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
