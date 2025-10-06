using Microsoft.AspNetCore.Mvc;

namespace Hotel_Management.Controllers.Customer
{
    public class ContactController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
