using Microsoft.AspNetCore.Mvc;

namespace GolTime.Controllers
{
    public class ReporteController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
