using System.ComponentModel.DataAnnotations;

namespace Servicio_DW_Pagos.Models
{
    public class Rol
    {
        [Key]
        public int ID_Rol { get; set; }
        public string Descripcion { get; set; }
    }
}
