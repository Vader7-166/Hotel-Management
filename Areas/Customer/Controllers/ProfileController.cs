using Microsoft.AspNetCore.Mvc;

namespace Hotel_Management.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ProfileController : Controller
    {
        [Route("Customer/Profile/Index")]
        public IActionResult Index()
        {
            return View();
        }
        [Route("Customer/Profile/Edit")]
        public IActionResult Edit()
        {
            return View();
        }
    }
}
