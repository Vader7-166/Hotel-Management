using Hotel_Management.Areas.Customer.Models;
using Microsoft.AspNetCore.Mvc;

namespace Hotel_Management.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class RoomsController : Controller
    {
        [Route("Customer/Room")]

        public IActionResult Index()
        {
            var roomTypes = new List<RoomTypeViewModel>
           {
            new RoomTypeViewModel
            {
                Id = 1,
                TypeName = "Loft",
                Description = "Phòng Loft là loại phòng có thiết kế hai tầng (duplex), tái hiện sống động hình ảnh những ngôi nhà truyền thống.",
               ImageUrl = "/images/room/room1.jpg"
            },
            new RoomTypeViewModel
            {
                Id = 2,
                TypeName = "Superior",
                Description = "Một lựa chọn thông minh với thiết kế gọn gàng, tinh tế và tối giản, mang vẻ đẹp thanh lịch, hoàn hảo.",
                ImageUrl = "/images/room/room1.jpg"
            },
            new RoomTypeViewModel
            {
                Id = 3,
                TypeName = "Deluxe",
                Description = "Những căn phòng rộng rãi với ban công thoáng mát được thiết kế để khơi dậy và tái tạo trọn vẹn.",
                ImageUrl ="/images/room/room3.jpg"
            },
            new RoomTypeViewModel { 
                Id = 4,
                TypeName = "Premier", Description = "Không gian yên bình được thiết kế mở với ban công rộng, nơi bạn có thể hòa mình vào thiên nhiên trong lành, là lựa chọn lý tưởng để tái tạo năng lượng và tìm lại sự thư thái tuyệt đối.",
                ImageUrl = "/images/room/room4.jpg" 
            },
            new RoomTypeViewModel { 
                Id = 5, 
                TypeName = "Suite", 
                Description = "Một không gian ấm cúng và đầy cảm hứng, nổi bật với nội thất trang nhã và tông màu ấm áp. Đây là nơi hoàn hảo để tận hưởng những khoảnh khắc riêng tư, lãng mạn và tạo nên những kỷ niệm đáng nhớ", 
                ImageUrl = "/images/room/room5.jpg" 
            },
            new RoomTypeViewModel { 
                Id = 6,
                TypeName = "Presidential", 
                Description = "Sự kết hợp hoàn hảo giữa không gian sang trọng và những tiện nghi hiện đại, đẳng cấp. Mỗi chi tiết đều được chăm chút tinh xảo, mang đến một trải nghiệm nghỉ dưỡng đỉnh cao và khẳng định phong cách của bạn.", 
                ImageUrl = "/images/room/room6.jpg" 
            },
             new RoomTypeViewModel {
                Id = 7,
                TypeName = "Executive",
                Description = "Không gian kết nối rộng rãi và đầy đủ tiện nghi, là sự lựa chọn hoàn hảo để gia đình hoặc nhóm bạn cùng nhau chia sẻ kỳ nghỉ đáng nhớ.",
                ImageUrl = "/images/room/room7.jpg"
            },
              new RoomTypeViewModel {
                Id = 8,
                TypeName = "Panoramic",
                Description = "Thiết kế tinh tế với khu vực làm việc yên tĩnh và tiện nghi cao cấp, giúp bạn vừa tập trung hiệu quả vừa thư giãn nạp lại năng lượng.",
                ImageUrl = "/images/room/room8.jpg"
            },
               new RoomTypeViewModel {
                Id = 9,
                TypeName = "Junior",
                Description = "Tận hưởng tầm nhìn bao quát toàn cảnh từ không gian nghỉ dưỡng riêng tư và độc đáo, nơi vẻ đẹp của cảnh quan và sự tinh tế trong thiết kế hòa quyện làm một.",
                ImageUrl = "/images/room/room9.jpg"
             }
        };

            return View(roomTypes);
        }
    }
}
