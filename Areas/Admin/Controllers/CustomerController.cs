using Hotel_Management.Models.Customers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
namespace Hotel_Management.Areas.Admin.Controllers
{

    [Area("Admin")]
    public class CustomerController : Controller
    {

        private int pageSize = 2; //số lượng đối tượng được hiển thị ở mỗi trang
        private List<ModelCustomer> listCustomer = new List<ModelCustomer>();

        public CustomerController()
        {
            listCustomer.Add(new ModelCustomer
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
            });
            listCustomer.Add(new ModelCustomer
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
            });
            listCustomer.Add(new ModelCustomer
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
            });
            listCustomer.Add(new ModelCustomer
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
            });
            listCustomer.Add(new ModelCustomer
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
            });
        }
        [Route("Admin/Customer")]
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult CustomerList()
        {
           

            //tính toán số trang
            int pageNum = (int)Math.Ceiling((float)listCustomer.Count() / (float)pageSize); //tính số trang
            var result = listCustomer.Take(pageSize).ToList(); //result là danh sách với list đã được thu gọn
            ViewBag.pageNum = pageNum;
            ViewBag.currentPage = 1;
            return View(result);
        }

        [HttpGet]
        public IActionResult AddNewCustomer()
        {

            return View();
        }

        [HttpPost]
        public IActionResult AddNewCustomer(ModelCustomer md)
        {
            listCustomer.Add(md);
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

        public IActionResult CustomerTableAndPagination(int? pageIndex)
        {
            // Tính toán phân trang
            int page = (int)(pageIndex == null || pageIndex <= 0 ? 1 : pageIndex);
            int pageNum = (int)Math.Ceiling((float)listCustomer.Count() / (float)pageSize);

            ViewBag.pageNum = pageNum;
            ViewBag.currentPage = page;

            // Lấy dữ liệu theo trang
            var result = listCustomer
                .Skip(pageSize * (page - 1))
                .Take(pageSize)
                .ToList();

            // Trả về Partial View
            return PartialView("_CustomerListPartial", result);
        }

        public IActionResult CustomerFilter(string? searchInput, string? status, string? type)
        {
            return PartialView("_CustomerListPartial");
        }

    }
}
