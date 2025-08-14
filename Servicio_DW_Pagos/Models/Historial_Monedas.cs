using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;

namespace Servicio_DW_Pagos.Models
{
    public class Historial_Monedas
    {
        [Key]
        public int ID_Moneda { get; set; }
        public string Codigo { get; set; }
        public decimal Tipo_Cambio { get; set; }
        public DateTime? Fecha { get; set; } = DateTime.Now;


    }
}
