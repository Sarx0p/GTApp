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

        // GET: /Reservacion?estado=Activa&pago=Pagado
        public async Task<IActionResult> Index(string? estado, string? pago)
        {
            var query = _context.Reservaciones
                .Include(r => r.Client)
                .Include(r => r.User)
                .AsQueryable();

            if (!string.IsNullOrEmpty(estado) &&
                Enum.TryParse<GolTime.Enums.EstadoReservaEnum>(estado, true, out var estadoEnum))
            {
                query = query.Where(r => r.EstadoReserva == estadoEnum);
            }

            if (!string.IsNullOrEmpty(pago) &&
                Enum.TryParse<GolTime.Enums.EstadoPagoEnum>(pago, true, out var pagoEnum))
            {
                query = query.Where(r => r.EstadoPago == pagoEnum);
            }

            var reservaciones = await query
                .OrderByDescending(r => r.Fecha)
                .ThenBy(r => r.HoraInicio)
                .ToListAsync();

            ViewBag.EstadoActual = estado;
            ViewBag.PagoActual = pago;

            return View(reservaciones);
        }

        // GET: /Reservacion/BuscarClientesAjax?term=xxx
        // Búsqueda en vivo contra la base de datos para el buscador de cliente
        [HttpGet]
        public async Task<IActionResult> BuscarClientesAjax(string? term)
        {
            term = (term ?? string.Empty).Trim().ToLower();

            var clientes = await _context.Clientes
                .Where(c => string.IsNullOrEmpty(term) || c.Nombre.ToLower().Contains(term))
                .OrderBy(c => c.Nombre)
                .Take(15)
                .Select(c => new { id = c.Id, nombre = c.Nombre, numero = c.Numero })
                .ToListAsync();

            return Json(clientes);
        }

        // GET: /Reservacion/Crear
        public IActionResult Crear()
        {
            return View();
        }

        // POST: /Reservacion/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(
            [Bind("Fecha,HoraInicio,HoraFin,EstadoReserva,EstadoPago,ClientId")] Reservacion reservacion)
        {
            if (ModelState.IsValid)
            {
                if (reservacion.HoraFin <= reservacion.HoraInicio)
                {
                    ModelState.AddModelError(string.Empty, "La hora de fin debe ser posterior a la hora de inicio.");
                }
                else if (reservacion.HoraInicio < new TimeOnly(7, 0) || reservacion.HoraFin > new TimeOnly(20, 0))
                {
                    ModelState.AddModelError(string.Empty, "El horario de la cancha es de 7:00 a.m. a 8:00 p.m.");
                }
                else if (await ExisteConflictoHorario(reservacion.Fecha, reservacion.HoraInicio, reservacion.HoraFin, null))
                {
                    ModelState.AddModelError(string.Empty, "Ya existe una reserva activa para ese día y horario.");
                }
                else
                {
                    // TODO: reemplazar por el usuario de la sesión actual cuando exista login.
                    var usuarioActual = await _context.Users.OrderBy(u => u.Id).FirstOrDefaultAsync();
                    reservacion.UserId = usuarioActual?.Id ?? 0;

                    reservacion.CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
                    reservacion.UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

                    _context.Add(reservacion);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
            }

            await CargarListas();
            return View(reservacion);
        }

        // POST: /Reservacion/CrearClienteAjax
        // Crea un cliente al vuelo desde el mini-modal "Nuevo cliente" y lo devuelve como JSON
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearClienteAjax(string nombre, string numero)
        {
            if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(numero))
            {
                return BadRequest(new { error = "Nombre y teléfono son obligatorios." });
            }

            var cliente = new Cliente
            {
                Nombre = nombre.Trim(),
                Numero = numero.Trim(),
                CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
                UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
            };

            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();

            return Json(new { id = cliente.Id, nombre = cliente.Nombre, numero = cliente.Numero });
        }

        // GET: /Reservacion/Editar/5
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

        // POST: /Reservacion/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(
            int id,
            [Bind("Id,Fecha,HoraInicio,HoraFin,EstadoReserva,EstadoPago,ClientId")] Reservacion form)
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
                if (form.HoraFin <= form.HoraInicio)
                {
                    ModelState.AddModelError(string.Empty, "La hora de fin debe ser posterior a la hora de inicio.");
                }
                else if (form.HoraInicio < new TimeOnly(7, 0) || form.HoraFin > new TimeOnly(20, 0))
                {
                    ModelState.AddModelError(string.Empty, "El horario de la cancha es de 7:00 a.m. a 8:00 p.m.");
                }
                else if (await ExisteConflictoHorario(form.Fecha, form.HoraInicio, form.HoraFin, form.Id))
                {
                    ModelState.AddModelError(string.Empty, "Ya existe una reserva activa para ese día y horario.");
                }
                else
                {
                    reservacion.Fecha = form.Fecha;
                    reservacion.HoraInicio = form.HoraInicio;
                    reservacion.HoraFin = form.HoraFin;
                    reservacion.EstadoReserva = form.EstadoReserva;
                    reservacion.EstadoPago = form.EstadoPago;
                    reservacion.ClientId = form.ClientId;
                    reservacion.UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }

            await CargarListas();
            return View(form);
        }

        // GET: /Reservacion/Eliminar/5
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

        // POST: /Reservacion/Eliminar/5
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

        // Listas para los <select> de Cliente y Usuario en Crear/Editar
        private async Task CargarListas()
        {
            ViewBag.Clientes = await _context.Clientes
                .OrderBy(c => c.Nombre)
                .ToListAsync();

            ViewBag.Usuarios = await _context.Users
                .OrderBy(u => u.Nombre)
                .ToListAsync();
        }

        // Revisa si ya existe otra reserva que se cruce en fecha y horario.
        // idExcluir se usa en Editar para no comparar la reserva contra sí misma.
        private async Task<bool> ExisteConflictoHorario(DateOnly fecha, TimeOnly horaInicio, TimeOnly horaFin, int? idExcluir)
        {
            var query = _context.Reservaciones.Where(r =>
                r.Fecha == fecha &&
                r.EstadoReserva != GolTime.Enums.EstadoReservaEnum.Cancelada &&
                r.HoraInicio < horaFin &&
                r.HoraFin > horaInicio);

            if (idExcluir.HasValue)
            {
                query = query.Where(r => r.Id != idExcluir.Value);
            }

            return await query.AnyAsync();
        }
    }
}