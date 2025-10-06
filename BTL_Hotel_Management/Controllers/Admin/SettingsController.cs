using Microsoft.AspNetCore.Mvc;

namespace BTL_Hotel_Management.Controllers.Admin
{
    public class SettingsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
