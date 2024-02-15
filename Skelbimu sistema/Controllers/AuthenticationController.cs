using Microsoft.AspNetCore.Mvc;

namespace Skelbimu_sistema.Controllers
{
    public class AuthenticationController : Controller
    {
        public IActionResult Registration()
        {
            return View();
        }
    }
}
