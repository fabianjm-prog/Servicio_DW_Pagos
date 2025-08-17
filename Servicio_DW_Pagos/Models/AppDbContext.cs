using Microsoft.EntityFrameworkCore;

namespace Servicio_DW_Pagos.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Moneda> Moneda { get; set; }

        public DbSet<Estado_Orden> Estado_Orden { get; set; }
        public DbSet<Orden_Pago> Orden_Pago { get; set; }

        public DbSet<Tipo_Pagocs> Tipo_Pago { get; set; }
        public DbSet<Usuario> Usuario { get; set; }
        public DbSet<Devolucion> Devolucion { get; set; }

        public DbSet<Tipo_Devolucion> Tipo_Devolucion { get; set; }

        public DbSet<Historial_Monedas> Historial_Monedas { get; set; }

        public DbSet<Rol> Rol { get; set; }

        public DbSet<Bitacora> Bitacora { get; set; }








    }
}
