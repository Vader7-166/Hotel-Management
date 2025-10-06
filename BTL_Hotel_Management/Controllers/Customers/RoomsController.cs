using Microsoft.AspNetCore.Mvc;

namespace BTL_Hotel_Management.Controllers.Customers
{
    public class RoomsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
