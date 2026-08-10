using GolTime.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace GolTime.Controllers
{
    public class AccesoController : Controller
    {
        private readonly GoltimeContext _context;

        public AccesoController(GoltimeContext context)
        {
            _context = context;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(InfoLogin infoLogin)
        {
            if (infoLogin != null)
            {
                SHA256 sha256 = SHA256.Create();

                byte[] datos = Encoding.UTF8.GetBytes(infoLogin.Password ?? "");

                byte[] hash = sha256.ComputeHash(datos);

                string claveEncriptada = BitConverter
                    .ToString(hash)
                    .Replace("-", "")
                    .ToLower();

                var usuario = _context.Users
                    .Include(u => u.Rol)
                    .Where(u => u.Correo == infoLogin.Login &&
                                u.Clave == claveEncriptada &&
                                u.Estado)
                    .FirstOrDefault();

                if (usuario != null)
                {
                    var claims = new List<Claim>()
                    {
                        new Claim(ClaimTypes.Name, usuario.Nombre ?? ""),
                        new Claim(ClaimTypes.Email, usuario.Correo ?? ""),
                        new Claim(ClaimTypes.Role, usuario.Rol?.Nombre ?? "")
                    };

                    var claimsIdentity = new ClaimsIdentity(
                        claims,
                        CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity));

                    return RedirectToAction("Index", "Reservacion");
                }
                else
                {
                    ViewBag.Error = "Correo o contraseña incorrectos.";
                    return View(infoLogin);
                }
            }

            return View(infoLogin);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login", "Acceso");
        }

        public IActionResult AccesoDenegado()
        {
            return View();
        }
    }
}
