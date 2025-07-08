using System.ComponentModel.DataAnnotations;

namespace Servicio_DW_Pagos.Models
{
    public class Estado_Orden
    {
        [Key]
        public int ID_Estado { get; set; }
        public string Estado { get; set; }
        public DateTime Fecha_Creacion { get; set; } = DateTime.Now;
    }
}
