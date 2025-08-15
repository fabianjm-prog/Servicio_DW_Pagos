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
        public async Task<IActionResult> CrearOrdenPago([FromBody] OrdenPagoDTO ordenDTO)
        {


            var moneda = await _context.Moneda.FirstOrDefaultAsync(m => m.ID_Moneda == ordenDTO.ID_Moneda);
            if (moneda == null)
                return BadRequest(new { mensaje = "La moneda seleccionada no existe." });



            var tipoCambio = await _tipoCambioService.ObtenerTipoCambioPorCodigo(moneda.Codigo);



            if (tipoCambio == null)
                return BadRequest(new { mensaje = "No se pudo obtener el tipo de cambio." });

            decimal descuentoDecimal = ordenDTO.Descuento / 100;
            decimal impuestoDecimal = ordenDTO.Impuesto / 100;

            decimal montoBase = ordenDTO.Monto;

            if (ordenDTO.Descuento > 0)
            {
                montoBase -= montoBase * descuentoDecimal;
            }

            if (ordenDTO.Impuesto > 0)
            {
                montoBase += montoBase * impuestoDecimal;
            }

            decimal montoConvertido = montoBase * tipoCambio.Value;


            var orden = new Orden_Pago
            {

                ID_Estado = ordenDTO.ID_Estado,
                ID_Usuario = ordenDTO.ID_Usuario,
                ID_Tipo_Pago = ordenDTO.ID_Tipo_Pago,
                ID_Moneda = ordenDTO.ID_Moneda,
                Fecha_Ingreso = DateTime.Now,
                Acreedor = ordenDTO.Acreedor,
                Numero_Factura = ordenDTO.Numero_Factura,
                Monto = montoBase,
                Descuento = ordenDTO.Descuento,
                Impuesto = ordenDTO.Impuesto,
                Documento_compensa = ordenDTO.Documento_compensa,
                Fecha_Factura = ordenDTO.Fecha_Factura,
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
        public async Task<IActionResult> ListaOrdenes([FromQuery] int? tipoPago)
        {
            var listaOrdenes = await _context.Orden_Pago
        .Include(o => o.Estado_Orden)       // Relación con tabla Estado
        .Include(o => o.Usuario)      // Relación con tabla Usuario
        .Include(o => o.Tipo_Pagocs)    // Relación con tabla Tipo_Pago
        .Include(o => o.Moneda)       // Relación con tabla Moneda
        .Select(o => new {
            o.ID_Orden,
            o.ID_Estado,
            EstadoNombre = o.Estado_Orden.Estado,  
            o.ID_Usuario,
            UsuarioNombre = o.Usuario.Nombre,     // Nombre usuario
            o.ID_Tipo_Pago,
            TipoPagoNombre = o.Tipo_Pagocs.Descripcion,
            o.ID_Moneda,
            MonedaNombre = o.Moneda.Codigo,
            o.Fecha_Ingreso,
            o.Fecha_Factura,
            o.Fecha_Pago,
            o.Fecha_Vencimiento,
            o.Fecha_Revision,
            o.Acreedor,
            o.Numero_Factura,
            o.Monto,
            o.Descuento,
            o.Impuesto,
            o.Documento_compensa,
            o.Tipo_Cambio,
            o.Monto_Convertido,
            o.Prioridad
        })
        .ToListAsync();

            if (tipoPago.HasValue)
            {
                listaOrdenes = listaOrdenes.Where(o => o.ID_Tipo_Pago == tipoPago.Value).ToList();
            }

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

        // --------------------------Crud arriba---------------------------------



        // --------------------------Coordinador Metodos---------------------------------

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
        public async Task<IActionResult> MisOrdenes([FromQuery] int userId, [FromQuery] int? tipoPago)
        {
           

            // Traer órdenes del usuario incluyendo las relaciones
            var listaOrdenes = await _context.Orden_Pago
                .Include(o => o.Estado_Orden)
                .Include(o => o.Usuario)
                .Include(o => o.Tipo_Pagocs)
                .Include(o => o.Moneda)
                .Where(o => o.ID_Usuario == userId && o.ID_Estado == 1)
                .AsNoTracking()
                .ToListAsync();

            if (tipoPago.HasValue)
            {
                listaOrdenes = listaOrdenes.Where(o => o.ID_Tipo_Pago == tipoPago.Value).ToList();
            }

            if (!listaOrdenes.Any())
            {
                return NotFound(new { mensaje = "No se encontraron órdenes para el usuario actual con los filtros aplicados." });
            }

            // Proyectar los datos que queremos devolver
            var resultado = listaOrdenes.Select(o => new
            {
                o.ID_Orden,
                o.ID_Estado,
                EstadoNombre = o.Estado_Orden.Estado,
                o.ID_Usuario,
                UsuarioNombre = o.Usuario.Nombre,     // Nombre usuario
                o.ID_Tipo_Pago,
                TipoPagoNombre = o.Tipo_Pagocs.Descripcion,
                o.ID_Moneda,
                MonedaNombre = o.Moneda.Codigo,
                o.Fecha_Ingreso,
                o.Fecha_Factura,
                o.Fecha_Pago,
                o.Fecha_Vencimiento,
                o.Fecha_Revision,
                o.Acreedor,
                o.Numero_Factura,
                o.Monto,
                o.Descuento,
                o.Impuesto,
                o.Documento_compensa,
                o.Tipo_Cambio,
                o.Monto_Convertido,
                o.Prioridad
            });

            return Ok(new { ordenes = resultado });
        }



        // --------------------------Analista Metodos---------------------------------

        [HttpGet("MisOrdenesAnalista")]
        public async Task<IActionResult> MisOrdenesAnalista([FromQuery] int? tipoPago)
        {
            // Traer órdenes con estado 2 (enviado) incluyendo las relaciones
            var listaOrdenes = await _context.Orden_Pago
                .Include(o => o.Estado_Orden)
                .Include(o => o.Usuario)
                .Include(o => o.Tipo_Pagocs)
                .Include(o => o.Moneda)
                .Where(o => o.ID_Estado == 2)
                .AsNoTracking()
                .ToListAsync();

            if (tipoPago.HasValue)
            {
                listaOrdenes = listaOrdenes.Where(o => o.ID_Tipo_Pago == tipoPago.Value).ToList();
            }

            if (!listaOrdenes.Any())
            {
                return NotFound(new { mensaje = "No se encontraron órdenes para el usuario actual con los filtros aplicados." });
            }

            // Proyectar los datos que queremos devolver
            var resultado = listaOrdenes.Select(o => new
            {
                o.ID_Orden,
                o.ID_Estado,
                EstadoNombre = o.Estado_Orden.Estado,
                o.ID_Usuario,
                UsuarioNombre = o.Usuario.Nombre,     // Nombre usuario
                o.ID_Tipo_Pago,
                TipoPagoNombre = o.Tipo_Pagocs.Descripcion,
                o.ID_Moneda,
                MonedaNombre = o.Moneda.Codigo,
                o.Fecha_Ingreso,
                o.Fecha_Factura,
                o.Fecha_Pago,
                o.Fecha_Vencimiento,
                o.Fecha_Revision,
                o.Acreedor,
                o.Numero_Factura,
                o.Monto,
                o.Descuento,
                o.Impuesto,
                o.Documento_compensa,
                o.Tipo_Cambio,
                o.Monto_Convertido,
                o.Prioridad
            });

            return Ok(new { ordenes = resultado });
        }






        [HttpPut("CambiarEstadoACreada/{id}")]
        public async Task<IActionResult> CambiarEstadoACreada(int id, [FromBody] DevolucionDT devolucioon)
        {
            var orden = await _context.Orden_Pago.FindAsync(id);
            var Devolucion = await _context.Devolucion.FindAsync(id);


            if (orden == null)
            {
                return NotFound(new { mensaje = $"Orden con ID {id} no encontrada." });
            }

            if (orden.ID_Estado != 2)
            {
                return BadRequest(new { mensaje = "La orden no está en estado 'Enviada' (ID_Estado = 2), no puede ser devuelta" });
            }

            orden.ID_Estado = 1;

            // crear devolucion

            var nuevaDevolucion = new Devolucion
            {
                ID_Orden = orden.ID_Orden,
                ID_Usuario = orden.ID_Usuario,
                ID_Tipo_Devolucion = devolucioon.ID_Tipo_Devolucion,
                Fecha_Devolucion = DateTime.Now,
                Estado = "Pendiente" 
            };
            try
            {
                // Guardar cambios en Orden y agregar devolución
                _context.Devolucion.Add(nuevaDevolucion);
                await _context.SaveChangesAsync();
               

                return Ok(new
                {
                    mensaje = " la devolución fue registrada correctamente.",
                    orden,
                    devolucion = nuevaDevolucion
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    mensaje = "Error al actualizar la orden o crear la devolución.",
                    error = ex.InnerException?.Message ?? ex.Message
                });
            }
        }




        [HttpPut("CambiarEstadoAPagada/{id}")]
        public async Task<IActionResult> CambiarEstadoAPagada(int id)
        {
            var orden = await _context.Orden_Pago.FindAsync(id);

            if (orden == null)
            {
                return NotFound(new { mensaje = $"Orden con ID {id} no encontrada." });
            }

            if (orden.ID_Estado != 2)
            {
                return BadRequest(new { mensaje = "La orden no está en estado 'Enviada' (ID_Estado = 2), por lo tanto no se puede cambiar a 'Pagada'." });
            }

            orden.ID_Estado = 3;

            orden.Fecha_Pago = DateTime.Now;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new { mensaje = "El estado de la orden ha sido cambiado a 'Pagada' correctamente.", orden });
            }
            catch (DbUpdateException ex)
            {
                var innerMessage = ex.InnerException?.Message ?? ex.Message;
                return StatusCode(500, new { mensaje = "Error al actualizar el estado de la orden.", error = innerMessage });
            }
        }
        [HttpGet("EstadisticasPagos")]
        public async Task<IActionResult> ObtenerEstadisticas()
        {
            try
            {
                //  Pagos generados coordinador
                var pagosPorCoordinador = await _context.Orden_Pago
                    .GroupBy(o => o.ID_Usuario)
                    .Select(g => new
                    {
                        ID_Coordinador = g.Key,
                        CantidadPagosGenerados = g.Count()
                    })
                    .ToListAsync();

                //Pagos revisados analista
                var pagosRevisadosPorAnalista = await _context.Orden_Pago
                    .Where(o => o.ID_Estado == 2) 
                    .GroupBy(o => o.ID_Usuario)
                    .Select(g => new
                    {
                        ID_Analista = g.Key,
                        CantidadRevisados = g.Count()
                    })
                    .ToListAsync();

                //Reportes por tipo de pago
                var reportesPorTipoPago = await _context.Orden_Pago
                    .GroupBy(o => o.ID_Tipo_Pago)
                    .Select(g => new
                    {
                        ID_TipoPago = g.Key,
                        Cantidad = g.Count()
                    })
                    .ToListAsync();

                // Cantidad total pagos
                var pagosRealizados = await _context.Orden_Pago
                    .CountAsync(o => o.ID_Estado == 3);

                return Ok(new
                {
                    PagosPorCoordinador = pagosPorCoordinador,
                    PagosRevisadosPorAnalista = pagosRevisadosPorAnalista,
                    ReportesPorTipoPago = reportesPorTipoPago,
                    TotalPagosRealizados = pagosRealizados
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    mensaje = "Error al obtener estadísticas.",
                    error = ex.InnerException?.Message ?? ex.Message
                });
            }
        }




    }
}
