using Hotel_Management.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Hotel_Management.Areas.Customer.Controllers
{
    [Area("Customer")]
   
    public class ContactController : Controller
    {
        [Route("Customer/Contact/Index")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
