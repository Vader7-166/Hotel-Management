using Hotel_Management.Models;

namespace Hotel_Management.ViewModels
{
    public class RoomDetailViewModel
    {
        public int RoomID { get; set; }
        public string RoomNumber { get; set; }
        public int RoomTypeID { get; set; }
        public string Status { get; set; }
        public int Floor { get; set; }
        public string Note { get; set; }
        public List<RoomType> AvailableRoomTypes { get; set; }
    }
}
