using DotnetAPI.Models;

namespace DotnetAPI.DTOs
{
    public partial class WorkLogGetDto
    {
        public int WorkLogId { get; set; }
        public int UserId { get; set; }
        public DateOnly WorkDay { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public string Description { get; set; } = "";
        public string ProjectName { get; set; } = "";
        public string ClientName { get; set; } = "";
        public List<Tag> Tags { get; set; } = new List<Tag>();
    }
}
