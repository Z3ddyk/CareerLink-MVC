using System.ComponentModel.DataAnnotations.Schema;

namespace CareerLink.Models
{
    public class SeekerSkill
    {
        public Guid Id { get; set; }

        public Guid JobSeekerProfileId { get; set; }
        [ForeignKey("JobSeekerProfileId")]
        public JobSeekerProfile JobSeekerProfile { get; set; } = null!;

        public Guid SkillId { get; set; }
        [ForeignKey("SkillId")]
        public Skill Skill { get; set; } = null!;

        public string? ProficiencyLevel { get; set; } // beginner | intermediate | advanced | expert
    }
}
