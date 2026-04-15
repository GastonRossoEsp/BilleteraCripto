using BilleteraCriptoProg3.Entities;
using Microsoft.EntityFrameworkCore;

namespace BilleteraCriptoProg3.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Transaccion> Transacciones { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Cliente>()
                .Property(c => c.Saldo)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Entities.Transaccion>()
                .Property(t => t.Dinero)
                .HasPrecision(18, 2);
        }
    }
}
