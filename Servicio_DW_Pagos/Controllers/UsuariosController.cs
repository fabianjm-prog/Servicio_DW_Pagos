using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Servicio_DW_Pagos.Models;

namespace Servicio_DW_Pagos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : Controller
    {
        private readonly AppDbContext _context;
        public UsuariosController(AppDbContext context) => _context = context;

        [HttpGet("Listar")]
        public async Task<IActionResult> Listar()
        {
            var listaUsuarios = await _context.Usuario
            .Include(o => o.Rol)
            .Select(o => new {
                o.ID_Usuario,
                o.ID_Rol,
               nombreRol= o.Rol.Descripcion,
               o.Cedula,
                o.Nombre,
                o.Apellido1,
                o.Apellido2,
                o.Telefono,
                o.Correo,
                o.Contrasena,
                o.Estado,
                o.Fecha_Creacion,



            })
        .ToListAsync();
            if (listaUsuarios == null || listaUsuarios.Count == 0)
                return StatusCode(StatusCodes.Status404NotFound, new { mensaje = "No hay Devolcuiones" });

            return StatusCode(StatusCodes.Status200OK, new { value = listaUsuarios });
        }

        [HttpPost("Crear")]
        public async Task<IActionResult> Crear([FromBody] UsuarioDTO inputdto)
        {

            if (inputdto == null)
            {
                return BadRequest(new { mensaje = "Los datos del Usuario son inválidos." });
            }

            try
            {
                var Usuario = new Usuario
                {

                    ID_Rol = inputdto.ID_Rol,
                    Cedula = inputdto.Cedula,
                    Nombre = inputdto.Nombre,
                    Apellido1 = inputdto.Apellido1,
                    Apellido2 = inputdto.Apellido2,
                    Telefono = inputdto.Telefono,
                    Correo = inputdto.Correo,
                    Contrasena = inputdto.Contrasena,
                    Estado = inputdto.Estado,
                    Fecha_Creacion = DateTime.Now



                };

                _context.Usuario.Add(Usuario);
                await _context.SaveChangesAsync();
                return Ok(new { mensaje = "Usuario creado", value = Usuario });


            }
            catch (Exception ex)
            {

                return StatusCode(500, new { mensaje = "Error al crear el usuario.", detalle = ex.InnerException?.Message ?? ex.Message });

            }
            
        }

        [HttpPut("Actualizar/{id:int}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] Usuario input)
        {
            var u = await _context.Usuario.FindAsync(id);
            if (u is null)
                return StatusCode(StatusCodes.Status404NotFound, new { mensaje = "Usuario no encontrado" });

            u.Cedula = input.Cedula;
            u.Nombre = input.Nombre;
            u.Apellido1 = input.Apellido1;
            u.Apellido2 = input.Apellido2;
            u.Telefono = input.Telefono;
            u.Correo = input.Correo;
            u.Contrasena = input.Contrasena; 
            u.Estado = input.Estado;

            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Usuario actualizado" });
        }

        [HttpDelete("Eliminar/{id:int}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var u = await _context.Usuario.FindAsync(id);
            if (u is null)
                return StatusCode(StatusCodes.Status404NotFound, new { mensaje = "Usuario no encontrado" });

            _context.Usuario.Remove(u);
            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Usuario eliminado", value = u });
        }
    }
}
