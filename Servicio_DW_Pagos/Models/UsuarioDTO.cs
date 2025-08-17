using System.ComponentModel.DataAnnotations;

namespace Servicio_DW_Pagos.Models
{
    public class UsuarioDTO
    {

        [Key]
        public int ID_Usuario { get; set; }
        public int ID_Rol { get; set; }
        public string Cedula { get; set; }
        public string Nombre { get; set; }
        public string Apellido1 { get; set; }
        public string Apellido2 { get; set; }
        public int Telefono { get; set; }
        public string Correo { get; set; }
        public string Contrasena { get; set; }
        public string Estado { get; set; }
        public DateTime Fecha_Creacion { get; set; } = DateTime.Now;

    }
}
