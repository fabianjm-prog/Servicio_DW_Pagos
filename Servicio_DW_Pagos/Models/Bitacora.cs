using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Servicio_DW_Pagos.Models
{
    public class Bitacora
    {
        [Key]
        public int ID_Bitacora { get; set; }
        public int ID_Usuario { get; set; }
        public int ID_Orden { get; set; }
        public string Tabla { get; set; }
        public string Columna { get; set; }
        public string Valor_Antes { get; set; }
        public string Valor_Despues { get; set; }
        public DateTime Fecha_Mov { get; set; } = DateTime.Now;
        public string Transaccion { get; set; }

       

        [ForeignKey("ID_Usuario")]
        public virtual Usuario Usuario { get; set; }
       


    }
}
