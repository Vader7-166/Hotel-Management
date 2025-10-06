using Microsoft.AspNetCore.Mvc;
//đăng ký đăng nhập
namespace Hotel_Management.Controllers.Customer
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
