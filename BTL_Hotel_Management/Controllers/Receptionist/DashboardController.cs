using Microsoft.AspNetCore.Mvc;

namespace BTL_Hotel_Management.Controllers.Receptionist
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
