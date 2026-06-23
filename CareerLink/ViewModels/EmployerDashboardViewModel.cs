using CareerLink.Models;

namespace CareerLink.ViewModels
{
    public class EmployerDashboardViewModel
    {
        public EmployerProfile? EmployerProfile { get; set; }
        public int ActiveJobsCount { get; set; }
        public int TotalApplicationsCount { get; set; }
        public int ToReviewCount { get; set; }
        public List<JobWithStats> JobsWithStats { get; set; } = new();
    }

    public class JobWithStats
    {
        public Job Job { get; set; } = null!;
        public int ApplicationCount { get; set; }
    }
}
