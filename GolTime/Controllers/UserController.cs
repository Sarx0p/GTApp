using Microsoft.AspNetCore.Mvc;

namespace GolTime.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
