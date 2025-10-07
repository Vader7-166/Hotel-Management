using Microsoft.AspNetCore.Mvc;

namespace Hotel_Management.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CustomerController : Controller
    {
        [Route("Admin/Customer")]
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult CustomerList()
        {
            return View();
        }

        public IActionResult AddNewCustomer()
        {
            return View();
        }

        public IActionResult CustomerSegmentation()
        {
            return View();
        }

        public IActionResult CustomerReport() 
        {
            return View();
        }
    }
}
