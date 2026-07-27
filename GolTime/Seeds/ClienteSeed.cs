using GolTime.Models;
using Microsoft.EntityFrameworkCore;

namespace GolTime.Seeds
{
    public class ClienteSeed
    {
        public ClienteSeed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cliente>().HasData(
                new Cliente
                {
                    Id = 1,
                    Nombre = "Juan Pérez",
                    Numero = "7534-4312",
                    CreatedAt = new DateTime(2026, 7, 26),
                    UpdatedAt = new DateTime(2026, 7, 26)
                },
                new Cliente
                {
                    Id = 2,
                    Nombre = "María López",
                    Numero = "7352-9702",
                    CreatedAt = new DateTime(2026, 7, 26),
                    UpdatedAt = new DateTime(2026, 7, 26)
                },
                new Cliente
                {
                    Id = 3,
                    Nombre = "Carlos Hernández",
                    Numero = "754-0232",
                    CreatedAt = new DateTime(2026, 7, 26),
                    UpdatedAt = new DateTime(2026, 7, 26)   
                }
            );
        }
    }
}
