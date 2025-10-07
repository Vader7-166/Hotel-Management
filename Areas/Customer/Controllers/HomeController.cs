using Microsoft.AspNetCore.Mvc;

//Trang chủ
namespace Hotel_Management.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        [Route("Customer/Index")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
