using Microsoft.AspNetCore.Mvc;

namespace Hotel_Management.Controllers.Customer
{
    public class AboutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
