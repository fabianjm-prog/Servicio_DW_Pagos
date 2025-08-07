using Microsoft.AspNetCore.Mvc;
using Servicio_DW_Pagos.Models;
using Microsoft.EntityFrameworkCore;
using Servicio_DW_Pagos.Servicios;

namespace Servicio_DW_Pagos.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class MonedasController : Controller
    {
        private readonly ServicioTipoCambio _tipoCambioService;
        private readonly AppDbContext _context;

        public MonedasController(AppDbContext context, ServicioTipoCambio tipoCambioService)
        {
            _context = context;
            _tipoCambioService = tipoCambioService;
        }

        [HttpGet]
        [Route("Listar")]
        public async Task<IActionResult> ListaOrdenes()
        {
            var listaMonedas = await _context.Moneda.ToListAsync();

            if (listaMonedas == null || listaMonedas.Count == 0)
            {
                return StatusCode(StatusCodes.Status404NotFound, new { mensaje = "No se logró encontrar ningun usuario" });
            }

            return StatusCode(StatusCodes.Status200OK, new { value = listaMonedas });
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
