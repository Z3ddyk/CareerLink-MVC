using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CareerLink.Models
{
    public class Job
    {
        public Guid Id { get; set; }

        public Guid EmployerId { get; set; }
        [ForeignKey("EmployerId")]
        public EmployerProfile Employer { get; set; } = null!;

        [Required, StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        public string? Requirements { get; set; }
        public string JobType { get; set; } = "full-time"; // full-time | part-time | contract | internship | remote
        public string Location { get; set; } = string.Empty;
        public decimal? SalaryMin { get; set; }
        public decimal? SalaryMax { get; set; }
        public string Currency { get; set; } = "KES";
        public string? ExperienceYears { get; set; }
        public string Status { get; set; } = "active"; // active | closed | draft
        public DateTime? Deadline { get; set; }
        public int ViewsCount { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // Navigation
        public ICollection<JobSkill> JobSkills { get; set; } = new List<JobSkill>();
        public ICollection<Application> Applications { get; set; } = new List<Application>();
        public ICollection<SavedJob> SavedJobs { get; set; } = new List<SavedJob>();
    }
}
