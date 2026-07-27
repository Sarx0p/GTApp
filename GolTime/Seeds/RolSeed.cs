using GolTime.Models;
using Microsoft.EntityFrameworkCore;

namespace GolTime.Seeds
{
    public class RolSeed
    {
        public RolSeed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Rol>().HasData(
                new Rol
                {
                    Id = 1,
                    Nombre = "Administrador",
                    CreatedAt = new DateTime(2026, 7, 26),
                    UpdatedAt = new DateTime(2026, 7, 26)
                },
                new Rol
                {
                    Id = 2,
                    Nombre = "Empleado",
                    CreatedAt = new DateTime(2026, 7, 26),
                    UpdatedAt = new DateTime(2026, 7, 26),
                }
            );
        }
    }
}
