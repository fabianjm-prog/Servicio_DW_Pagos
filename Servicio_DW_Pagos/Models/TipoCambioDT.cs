using System.ComponentModel.DataAnnotations;

namespace Servicio_DW_Pagos.Models
{
    public class TipoCambioDT
    {
        [Key]
        public string Codigo { get; set; }
        public decimal Tipo_Cambio { get; set; }
    }
}
