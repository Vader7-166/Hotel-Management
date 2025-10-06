using Microsoft.AspNetCore.Mvc;

namespace Hotel_Management.Areas.Admin.Controllers
{
    public class AdminHomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
