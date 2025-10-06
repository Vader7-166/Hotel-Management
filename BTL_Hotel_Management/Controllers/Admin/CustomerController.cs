using Microsoft.AspNetCore.Mvc;

namespace BTL_Hotel_Management.Controllers.Admin
{
    public class CustomerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
