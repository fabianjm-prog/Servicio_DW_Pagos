using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Servicio_DW_Pagos.Models;
using Servicio_DW_Pagos.Models.Login;


namespace Servicio_DW_Pagos.Controllers
{

    [Route("api/[Controller]")]


    [ApiController]
    public class LoginController : Controller
    {

        private readonly AppDbContext _context;

        public LoginController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("Login")]

        public async Task<IActionResult> Login(Login objeto)
        {
            var usuarioEncontrado = await _context.Usuario.Where(u => u.Correo == objeto.Correo && u.Contrasena == objeto.Contrasena).FirstOrDefaultAsync();

            if (usuarioEncontrado == null)
            {
                return Unauthorized(new { message = "Credenciales incorrectas" });
            }

            HttpContext.Session.SetInt32("UserId", usuarioEncontrado.ID_Usuario);
            HttpContext.Session.SetString("UserName", usuarioEncontrado.Nombre);

            return Ok(new
            {
                message = "Inicio de sesión exitoso",
                usuario = new
                {
                    Id = usuarioEncontrado.ID_Usuario,
                    Nombre = usuarioEncontrado.Nombre,
                    Correo = usuarioEncontrado.Correo
                }
            });
        }

        [HttpGet("Logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return Ok(new { message = "Sesion cerrada correctamente" });
        }


        public IActionResult Index()
        {
            return View();
        }
    }
}
