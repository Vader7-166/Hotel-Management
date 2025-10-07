using Microsoft.AspNetCore.Mvc;

namespace Hotel_Management.Areas.Admin.Controllers
{
    public class CustomerController : Controller
    {
        [Area("Admin")]
        [Route("Admin/Customer")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
