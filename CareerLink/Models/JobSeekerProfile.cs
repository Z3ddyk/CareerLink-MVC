using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CareerLink.Models
{
    public class JobSeekerProfile
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; } = null!;

        public string? Headline { get; set; }
        public string? Bio { get; set; }
        public string? Phone { get; set; }
        public string? Location { get; set; }
        public int? YearsExperience { get; set; }
        public string? ResumeUrl { get; set; }
        public string? LinkedinUrl { get; set; }
        public string? GithubUrl { get; set; }
        public string? PortfolioUrl { get; set; }
        public string? Availability { get; set; } // immediately | two_weeks | one_month | not_looking
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // Navigation
        public ICollection<SeekerSkill> SeekerSkills { get; set; } = new List<SeekerSkill>();
        public ICollection<Application> Applications { get; set; } = new List<Application>();
        public ICollection<SavedJob> SavedJobs { get; set; } = new List<SavedJob>();
    }
}
