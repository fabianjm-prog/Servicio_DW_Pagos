using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Servicio_DW_Pagos.Models;

namespace Servicio_DW_Pagos.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class DevolucionesController : Controller
    {
        private readonly AppDbContext _context;

        public DevolucionesController(AppDbContext context) => _context = context;


        [HttpGet("Listar")]
        public async Task<IActionResult> Listar()
        {
            var listaDevolcuiones = await _context.Devolucion
        .Include(o => o.Usuario)
        .Include(o => o.Tipo_Devolucion)


        .Select(o => new {
            o.ID_Devolucion,
            o.ID_Orden,
            o.ID_Usuario,
            UsuarioNombre = o.Usuario.Nombre,
            o.ID_Tipo_Devolucion,
            Tipodevolucion = o.Tipo_Devolucion.Descripcion,
            o.Fecha_Devolucion,
            o.Estado,
           
        })
        .ToListAsync();
            if (listaDevolcuiones == null || listaDevolcuiones.Count == 0)
                return StatusCode(StatusCodes.Status404NotFound, new { mensaje = "No hay Devolcuiones" });

            return StatusCode(StatusCodes.Status200OK, new { value = listaDevolcuiones });
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
