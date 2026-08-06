using GolTime.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GolTime.Controllers
{
    public class ReservacionController : Controller
    {
        private readonly GoltimeContext _context;

        public ReservacionController(GoltimeContext context)
        {
            _context = context;
        }

    
        public async Task<IActionResult> Index()
        {
            var reservaciones = await _context.Reservaciones
                .Include(r => r.Client)
                .Include(r => r.User)
                .OrderByDescending(r => r.Fecha)
                .ThenBy(r => r.HoraInicio)
                .ToListAsync();

            return View(reservaciones);
        }

        public async Task<IActionResult> Crear()
        {
            await CargarListas();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(
            [Bind("Fecha,HoraInicio,HoraFin,EstadoReserva,EstadoPago,ClientId,UserId")] Reservacion reservacion)
        {
            if (ModelState.IsValid)
            {
                reservacion.CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
                reservacion.UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

                _context.Add(reservacion);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            await CargarListas();
            return View(reservacion);
        }

        public async Task<IActionResult> Editar(int id)
        {
            var reservacion = await _context.Reservaciones.FindAsync(id);
            if (reservacion == null)
            {
                return NotFound();
            }

            await CargarListas();
            return View(reservacion);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(
            int id,
            [Bind("Id,Fecha,HoraInicio,HoraFin,EstadoReserva,EstadoPago,ClientId,UserId")] Reservacion form)
        {
            if (id != form.Id)
            {
                return NotFound();
            }

            var reservacion = await _context.Reservaciones.FindAsync(id);
            if (reservacion == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                reservacion.Fecha = form.Fecha;
                reservacion.HoraInicio = form.HoraInicio;
                reservacion.HoraFin = form.HoraFin;
                reservacion.EstadoReserva = form.EstadoReserva;
                reservacion.EstadoPago = form.EstadoPago;
                reservacion.ClientId = form.ClientId;
                reservacion.UserId = form.UserId;
                reservacion.UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            await CargarListas();
            return View(form);
        }

        [HttpGet]
        public async Task<IActionResult> Eliminar(int id)
        {
            var reservacion = await _context.Reservaciones
                .Include(r => r.Client)
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reservacion == null)
            {
                return NotFound();
            }

            return View(reservacion);
        }

        [HttpPost]
        [ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            var reservacion = await _context.Reservaciones.FindAsync(id);

            if (reservacion != null)
            {
                _context.Reservaciones.Remove(reservacion);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task CargarListas()
        {
            ViewBag.Clientes = await _context.Clientes
                .OrderBy(c => c.Nombre)
                .ToListAsync();

            ViewBag.Usuarios = await _context.Users
                .OrderBy(u => u.Nombre)
                .ToListAsync();
        }
    }
}