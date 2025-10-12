using Microsoft.AspNetCore.Mvc;
//đăng ký đăng nhập
namespace Hotel_Management.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class AccountController : Controller
    {
        [Route("Customer/Account/Index")]
        public IActionResult Index()
        {
            return View();
        }
        [Route("Customer/Account/Login")]
        public IActionResult Login()
        {
            return View();
        }
        [Route("Customer/Account/Register")]
        public IActionResult Register()
        {
            return View();
        }
    }
}
