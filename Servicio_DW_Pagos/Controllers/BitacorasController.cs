using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Servicio_DW_Pagos.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

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

        [HttpGet("GenerarPDF")]

        public IActionResult GenerarPDF()
        {
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

            var listaBitacora = _context.Bitacora
                .Include(o => o.Usuario)
                .Select(o => new
                {
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
                .ToList();
            if (listaBitacora == null || listaBitacora.Count == 0)
                return StatusCode(StatusCodes.Status404NotFound, new { mensaje = "No hay Bitacora" });


            var pdfBytes = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(20);
                    page.Size(PageSizes.A4);

                    page.Header().Text("Bitacora").FontSize(20).Bold().AlignCenter();

                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(50); 
                            columns.RelativeColumn();    
                            columns.RelativeColumn();   
                            columns.RelativeColumn();   
                            columns.RelativeColumn();   
                            columns.RelativeColumn();   
                            columns.RelativeColumn();   
                            columns.RelativeColumn();
                        });

                        // Encabezado
                        table.Header(header =>
                        {
                            header.Cell().Text("ID").Bold();
                            header.Cell().Text("Usuario").Bold();
                            header.Cell().Text("Tabla").Bold();
                            header.Cell().Text("Columna").Bold();
                            header.Cell().Text("Valor Antes").Bold();
                            header.Cell().Text("Valor Después").Bold();
                            header.Cell().Text("Fecha").Bold();
                            header.Cell().Text("Transacción").Bold();
                        });

                        // Filas
                        foreach (var b in listaBitacora)
                        {
                            table.Cell().Text(b.ID_Bitacora.ToString());
                            table.Cell().Text(b.UsuarioNombre);
                            table.Cell().Text(b.Tabla);
                            table.Cell().Text(b.Columna);
                            table.Cell().Text(b.Valor_Antes);
                            table.Cell().Text(b.Valor_Despues);
                            table.Cell().Text(b.Fecha_Mov.ToString("yyyy-MM-dd HH:mm"));
                            table.Cell().Text(b.Transaccion);
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
            return File(pdfBytes, "application/pdf", "Bitacora.pdf");
        }


        public IActionResult Index()
        {
            return View();
        }
    }
}
