using Microsoft.AspNetCore.Mvc;

namespace Hotel_Management.Controllers.Admin
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
