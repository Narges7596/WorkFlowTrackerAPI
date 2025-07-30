namespace DotnetAPI.Models
{
    public partial class AbsentDay
    {
        public int AbsentDayId { get; set; }
        public int UserId { get; set; }
        public TimeOnly StartDate { get; set; }
        public TimeOnly EndDate { get; set; }
        public string Type { get; set; } = ""; //vacation, sick, special, unpaid, other
        public string Status { get; set; } = ""; //pending, approved, rejected
        public string Description { get; set; } = "";
        public TimeOnly RequestedAt { get; set; }
        public int ApprovedBy { get; set; }
        public TimeOnly ApprovedAt { get; set; }
    }
}