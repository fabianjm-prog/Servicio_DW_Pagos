namespace Servicio_DW_Pagos.Models
{
    public class Devolucion
    {

        public int ID_Devolucion { get; set; }
        public int ID_Orden { get; set; }
        public int ID_Usuario { get; set; }
        public int ID_Tipo_Devolucion { get; set; }
        public DateTime Fecha_Devolucion { get; set; }
        public string Estado { get; set; }

    }
}
