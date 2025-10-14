using Microsoft.AspNetCore.Mvc;
using Hotel_Management.Models;

namespace Hotel_Management.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class RoomsController : Controller
    {
        [Route("Customer/Room")]
        public IActionResult Index()
        {
            var roomTypes = GetRoomTypes();
            return View(roomTypes);
        }
        [HttpGet("Details/{id}")]
        public IActionResult Details(int id)
        {
            var roomTypes = GetRoomTypes();
            var roomType = roomTypes.FirstOrDefault(r => r.RoomTypeId == id);

            if (roomType == null)
            {
                return NotFound();
            }

            return View(roomType);
        }

        // Method để lấy danh sách room types (tái sử dụng)
        private List<RoomType> GetRoomTypes()
        {
            return new List<RoomType>
            {
                new RoomType
                {
                    RoomTypeId = 1,
                    TypeName = "Loft",
                    MaxOccupancy = 4,
                    Description = "Phòng Loft là loại phòng có thiết kế hai tầng (duplex), tái hiện sống động hình ảnh những ngôi nhà truyền thống.",
                    BasePrice = 2500000m,
                    CreatedAt = DateTime.Now.AddMonths(-6),
                    UpdatedAt = DateTime.Now.AddDays(-15)
                },
                new RoomType
                {
                    RoomTypeId = 2,
                    TypeName = "Superior",
                    MaxOccupancy = 2,
                    Description = "Một lựa chọn thông minh với thiết kế gọn gàng, tinh tế và tối giản, mang vẻ đẹp thanh lịch, hoàn hảo.",
                    BasePrice = 1500000m,
                    CreatedAt = DateTime.Now.AddMonths(-6),
                    UpdatedAt = DateTime.Now.AddDays(-10)
                },
                new RoomType
                {
                    RoomTypeId = 3,
                    TypeName = "Deluxe",
                    MaxOccupancy = 3,
                    Description = "Những căn phòng rộng rãi với ban công thoáng mát được thiết kế để khơi dậy và tái tạo trọn vẹn.",
                    BasePrice = 2000000m,
                    CreatedAt = DateTime.Now.AddMonths(-6),
                    UpdatedAt = DateTime.Now.AddDays(-8)
                },
                new RoomType
                {
                    RoomTypeId = 4,
                    TypeName = "Premier",
                    MaxOccupancy = 3,
                    Description = "Không gian yên bình được thiết kế mở với ban công rộng, nơi bạn có thể hòa mình vào thiên nhiên trong lành, là lựa chọn lý tưởng để tái tạo năng lượng và tìm lại sự thư thái tuyệt đối.",
                    BasePrice = 2800000m,
                    CreatedAt = DateTime.Now.AddMonths(-5),
                    UpdatedAt = DateTime.Now.AddDays(-5)
                },
                new RoomType
                {
                    RoomTypeId = 5,
                    TypeName = "Suite",
                    MaxOccupancy = 4,
                    Description = "Sự kết hợp hoàn hảo giữa không gian sang trọng và những tiện nghi hiện đại, đẳng cấp. Mỗi chi tiết đều được chăm chút tinh xảo, mang đến một trải nghiệm nghỉ dưỡng đỉnh cao và khẳng định phong cách của bạn.",
                    BasePrice = 3500000m,
                    CreatedAt = DateTime.Now.AddMonths(-5),
                    UpdatedAt = DateTime.Now.AddDays(-3)
                },
                new RoomType
                {
                    RoomTypeId = 6,
                    TypeName = "Presidential",
                    MaxOccupancy = 6,
                    Description = "Một không gian ấm cúng và đầy cảm hứng, nổi bật với nội thất trang nhã và tông màu ấm áp. Đây là nơi hoàn hảo để tận hưởng những khoảnh khắc riêng tư, lãng mạn và tạo nên những kỷ niệm đáng nhớ.",
                    BasePrice = 8000000m,
                    CreatedAt = DateTime.Now.AddMonths(-4),
                    UpdatedAt = DateTime.Now.AddDays(-2)
                },
                new RoomType
                {
                    RoomTypeId = 7,
                    TypeName = "Executive",
                    MaxOccupancy = 4,
                    Description = "Không gian kết nối rộng rãi và đầy đủ tiện nghi, là sự lựa chọn hoàn hảo để gia đình hoặc nhóm bạn cùng nhau chia sẻ kỳ nghỉ đáng nhớ.",
                    BasePrice = 3200000m,
                    CreatedAt = DateTime.Now.AddMonths(-4),
                    UpdatedAt = DateTime.Now.AddDays(-7)
                },
                new RoomType
                {
                    RoomTypeId = 8,
                    TypeName = "Panoramic",
                    MaxOccupancy = 3,
                    Description = "Thiết kế tinh tế với khu vực làm việc yên tĩnh và tiện nghi cao cấp, giúp bạn vừa tập trung hiệu quả vừa thư giãn nạp lại năng lượng.",
                    BasePrice = 4000000m,
                    CreatedAt = DateTime.Now.AddMonths(-3),
                    UpdatedAt = DateTime.Now.AddDays(-1)
                },
                new RoomType
                {
                    RoomTypeId = 9,
                    TypeName = "Junior",
                    MaxOccupancy = 2,
                    Description = "Tận hưởng tầm nhìn bao quát toàn cảnh từ không gian nghỉ dưỡng riêng tư và độc đáo, nơi vẻ đẹp của cảnh quan và sự tinh tế trong thiết kế hòa quyện làm một.",
                    BasePrice = 1800000m,
                    CreatedAt = DateTime.Now.AddMonths(-3),
                    UpdatedAt = null
                }
            };
        }
    }
}