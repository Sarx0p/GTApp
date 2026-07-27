using GolTime.Seeds;
using Microsoft.EntityFrameworkCore;




namespace GolTime.Models
{
    public class GoltimeContext : DbContext
    {
        public GoltimeContext(DbContextOptions<GoltimeContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Reservacion> Reservaciones { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Reservacion>()
                .Property(r => r.EstadoReserva)
                .HasConversion<string>();

            modelBuilder.Entity<Reservacion>()
                .Property(r => r.EstadoPago)
                .HasConversion<string>();

            new RolSeed(modelBuilder);
            new ReservacionSeed(modelBuilder);
            new ClienteSeed(modelBuilder);
            new UserSeed(modelBuilder);

        }
    }
}

