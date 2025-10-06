using Microsoft.AspNetCore.Mvc;

namespace Hotel_Management.Controllers.Receptionist
{
    public class CustomerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
