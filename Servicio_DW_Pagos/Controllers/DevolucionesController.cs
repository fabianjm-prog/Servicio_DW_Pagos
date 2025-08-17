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

        [HttpGet("GenerarPDF")]

        public IActionResult GenerarPDF()
        {
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

            var listaDevoluciones = _context.Devolucion
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
        })
                .ToList();
            if (listaDevoluciones == null || listaDevoluciones.Count == 0)
                return StatusCode(StatusCodes.Status404NotFound, new { mensaje = "No hay Devoluciones" });


            var pdfBytes = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(20);
                    page.Size(PageSizes.A4);

                    page.Header().Text("Devoluciones").FontSize(20).Bold().AlignCenter();

                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(50);
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                           
                        });

                        // Encabezado
                        table.Header(header =>
                        {
                            header.Cell().Text("ID").Bold();
                            header.Cell().Text("Orden").Bold();
                            header.Cell().Text("Usuario").Bold();
                            header.Cell().Text("Tipo devolucion").Bold();
                            header.Cell().Text("Fecha").Bold();
                        });

                        // Filas
                        foreach (var b in listaDevoluciones)
                        {
                            table.Cell().Text(b.ID_Tipo_Devolucion.ToString());
                            table.Cell().Text(b.ID_Orden);
                            table.Cell().Text(b.UsuarioNombre);
                            table.Cell().Text(b.Tipodevolucion);
                            table.Cell().Text(b.Fecha_Devolucion.ToString("yyyy-MM-dd HH:mm"));
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
            return File(pdfBytes, "application/pdf", "Devoluciones.pdf");
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
