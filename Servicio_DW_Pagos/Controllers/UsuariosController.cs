using Microsoft.AspNetCore.Mvc;
using Servicio_DW_Pagos.Models;
using Microsoft.EntityFrameworkCore;
using Servicio_DW_Pagos.Servicios;

namespace Servicio_DW_Pagos.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : Controller
    {
        private readonly ServicioTipoCambio _tipoCambioService;
        private readonly AppDbContext _context;

        public UsuariosController(AppDbContext context, ServicioTipoCambio tipoCambioService)
        {
            _context = context;
            _tipoCambioService = tipoCambioService;
        }


        [HttpGet]
        [Route("Listar")]
        public async Task<IActionResult> ListaOrdenes()
        {
            var listaUsuarios = await _context.Usuario.ToListAsync();

            if (listaUsuarios == null || listaUsuarios.Count == 0)
            {
                return StatusCode(StatusCodes.Status404NotFound, new { mensaje = "No se logró encontrar ningun usuario" });
            }

            return StatusCode(StatusCodes.Status200OK, new { value = listaUsuarios });
        }



        //----------------crear usuario 


        //----------------editar usuario 
        //----------------Eliminar usuario 




        public IActionResult Index()
        {
            return View();
        }



    }
}
