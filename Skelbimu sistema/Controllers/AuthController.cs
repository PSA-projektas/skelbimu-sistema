using Microsoft.AspNetCore.Mvc;

namespace Skelbimu_sistema.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Registration()
        {
            return View();
        }
    }
}
