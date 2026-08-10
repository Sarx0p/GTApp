using GolTime.Models;

namespace GolTime.Pdf
{
    public class ReservacionesModel
    {
        public DateOnly FechaInicio { get; set; }

        public DateOnly FechaFin { get; set; }

        public DateTime FechaGeneracion { get; set; }

        public List<Reservacion> Reservaciones { get; set; } = new();
    }
}
