using System.ComponentModel.DataAnnotations;

namespace CareerLink.ViewModels
{
    public class PostJobViewModel
    {
        public Guid? JobId { get; set; }

        [Required, StringLength(200), Display(Name = "Job Title")]
        public string Title { get; set; } = string.Empty;

        [Required, Display(Name = "Job Description")]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "Requirements")]
        public string? Requirements { get; set; }

        [Required, Display(Name = "Job Type")]
        public string JobType { get; set; } = "full-time";

        [Required, Display(Name = "Location")]
        public string Location { get; set; } = string.Empty;

        [Display(Name = "Minimum Salary (KES)")]
        public decimal? SalaryMin { get; set; }

        [Display(Name = "Maximum Salary (KES)")]
        public decimal? SalaryMax { get; set; }

        [Display(Name = "Experience Required")]
        public string? ExperienceYears { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; } = "active";

        [Display(Name = "Application Deadline")]
        [DataType(DataType.Date)]
        public DateTime? Deadline { get; set; }

        [Display(Name = "Required Skills (comma-separated)")]
        public string? SkillsText { get; set; }
    }
}
