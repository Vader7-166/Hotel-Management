using Hotel_Management.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Hotel_Management.Areas.Customer.Controllers
{
    [Area("Customer")]
   
    public class AboutController : Controller
    {
        [Route("Customer/About/Index")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
