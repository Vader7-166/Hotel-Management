using Microsoft.AspNetCore.Mvc;

namespace BTL_Hotel_Management.Controllers.Customers
{
    public class HomeController : Controller
    {
        [Area("Customer")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
