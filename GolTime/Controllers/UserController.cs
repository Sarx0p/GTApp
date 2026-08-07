using GolTime.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace GolTime.Controllers
{
    public class UserController : Controller
    {
        private readonly GoltimeContext _context;

        public UserController(GoltimeContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? rolId, string? estado, string? busqueda)
        {
            var usuarios = _context.Users
                .Include(u => u.Rol)
                .AsQueryable();

            if (rolId.HasValue && rolId.Value > 0)
            {
                usuarios = usuarios.Where(u => u.RolId == rolId.Value);
            }

            if (!string.IsNullOrWhiteSpace(estado))
            {
                if (estado == "activo")
                {
                    usuarios = usuarios.Where(u => u.Estado);
                }
                else if (estado == "inactivo")
                {
                    usuarios = usuarios.Where(u => !u.Estado);
                }
            }

            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                var filtro = busqueda.Trim().ToLower();
                usuarios = usuarios.Where(u =>
                    (u.Nombre ?? "").ToLower().Contains(filtro) ||
                    (u.Correo ?? "").ToLower().Contains(filtro));
            }

            await CargarRoles();
            ViewBag.RolSeleccionado = rolId;
            ViewBag.EstadoSeleccionado = estado;
            ViewBag.Busqueda = busqueda;

            return View(await usuarios
                .OrderBy(u => u.Nombre)
                .ToListAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(string nombre, string correo, string clave, int rolId, bool estado = true)
        {
            if (string.IsNullOrWhiteSpace(nombre) ||
                string.IsNullOrWhiteSpace(correo) ||
                string.IsNullOrWhiteSpace(clave) ||
                rolId <= 0)
            {
                TempData["Error"] = "Completa todos los campos obligatorios.";
                return RedirectToAction(nameof(Index));
            }

            var existeCorreo = await _context.Users.AnyAsync(u => u.Correo == correo.Trim());
            if (existeCorreo)
            {
                TempData["Error"] = "Ya existe un usuario con ese correo.";
                return RedirectToAction(nameof(Index));
            }

            var ahora = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

            var usuario = new User
            {
                Nombre = nombre.Trim(),
                Correo = correo.Trim(),
                Clave = EncriptarClave(clave),
                Estado = estado,
                RolId = rolId,
                CreatedAt = ahora,
                UpdatedAt = ahora
            };

            _context.Users.Add(usuario);
            await _context.SaveChangesAsync();

            TempData["Exito"] = "Usuario creado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, string nombre, string correo, string? clave, int rolId, bool estado)
        {
            var usuario = await _context.Users.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            if (string.IsNullOrWhiteSpace(nombre) ||
                string.IsNullOrWhiteSpace(correo) ||
                rolId <= 0)
            {
                TempData["Error"] = "Completa todos los campos obligatorios.";
                return RedirectToAction(nameof(Index));
            }

            var correoLimpio = correo.Trim();
            var existeCorreo = await _context.Users
                .AnyAsync(u => u.Id != id && u.Correo == correoLimpio);

            if (existeCorreo)
            {
                TempData["Error"] = "Ya existe otro usuario con ese correo.";
                return RedirectToAction(nameof(Index));
            }

            usuario.Nombre = nombre.Trim();
            usuario.Correo = correoLimpio;
            usuario.RolId = rolId;
            usuario.Estado = estado;
            usuario.UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

            if (!string.IsNullOrWhiteSpace(clave))
            {
                usuario.Clave = EncriptarClave(clave);
            }

            await _context.SaveChangesAsync();

            TempData["Exito"] = "Usuario actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id)
        {
            var usuario = await _context.Users
                .Include(u => u.Reservaciones)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (usuario == null)
            {
                return NotFound();
            }

            if (usuario.Reservaciones.Any())
            {
                usuario.Estado = false;
                usuario.UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
                TempData["Exito"] = "El usuario tiene reservaciones, por eso fue desactivado.";
            }
            else
            {
                _context.Users.Remove(usuario);
                TempData["Exito"] = "Usuario eliminado correctamente.";
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private async Task CargarRoles()
        {
            ViewBag.Roles = await _context.Roles
                .OrderBy(r => r.Nombre)
                .ToListAsync();
        }

        private static string EncriptarClave(string clave)
        {
            using var sha256 = SHA256.Create();
            var datos = Encoding.UTF8.GetBytes(clave);
            var hash = sha256.ComputeHash(datos);

            return BitConverter
                .ToString(hash)
                .Replace("-", "")
                .ToLower();
        }
    }
}
