namespace DotnetAPI.Models
{
    public partial class WorkLog
    {
        public int WorkLogId { get; set; }
        public int UserId { get; set; }
        public DateOnly WorkDay { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public int ProjectID { get; set; }
        public string Description { get; set; } = "";
    }
}