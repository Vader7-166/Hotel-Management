using Hotel_Management.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Hotel_Management.ViewModels
{
    public class RoomDetailViewModel
    {
        public int RoomId { get; set; } = 0;
        public string RoomNumber { get; set; } = string.Empty;
        public int? Floor { get; set; } = 1;
        public string Status { get; set; } = "Available";
        public string Note { get; set; } = string.Empty;
        public int RoomTypeId { get; set; } = 0;
        public List<SelectListItem> AvailableRoomTypes { get; set; } = new List<SelectListItem>();
    }
}
