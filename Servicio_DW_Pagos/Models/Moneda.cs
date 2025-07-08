namespace Servicio_DW_Pagos.Models
{
    public class Moneda
    {
        public int ID_Moneda { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string Estado { get; set; }
        public decimal Tipo_Cambio { get; set; }
        public DateTime? Fecha_Creacion { get; set; } = DateTime.Now;
    }
}
