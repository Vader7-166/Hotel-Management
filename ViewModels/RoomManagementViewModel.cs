using Hotel_Management.Models;

namespace Hotel_Management.ViewModels
{
    public class RoomManagementViewModel
    {
        public List<FloorGroup> Floors { get; set; }
        public List<RoomType> RoomTypes { get; set; }
    }
    public class FloorGroup
    {
        public int FloorNumber { get; set; }
        public List<Room> Rooms { get; set; }
    }
}
