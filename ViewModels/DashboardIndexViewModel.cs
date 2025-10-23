namespace Hotel_Management.ViewModels
{
    public class DashboardIndexViewModel
    {
        public class ChartDataViewModel
        {
            public List<string> Labels { get; set; } = new List<string>();
            public List<decimal> Data { get; set; } = new List<decimal>();
        }

        public class CheckInInfo
        {
            public string Room { get; set; }
            public string Customer { get; set; }
            public string CheckTime { get; set; }
            public string Status { get; set; }
        }

        public class CheckOutInfo
        {
            public string Room { get; set; }
            public string Customer { get; set; }
            public string CheckTime { get; set; }
            public string Status { get; set; }
        }

        // 1. Data for card
        public int TotalRoom { get; set; }
        public int CurrentlyRenting { get; set; }
        public int AvailableRooms { get; set; }
        public int MoreThanYesterday { get; set; }
        public decimal RevenueToday { get; set; }
        public decimal RevenueCompareToYesterday { get; set; }

        // 2. Data for chart
        public ChartDataViewModel DailyRevenueChart { get; set; }
        public ChartDataViewModel MonthlyRevenueChart { get; set; }

        // 3. Data for checkIn/Out
        public List<CheckInInfo> CheckInList { get; set; }
        public List<CheckOutInfo> CheckOutList { get; set; }

        public int TotalCheckIn { get; set; }
        public int TotalCheckOut { get; set; }

        // Constructor
        public DashboardIndexViewModel()
        {
            DailyRevenueChart = new ChartDataViewModel();
            MonthlyRevenueChart = new ChartDataViewModel();
            CheckInList = new List<CheckInInfo>();
            CheckOutList = new List<CheckOutInfo>();
        }
    }
}
