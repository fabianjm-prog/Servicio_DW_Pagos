using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Helpers;
using Servicio_DW_Pagos.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

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
        [HttpGet("GenerarPDF")]

        public IActionResult GenerarPDF()
        {
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

            var listaHistorial = _context.Historial_Monedas
                
                .Select(o => new {
                    o.ID_Moneda,
                    o.Codigo,
                    o.Tipo_Cambio,
                    o.Fecha
                   
                })
                .ToList();
            if (listaHistorial == null || listaHistorial.Count == 0)
                return StatusCode(StatusCodes.Status404NotFound, new { mensaje = "No hay Historial" });


            var pdfBytes = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(20);
                    page.Size(PageSizes.A4);

                    page.Header().Text("Historial de Tipo de cambio").FontSize(20).Bold().AlignCenter();

                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(50);
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();

                        });

                        // Encabezado
                        table.Header(header =>
                        {
                            header.Cell().Text("ID").Bold();
                            header.Cell().Text("Codigo").Bold();
                            header.Cell().Text("Tipo Cambio").Bold();
                            header.Cell().Text("Fecha").Bold();
                        });

                        // Filas
                        foreach (var b in listaHistorial)
                        {
                            table.Cell().Text(b.ID_Moneda.ToString());
                            table.Cell().Text(b.Codigo);
                            table.Cell().Text(b.Tipo_Cambio);
                            table.Cell().Text(b.Fecha.ToString("yyyy-MM-dd HH:mm"));
                        }
                    });

                    page.Footer().AlignCenter().Text(txt =>
                    {
                        txt.Span("Página ");
                        txt.CurrentPageNumber();
                        txt.Span(" de ");
                        txt.TotalPages();
                    });
                });
            }).GeneratePdf();

            // Devolver PDF como archivo
            return File(pdfBytes, "application/pdf", "HistorialMonedas.pdf");
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
