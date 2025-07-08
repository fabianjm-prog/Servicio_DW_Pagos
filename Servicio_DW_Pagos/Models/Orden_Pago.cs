namespace Servicio_DW_Pagos.Models
{
    public class Orden_Pago
    {
        public int ID_Orden { get; set; }

        public int ID_Estado { get; set; }
        public int ID_Usuario { get; set; }
        public int ID_Tipo_Pago { get; set; }
        public int ID_Moneda { get; set; }

        public DateTime Fecha_Ingreso { get; set; }
        public DateTime Fecha_Factura { get; set; }
        public DateTime Fecha_Pago { get; set; }
        public DateTime Fecha_Vencimiento { get; set; }
        public DateTime Fecha_Revision { get; set; }

        public string Acreedor { get; set; }

        public string Numero_Factura { get; set; }

        public decimal Monto { get; set; }

        public decimal Descuento { get; set; }

        public decimal Impuesto { get; set; }

        public string Documento_compensa {  get; set; } // revisar que es este atributo

        public decimal Tipo_Cambio { get; set; }

        public decimal Monto_Convertido { get; set; }

        public string Estado {  get; set; } 

        public bool Tipo { get; set; } = false;













    }
}
