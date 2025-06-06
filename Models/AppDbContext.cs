using Microsoft.EntityFrameworkCore;

namespace BilleteraCriptoProg3.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Transaccion> Transacciones { get; set; }
    }
}
