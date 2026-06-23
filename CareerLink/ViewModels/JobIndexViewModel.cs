using CareerLink.Models;

namespace CareerLink.ViewModels
{
    public class JobIndexViewModel
    {
        public List<Job> Jobs { get; set; } = new();
        public string? Keyword { get; set; }
        public string? JobType { get; set; }
        public string? Location { get; set; }
        public int TotalCount { get; set; }
        public int Page { get; set; } = 1;
        public int TotalPages { get; set; }
    }
}
