namespace DotnetAPI.DTOs
{
    public partial class UserCompleteDto
    {
        public int UserId { get; set; } = 0;

        public string Email { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Gender { get; set; } = "";

        public string Department { get; set; } = "";
        public string Role { get; set; } = "";
        public string Team { get; set; } = "";
        public DateTime DateJoined { get; set; } = DateTime.Now;

        public decimal Salary { get; set; }
        public int DaysPerWeek { get; set; } = 5;
        public int HoursPerDay { get; set; } = 8;
    }
}
