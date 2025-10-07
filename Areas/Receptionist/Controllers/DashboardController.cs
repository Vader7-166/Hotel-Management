using Microsoft.AspNetCore.Mvc;

namespace Hotel_Management.Controllers.Receptionist
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
