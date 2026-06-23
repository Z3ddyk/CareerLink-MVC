using System.ComponentModel.DataAnnotations.Schema;

namespace CareerLink.Models
{
    public class SavedJob
    {
        public Guid Id { get; set; }

        public Guid JobSeekerProfileId { get; set; }
        [ForeignKey("JobSeekerProfileId")]
        public JobSeekerProfile JobSeekerProfile { get; set; } = null!;

        public Guid JobId { get; set; }
        [ForeignKey("JobId")]
        public Job Job { get; set; } = null!;

        public DateTime SavedAt { get; set; } = DateTime.Now;
    }
}
