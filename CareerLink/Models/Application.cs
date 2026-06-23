using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CareerLink.Models
{
    public class Application
    {
        public Guid Id { get; set; }

        public Guid JobId { get; set; }
        [ForeignKey("JobId")]
        public Job Job { get; set; } = null!;

        public Guid JobSeekerProfileId { get; set; }
        [ForeignKey("JobSeekerProfileId")]
        public JobSeekerProfile JobSeekerProfile { get; set; } = null!;

        public string? CoverLetter { get; set; }
        public string? ResumeUrl { get; set; }
        public string Status { get; set; } = "pending"; // pending | reviewed | shortlisted | rejected | hired
        public string? EmployerNotes { get; set; }
        public DateTime AppliedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
