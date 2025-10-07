using Microsoft.AspNetCore.Mvc;
using static Hotel_Management.Areas.Admin.Controllers.DashboardController;

namespace Hotel_Management.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DashboardController : Controller
    {
        public DashboardController() 
        {
                   
        }
        public class CheckInOutInfo
        {
            public string Room { get; set; }
            public string Customer { get; set; }
            public string CheckTime { get; set; }
            public string Status { get; set; } // "Đã check-in" hoặc "Chờ check-in"
        }
        public void TemporaryData()
        {
            //Ghi chú những giá trị này khi liên kết db cần chỉnh sửa lại ở index
            // Phần biểu đồ Fill rate
            ViewBag.TotalRoom = 150;
            ViewBag.CurrentlyRenting = 110;
            ViewBag.MoreThanYesterday = 10;
            ViewBag.AvailableRooms = (int)ViewBag.TotalRoom - (int)ViewBag.CurrentlyRenting;
            ViewBag.RevenueToday = 50.8;

            // Phần biểu đồ Doanh thu ngày
            ViewBag.RevenuePerDay = new List<double> { 38.5, 42.3, 39.8, 45.2, 67.6, 72.1, 65.2 };

            // Phần biểu đồ doanh thu tháng
            ViewBag.RevenueMonthly = new List<double> { 1.8, 2.6, 0.8, 3.6, 3.8, 2.2 };

            // Phần check-in 
            ViewBag.TotalCheckIn = 15;
            ViewBag.CheckInList = new List<CheckInOutInfo>
            {
                new CheckInOutInfo { Room = "101", Customer = "Trần Duy Hưng", CheckTime = "14:20", Status = "Đã check-in"},
                new CheckInOutInfo { Room = "201", Customer = "Nguyễn Văn A", CheckTime = "14:00", Status = "Đã check-in" },
                new CheckInOutInfo { Room = "305", Customer = "Trần Thị B", CheckTime = "14:30", Status = "Đã check-in" },
                new CheckInOutInfo { Room = "412", Customer = "Lê Văn C", CheckTime = "15:00", Status = "Chờ check-in" },
                new CheckInOutInfo { Room = "508", Customer = "Phạm Thị D", CheckTime = "16:00", Status = "Chờ check-in" },
                new CheckInOutInfo { Room = "615", Customer = "Hoàng Văn E", CheckTime = "16:30", Status = "Chờ check-in" },
                new CheckInOutInfo { Room = "701", Customer = "Trịnh Thị F", CheckTime = "17:00", Status = "Đã check-in" }
            };

            // Phần check-out 
            ViewBag.CheckOutList = new List<CheckInOutInfo>
            {
                new CheckInOutInfo { Room = "103", Customer = "Vũ Thị F", CheckTime = "12:00", Status = "Đã check-out" },
                new CheckInOutInfo { Room = "207", Customer = "Đặng Văn G", CheckTime = "12:00", Status = "Đã check-out" },
                new CheckInOutInfo { Room = "314", Customer = "Bùi Thị H", CheckTime = "12:00", Status = "Đang dọn phòng" },
                new CheckInOutInfo { Room = "421", Customer = "Ngô Văn I", CheckTime = "12:00", Status = "Đang dọn phòng" },
                new CheckInOutInfo { Room = "529", Customer = "Đinh Thị K", CheckTime = "12:00", Status = "Chưa check-out" },
                new CheckInOutInfo { Room = "603", Customer = "Phan Văn L", CheckTime = "13:00", Status = "Đã check-out" },
            };
            ViewBag.TotalCheckOut = ((List<CheckInOutInfo>)ViewBag.CheckOutList).Count;
        }
        public IActionResult Index()
        {
            TemporaryData();
            return View();
        }
    }
}
