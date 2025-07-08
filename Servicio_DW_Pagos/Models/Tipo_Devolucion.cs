using System.ComponentModel.DataAnnotations;

namespace Servicio_DW_Pagos.Models
{
    public class Tipo_Devolucion
    {
        [Key]
        public int ID_Tipo_Devolucion { get; set; }
        public string Descripcion { get; set; }
        public string Estado { get; set; }
        public DateTime Fecha_Creacion { get; set; } = DateTime.Now;
        }
}
