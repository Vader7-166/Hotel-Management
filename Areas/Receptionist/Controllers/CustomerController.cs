using Microsoft.AspNetCore.Mvc;

namespace Hotel_Management.Areas.Receptionist.Controllers
{
    public class CustomerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
