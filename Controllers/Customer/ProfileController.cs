using Microsoft.AspNetCore.Mvc;

namespace Hotel_Management.Controllers.Customer
{
    public class ProfileController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
