using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Servicio_DW_Pagos.Models
{
    public class Devolucion
    {
        [Key]
        public int ID_Devolucion { get; set; }
        public int ID_Orden { get; set; }
        public int ID_Usuario { get; set; }
        public int ID_Tipo_Devolucion { get; set; }
        public DateTime Fecha_Devolucion { get; set; }
        public string Estado { get; set; }


        [ForeignKey("ID_Usuario")]
        public virtual Usuario Usuario { get; set; }


        [ForeignKey("ID_Tipo_Devolucion")]

        public virtual Tipo_Devolucion Tipo_Devolucion { get; set; }

    }
}
