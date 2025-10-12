namespace Hotel_Management.Models.Customers
{
    public class ModelCustomer
    {
        // Đổi thành public và đặt tên theo quy tắc
        public string CustomerCode { get; set; } // Ví dụ: "#CUS001"
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerPhone { get; set; }

        // Dùng string để hiển thị trực tiếp trong View cho đơn giản
        public string CustomerType { get; set; } // "VIP", "Regular", "New"
        public string CustomerStatus { get; set; } // "Active", "Inactive"

        public int TotalBooking { get; set; }
        public DateTime? LastVisit { get; set; } // Dùng DateTime? là tốt, View sẽ định dạng lại

        // Thêm thuộc tính cho ảnh đại diện
        public string AvatarUrl { get; set; }
    }
}