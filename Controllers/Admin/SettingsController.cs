using Microsoft.AspNetCore.Mvc;

namespace Hotel_Management.Controllers.Admin
{
    public class SettingsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
