using Microsoft.AspNetCore.Mvc;
using Servicio_DW_Pagos.Models;
using Microsoft.EntityFrameworkCore;
using Servicio_DW_Pagos.Servicios;


namespace Servicio_DW_Pagos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class OrdenPagosController : Controller
    {
        private readonly ServicioTipoCambio _tipoCambioService;
        private readonly AppDbContext _context;

        public OrdenPagosController(AppDbContext context, ServicioTipoCambio tipoCambioService)
        {
            _context = context;
            _tipoCambioService = tipoCambioService;
        }

        [HttpPost("crear")]
        public async Task<IActionResult> CrearOrdenPago([FromBody] Orden_Pago ordenDTO)
        {


            var moneda = await _context.Moneda.FirstOrDefaultAsync(m => m.ID_Moneda == ordenDTO.ID_Moneda);
            if (moneda == null)
                return BadRequest(new { mensaje = "La moneda seleccionada no existe." });



            var tipoCambio = await _tipoCambioService.ObtenerTipoCambioPorCodigo(moneda.Codigo);



            if (tipoCambio == null)
                return BadRequest(new { mensaje = "No se pudo obtener el tipo de cambio." });

            decimal montoConvertido = ordenDTO.Monto * tipoCambio.Value;

            var orden = new Orden_Pago
            {
                ID_Estado = ordenDTO.ID_Estado,
                ID_Usuario = ordenDTO.ID_Usuario,
                ID_Tipo_Pago = ordenDTO.ID_Tipo_Pago,
                ID_Moneda = ordenDTO.ID_Moneda,
                Fecha_Ingreso = DateTime.Now,
                Acreedor = ordenDTO.Acreedor,
                Numero_Factura = ordenDTO.Numero_Factura,
                Monto = ordenDTO.Monto,
                Descuento = ordenDTO.Descuento,
                Impuesto = ordenDTO.Impuesto,
                Documento_compensa = ordenDTO.Documento_compensa,
                Fecha_Factura = ordenDTO.Fecha_Factura,
                Fecha_Pago = ordenDTO.Fecha_Pago,
                Fecha_Vencimiento = ordenDTO.Fecha_Vencimiento,
                Fecha_Revision = ordenDTO.Fecha_Revision,
                Tipo_Cambio = tipoCambio.Value,
                Monto_Convertido = montoConvertido,
                Prioridad = ordenDTO.Prioridad,
                
            };

            
            _context.Orden_Pago.Add(orden);
            await _context.SaveChangesAsync();

            // 5. Devolver respuesta
            return Ok(new { mensaje = "Orden de pago creada exitosamente", orden });
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("Listar")]

        public async Task<IActionResult> ListaOrdenes()
        {
            var listaOrdenes = await _context.Orden_Pago.ToListAsync();

            if (listaOrdenes == null || listaOrdenes.Count == 0)
            {
                return StatusCode(StatusCodes.Status404NotFound, new { mensaje = "No se logro encontrar ninguna Orden de pago" });
            }

            return StatusCode(StatusCodes.Status200OK, new { value = listaOrdenes });
        }
    }
}
