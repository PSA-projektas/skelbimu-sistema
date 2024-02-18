using Microsoft.AspNetCore.Mvc;

namespace Skelbimu_sistema.Controllers
{
    public class AutentifikacijaController : Controller
    {
        public IActionResult Registracija()
        {
            return View();
        }
    }
}
