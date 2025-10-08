using Hotel_Management.Models.Customer;
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
                // Đối tượng thứ nhất
                var listCustomer = new List<ModelCustomer>
                {
                // Đối tượng thứ nhất
                new ModelCustomer
                {
                    CustomerCode = "#CUS001",
                    CustomerName = "John Michael",
                    CustomerEmail = "john@example.com",
                    CustomerPhone = "+84 123 456 789",
                    CustomerType = "VIP",
                    CustomerStatus = "Active",
                    TotalBooking = 15,
                    LastVisit = new DateTime(2024, 10, 15),
                    AvatarUrl = "~/images/team-2.jpg"
                },

                // Đối tượng thứ hai
                new ModelCustomer
                {
                    CustomerCode = "#CUS002",
                    CustomerName = "Sarah Johnson",
                    CustomerEmail = "sarah@example.com",
                    CustomerPhone = "+84 987 654 321",
                    CustomerType = "Regular",
                    CustomerStatus = "Active",
                    TotalBooking = 8,
                    LastVisit = new DateTime(2024, 9, 20),
                    AvatarUrl = "~/images/team-3.jpg"
                },

                // Đối tượng thứ ba
                new ModelCustomer
                {
                    CustomerCode = "#CUS003",
                    CustomerName = "Michael Chen",
                    CustomerEmail = "michael@example.com",
                    CustomerPhone = "+84 456 789 123",
                    CustomerType = "New",
                    CustomerStatus = "Inactive",
                    TotalBooking = 2,
                    LastVisit = new DateTime(2024, 10, 5),
                    AvatarUrl = "~/images/team-4.jpg"
                }
            };
            return View(listCustomer);
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
