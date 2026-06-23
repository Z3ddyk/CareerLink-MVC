using CareerLink.Models;

namespace CareerLink.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int TotalUsers { get; set; }
        public int JobSeekerCount { get; set; }
        public int EmployerCount { get; set; }
        public int AdminCount { get; set; }
        public int ActiveCompanies { get; set; }
        public int TotalApplications { get; set; }
        public int PendingApplications { get; set; }
        public int ReviewedApplications { get; set; }
        public int ShortlistedApplications { get; set; }
        public int RejectedApplications { get; set; }
        public int HiredApplications { get; set; }
        public int TotalJobs { get; set; }
        public int ActiveJobs { get; set; }
        public decimal TotalSalaryValue { get; set; }
        public int ApplicationsThisMonth { get; set; }

        public List<User> Users { get; set; } = new();
        public List<Job> Jobs { get; set; } = new();
        public List<JobTypeCount> JobTypeCounts { get; set; } = new();
        public List<TopPosition> TopPositions { get; set; } = new();

        public string ActiveTab { get; set; } = "overview";
    }

    public class JobTypeCount
    {
        public string JobType { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class TopPosition
    {
        public string Title { get; set; } = string.Empty;
        public decimal? SalaryMin { get; set; }
        public decimal? SalaryMax { get; set; }
        public int ApplicationCount { get; set; }
    }
}
