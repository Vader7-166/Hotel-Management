using Microsoft.AspNetCore.Mvc;

namespace BTL_Hotel_Management.Controllers.Customers
{
    public class ContactController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
