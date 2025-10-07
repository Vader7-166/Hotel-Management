using Hotel_Management.Areas.Customer.Models;
using Microsoft.AspNetCore.Mvc;

namespace Hotel_Management.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class RoomsController : Controller
    {
<<<<<<< Updated upstream
        [Route("Customer/Rooms/Index")]
=======
        [Route("Customer/Room")]
>>>>>>> Stashed changes
        public IActionResult Index()
        {
            var roomTypes = new List<RoomTypeViewModel>
           {
            new RoomTypeViewModel
            {
                Id = 1,
                TypeName = "Loft",
                Description = "Phòng Loft là loại phòng có thiết kế hai tầng (duplex), tái hiện sống động hình ảnh những ngôi nhà truyền thống.",
                ImageUrl = "/images/rooms/loft.jpg" 
            },
            new RoomTypeViewModel
            {
                Id = 2,
                TypeName = "Superior",
                Description = "Một lựa chọn thông minh với thiết kế gọn gàng, tinh tế và tối giản, mang vẻ đẹp thanh lịch, hoàn hảo.",
                ImageUrl = "/images/rooms/superior.jpg"
            },
            new RoomTypeViewModel
            {
                Id = 3,
                TypeName = "Deluxe",
                Description = "Những căn phòng rộng rãi với ban công thoáng mát được thiết kế để khơi dậy và tái tạo trọn vẹn.",
                ImageUrl = "/images/rooms/deluxe.jpg"
            },
            new RoomTypeViewModel { Id = 4, TypeName = "Premier", Description = "Mô tả cho phòng Premier.", ImageUrl = "/images/rooms/premier.jpg" },
            new RoomTypeViewModel { Id = 5, TypeName = "Suite", Description = "Mô tả cho phòng Suite.", ImageUrl = "/images/rooms/suite.jpg" },
            new RoomTypeViewModel { Id = 6, TypeName = "Presidential", Description = "Mô tả cho phòng Presidential.", ImageUrl = "/images/rooms/presidential.jpg" }
        };

            return View(roomTypes);
        }
    }
}
