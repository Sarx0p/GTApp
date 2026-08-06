using GolTime.Models;
using Microsoft.EntityFrameworkCore;

namespace GolTime.Seeds
{
    public class UserSeed
    {
        public UserSeed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Nombre = "Administrador",
                    Correo = "admin@goltime.com",
                    Clave = "6aefc64cd463f5f24c723c6632f06effdd1cd255af7db68cd0dc4c0d80643d7c",
                    Estado = true,
                    RolId = 1,
                    CreatedAt = new DateTime(2026, 7, 26),
                    UpdatedAt = new DateTime(2026, 7, 26),
                },
                new User
                {
                    Id = 2,
                    Nombre = "Empleado",
                    Correo = "empleado@goltime.com",
                    Clave = "6aefc64cd463f5f24c723c6632f06effdd1cd255af7db68cd0dc4c0d80643d7c",
                    Estado = true,
                    RolId = 2,
                    CreatedAt = new DateTime(2026, 7, 26),  
                    UpdatedAt = new DateTime(2026, 7, 26)   
                }
            );
        }
    }
}
