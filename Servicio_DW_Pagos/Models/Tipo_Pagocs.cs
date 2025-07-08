using System.ComponentModel.DataAnnotations;

namespace Servicio_DW_Pagos.Models
{
    public class Tipo_Pagocs
    {
        [Key]
        public int ID_Tipo_Pago { get; set; }
        public string Descripcion { get; set; }
        public string Siglas { get; set; }
        public string Estado { get; set; }
        public DateTime Fecha_Creacion { get; set; } = DateTime.Now;
    }
}
