using GolTime.Models;
using GolTime.Pdf;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace GolTime.Controllers
{
    public class ReporteController : Controller
    {

        private readonly GoltimeContext _context;

        public ReporteController(GoltimeContext context)
        {
            _context = context;
            QuestPDF.Settings.License = LicenseType.Community;
        }


        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IResult GenerarReporte(DateOnly fechaInicio, DateOnly fechaFin)
        {
            var reservaciones = _context.Reservaciones
                .Include(r => r.Client)
                .Include(r => r.User)
                .Where(r => r.Fecha >= fechaInicio && r.Fecha <= fechaFin)
                .OrderBy(r => r.Fecha)
                .ThenBy(r => r.HoraInicio)
                .ToList();

            var data = new ReservacionesModel
            {
                FechaInicio = fechaInicio,
                FechaFin = fechaFin,
                FechaGeneracion = DateTime.Now,
                Reservaciones = reservaciones
            };

            var document = new ReservacionesDocument(data);

            var pdf = document.GeneratePdf();

            return Results.File(
                pdf,
                "application/pdf",
                "ReporteReservaciones.pdf");
        }



    }


}
