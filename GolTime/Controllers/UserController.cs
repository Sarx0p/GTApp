using GolTime.Models;
using GolTime.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace GolTime.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class UserController : Controller
    {
        private const string CorreoAdminPrincipal = "admin@goltime.com";
        private readonly GoltimeContext _context;

        public UserController(GoltimeContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int pg = 1, int? rolId = null, string? estado = null, string? busqueda = null)
        {
            var data = await PrepararIndex(pg, rolId, estado, busqueda);
            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> Crear(int pg = 1, int? rolId = null, string? estado = null, string? busqueda = null)
        {
            var data = await PrepararIndex(pg, rolId, estado, busqueda);

            ViewBag.ModalUsuario = "Crear";
            ViewBag.UsuarioFormulario = new User { Estado = true };

            return View("Index", data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear([Bind("Nombre,Correo,Clave,Estado,RolId")] User usuario)
        {
            if (!ModelState.IsValid)
            {
                var data = await PrepararIndex();
                ViewBag.ModalUsuario = "Crear";
                ViewBag.UsuarioFormulario = usuario;
                return View("Index", data);
            }

            var correoLimpio = (usuario.Correo ?? string.Empty).Trim().ToLower();
            var existeCorreo = await _context.Users.AnyAsync(u => (u.Correo ?? "").ToLower() == correoLimpio);

            if (existeCorreo)
            {
                ModelState.AddModelError("Correo", "Ya existe un usuario con ese correo.");

                var data = await PrepararIndex();
                ViewBag.ModalUsuario = "Crear";
                ViewBag.UsuarioFormulario = usuario;
                return View("Index", data);
            }

            var ahora = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

            usuario.Nombre = usuario.Nombre?.Trim();
            usuario.Correo = correoLimpio;
            usuario.Clave = EncriptarClave(usuario.Clave ?? string.Empty);
            usuario.CreatedAt = ahora;
            usuario.UpdatedAt = ahora;

            _context.Users.Add(usuario);
            await _context.SaveChangesAsync();

            TempData["Exito"] = "Usuario creado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id, int pg = 1, int? rolId = null, string? estado = null, string? busqueda = null)
        {
            var usuario = await _context.Users.FindAsync(id);

            if (usuario == null)
            {
                return NotFound();
            }

            var data = await PrepararIndex(pg, rolId, estado, busqueda);

            ViewBag.ModalUsuario = "Editar";
            ViewBag.UsuarioFormulario = usuario;
            ViewBag.EsAdminPrincipal = EsAdminPrincipal(usuario);
            ViewBag.EsUsuarioActual = EsUsuarioActual(usuario);

            return View("Index", data);
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

            if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(correo) || rolId <= 0)
            {
                ModelState.AddModelError(string.Empty, "Completa todos los campos obligatorios.");

                var data = await PrepararIndex();
                ViewBag.ModalUsuario = "Editar";
                ViewBag.UsuarioFormulario = usuario;
                ViewBag.EsAdminPrincipal = EsAdminPrincipal(usuario);
                ViewBag.EsUsuarioActual = EsUsuarioActual(usuario);

                return View("Index", data);
            }

            if (!estado && EsUsuarioActual(usuario))
            {
                TempData["Error"] = "No puedes desactivar la cuenta con la que iniciaste sesion.";
                return RedirectToAction(nameof(Index));
            }

            if (!estado && EsAdminPrincipal(usuario))
            {
                TempData["Error"] = "No puedes desactivar la cuenta principal admin@goltime.com.";
                return RedirectToAction(nameof(Index));
            }

            var correoLimpio = correo.Trim().ToLower();
            var existeCorreo = await _context.Users
                .AnyAsync(u => u.Id != id && (u.Correo ?? "").ToLower() == correoLimpio);

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

        [HttpGet]
        public async Task<IActionResult> Eliminar(int id, int pg = 1, int? rolId = null, string? estado = null, string? busqueda = null)
        {
            var usuario = await _context.Users
                .Include(u => u.Rol)
                .Include(u => u.Reservaciones)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (usuario == null)
            {
                return NotFound();
            }

            var data = await PrepararIndex(pg, rolId, estado, busqueda);

            ViewBag.ModalUsuario = "Eliminar";
            ViewBag.UsuarioFormulario = usuario;
            ViewBag.NoPuedeEliminar = ObtenerMotivoBloqueoEliminacion(usuario);

            return View("Index", data);
        }

        [HttpPost]
        [ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            var usuario = await _context.Users
                .Include(u => u.Reservaciones)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (usuario == null)
            {
                return NotFound();
            }

            var motivoBloqueo = ObtenerMotivoBloqueoEliminacion(usuario);

            if (!string.IsNullOrWhiteSpace(motivoBloqueo))
            {
                TempData["Error"] = motivoBloqueo;
                return RedirectToAction(nameof(Index));
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

        private async Task<List<User>> PrepararIndex(int pg = 1, int? rolId = null, string? estado = null, string? busqueda = null)
        {
            var usuarios = FiltrarUsuarios(rolId, estado, busqueda);
            var totalRegistros = await usuarios.CountAsync();
            var paginacion = new UserPaginacion(totalRegistros, pg, 5, "User", "Index", rolId, estado, busqueda);

            var data = await usuarios
                .OrderBy(u => u.Id == 1 ? 0 : 1)
                .ThenBy(u => u.Nombre)
                .Skip(paginacion.Salto)
                .Take(paginacion.RegistrosPagina)
                .ToListAsync();

            await CargarRoles();

            ViewBag.Paginacion = paginacion;
            ViewBag.TotalRegistros = totalRegistros;
            ViewBag.RolSeleccionado = rolId;
            ViewBag.EstadoSeleccionado = estado;
            ViewBag.Busqueda = busqueda;
            ViewBag.CorreoUsuarioActual = ObtenerCorreoUsuarioActual();
            ViewBag.CorreoAdminPrincipal = CorreoAdminPrincipal;

            return data;
        }

        private IQueryable<User> FiltrarUsuarios(int? rolId, string? estado, string? busqueda)
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

            return usuarios;
        }

        private async Task CargarRoles()
        {
            ViewBag.Roles = await _context.Roles
                .OrderBy(r => r.Nombre)
                .ToListAsync();
        }

        private string? ObtenerCorreoUsuarioActual()
        {
            return User.FindFirstValue(ClaimTypes.Email)?.Trim().ToLower();
        }

        private bool EsUsuarioActual(User usuario)
        {
            var correoActual = ObtenerCorreoUsuarioActual();

            return !string.IsNullOrWhiteSpace(correoActual) &&
                string.Equals(usuario.Correo?.Trim(), correoActual, StringComparison.OrdinalIgnoreCase);
        }

        private static bool EsAdminPrincipal(User usuario)
        {
            return usuario.Id == 1 ||
                string.Equals(usuario.Correo?.Trim(), CorreoAdminPrincipal, StringComparison.OrdinalIgnoreCase);
        }

        private string? ObtenerMotivoBloqueoEliminacion(User usuario)
        {
            if (EsUsuarioActual(usuario))
            {
                return "No puedes eliminar o desactivar la cuenta con la que iniciaste sesion.";
            }

            if (EsAdminPrincipal(usuario))
            {
                return "No se puede eliminar ni desactivar la cuenta principal admin@goltime.com.";
            }

            return null;
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