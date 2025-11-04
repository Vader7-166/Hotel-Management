using Hotel_Management.Filters;
using Hotel_Management.Models;
using Microsoft.AspNetCore.Mvc;

namespace Hotel_Management.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Route("Customer/[controller]/[action]")]
    [CustomerAuthorize]
    public class ProfileController : Controller
    {
        private readonly HotelManagementContext _context;

        public ProfileController(HotelManagementContext context)
        {
            _context = context;
        }

        [Route("Customer/Profile/Index")]
        public IActionResult Index()
        {
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var customer = _context.Customers.FirstOrDefault(c => c.CustomerId == customerId);
            if (customer == null)
            {
                ViewBag.Error = "Không tìm thấy thông tin người dùng.";
                return View();
            }

            ViewBag.FullName = customer.FullName;
            ViewBag.Email = customer.Email;
            ViewBag.Phone = customer.Phone;
            ViewBag.Address = customer.Address;
            ViewBag.Username = HttpContext.Session.GetString("Username");

            return View();
        }

        // ✅ Edit - GET
        [HttpGet]
       
        public IActionResult Edit()
        {
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var customer = _context.Customers.FirstOrDefault(c => c.CustomerId == customerId);
            if (customer == null)
            {
                ViewBag.Error = "Do not see any information";
                return NotFound();
            }

            return View(customer);
        }

        // ✅ Edit - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Hotel_Management.Models.Customer model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList();
                TempData["Error"] = string.Join("; ", errors);
                return View(model);
            }

            var customer = _context.Customers.FirstOrDefault(c => c.CustomerId == model.CustomerId);
            if (customer == null)
            {
                TempData["Error"] = "Customer not found.";
                return NotFound();
            }

            customer.FullName = model.FullName;
            customer.Email = model.Email;
            customer.Phone = model.Phone;
            customer.Address = model.Address;
            customer.UpdatedAt = DateTime.Now;

            _context.Update(customer);
            _context.SaveChanges();

            TempData["Success"] = "Profile updated successfully.";
            return RedirectToAction("Index");
        }

    }
}
