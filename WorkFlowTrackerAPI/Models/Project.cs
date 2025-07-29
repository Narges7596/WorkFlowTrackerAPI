namespace DotnetAPI.Models
{
    public partial class Project
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; } = "";
        public int ClientId { get; set; }
    }
}