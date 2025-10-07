using Microsoft.AspNetCore.Mvc;
//đăng ký đăng nhập
namespace Hotel_Management.Areas.Customer.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
