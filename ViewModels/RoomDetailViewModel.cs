using Hotel_Management.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Hotel_Management.ViewModels
{
    public class RoomDetailViewModel
    {
        public int RoomId { get; set; } = 0;
        [Required(ErrorMessage = "Please select a room number.")]
        [Display(Name = "Room Number")]
        public string RoomNumber { get; set; } = string.Empty;
        [Required(ErrorMessage = "Please select a floor.")]
        [Display(Name = "Floor")]
        public int? Floor { get; set; } = 1;
        [Required(ErrorMessage = "Please select room status")]
        [Display(Name = "Status")]
        public string Status { get; set; } = "Available";
        public string? Note { get; set; }
        [Required(ErrorMessage = "Please select room type")]
        [Display(Name = "RoomTypeId")]
        public int RoomTypeId { get; set; } = 0;

        // Customer information fill in note
        public string? CustomerName { get; set; }
        public DateOnly? CheckInDate { get; set; }
        public DateOnly? CheckOutDate { get; set; }
        public List<SelectListItem> AvailableRoomTypes { get; set; } = new List<SelectListItem>();
    }
}
