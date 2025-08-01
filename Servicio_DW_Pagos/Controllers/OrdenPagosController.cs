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
            var listaOrdenes = await _context.Orden_Pago
                .Include(o => o.Moneda)
                .Include(o => o.TipoPago)
                .Include(o => o.EstadoOrden)
                .Include(o => o.Usuario)
                .ToListAsync();

            if (listaOrdenes == null || listaOrdenes.Count == 0)
            {
                return StatusCode(StatusCodes.Status404NotFound, new { mensaje = "No se logró encontrar ninguna Orden de pago" });
            }

            return StatusCode(StatusCodes.Status200OK, new { value = listaOrdenes });
        }




        [HttpDelete]
        [Route("Eliminar/{ID_Orden}")]
        public async Task<IActionResult> EliminarOrden(int ID_Orden)
        {
            if (ID_Orden <= 0)
            {
                return BadRequest(new { mensaje = "El ID proporcionado no es valido." });
            }

            try
            {
                var movimiento = await _context.Orden_Pago.FirstOrDefaultAsync(m => m.ID_Orden == ID_Orden);

                if (movimiento == null)
                {
                    return NotFound(new { mensaje = "La orden con ID" + ID_Orden + "no econtrada" });
                }

                _context.Orden_Pago.Remove(movimiento);
                await _context.SaveChangesAsync();

                return Ok(new { mensaje = "Orden de pago eliminada correctamente.", value = movimiento });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al eliminar", error = ex.InnerException?.Message ?? ex.Message });
            }
        }



        [HttpPut("Actualizar/{id}")]
        public async Task<IActionResult> ActualizarOrden(int id, [FromBody] Orden_Pago OrdenPago)
        {
            if (id != OrdenPago.ID_Orden)
            {
                return BadRequest(new { mensaje = "El ID de Orden no existe" });
            }

            var OrdenEx = await _context.Orden_Pago.FindAsync(id);
            if (OrdenEx == null)
            {
                return NotFound(new { mensaje = "Orden no encontrada" });
            }

            OrdenEx.ID_Orden = OrdenEx.ID_Orden;

            //Cris aqui tenes que agregar los campos que se tienen que actualizar

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new { mensaje = "Orden de pago actualizado correctamente." });
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new { mensaje = "Error al actualizar Orden de pago.", detalle = ex.Message });
            }
        }

        [HttpPut("CambiarEstadoAEnviada/{id}")]
        public async Task<IActionResult> CambiarEstadoAEnviada(int id)
        {
            var orden = await _context.Orden_Pago.FindAsync(id);

            if (orden == null)
            {
                return NotFound(new { mensaje = $"Orden con ID {id} no encontrada." });
            }

            if (orden.ID_Estado != 1)
            {
                return BadRequest(new { mensaje = "La orden no está en estado 'creada' (ID_Estado = 1), por lo tanto no se puede cambiar a 'enviada'." });
            }

            orden.ID_Estado = 2;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new { mensaje = "El estado de la orden ha sido cambiado a 'enviada' correctamente.", orden });
            }
            catch (DbUpdateException ex)
            {
                var innerMessage = ex.InnerException?.Message ?? ex.Message;
                return StatusCode(500, new { mensaje = "Error al actualizar el estado de la orden.", error = innerMessage });
            }
        }

        [HttpGet("MisOrdenes")]
        public async Task<IActionResult> MisOrdenes([FromQuery] int? tipoPago)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return Unauthorized(new { mensaje = "Debe iniciar sesión para ver sus órdenes de pago." });
            }

            var listaOrdenes = await _context.Orden_Pago
                .Include(o => o.Moneda)
                .Include(o => o.TipoPago)
                .Include(o => o.EstadoOrden)
                .Include(o => o.Usuario)
                .ToListAsync();

            if (listaOrdenes == null || listaOrdenes.Count == 0)
            {
                return NotFound(new { mensaje = "No hay órdenes de pago registradas." });
            }

            var ordenesDelUsuario = listaOrdenes.Where(o => o.ID_Usuario == userId);

            if (tipoPago.HasValue)
            {
                ordenesDelUsuario = ordenesDelUsuario.Where(o => o.ID_Tipo_Pago == tipoPago.Value);
            }

            if (!ordenesDelUsuario.Any())
            {
                return NotFound(new { mensaje = "No se encontraron órdenes para el usuario actual con los filtros aplicados." });
            }

            return Ok(new { ordenes = ordenesDelUsuario });
        }


        // aqui recuerde agregar los metodos que necesite para el controlador de Ordenes de Pago 
        // que estan en el proyecto el el documento. 
        //adapta todo 
    }
}
