using GolTime.Models;
using Microsoft.AspNetCore.Mvc;

namespace GolTime.Controllers
{
    public class AccesoController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View(new InfoLogin());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(InfoLogin infoLogin)
        {
            return View(infoLogin);
        }
    }
}
