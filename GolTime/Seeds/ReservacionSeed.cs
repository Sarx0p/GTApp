using GolTime.Models;
using Microsoft.EntityFrameworkCore;

namespace GolTime.Seeds
{
    public class ReservacionSeed
    {
        public ReservacionSeed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Reservacion>().HasData(
               new Reservacion
               {
                   Id = 1,
                   Fecha = new DateOnly(2026, 7, 27),
                   HoraInicio = new TimeOnly(8, 0),
                   HoraFin = new TimeOnly(10, 0),
                   EstadoReserva = Enums.EstadoReservaEnum.Activa,
                   EstadoPago = Enums.EstadoPagoEnum.Pendiente,
                   UserId = 1,
                   ClientId = 1,
                   CreatedAt = new DateTime(2026, 7, 26),
                   UpdatedAt = new DateTime(2026, 7, 26),
               }
           );
        }

    }
}
