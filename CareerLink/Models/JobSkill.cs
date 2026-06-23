using System.ComponentModel.DataAnnotations.Schema;

namespace CareerLink.Models
{
    public class JobSkill
    {
        public Guid Id { get; set; }

        public Guid JobId { get; set; }
        [ForeignKey("JobId")]
        public Job Job { get; set; } = null!;

        public Guid SkillId { get; set; }
        [ForeignKey("SkillId")]
        public Skill Skill { get; set; } = null!;
    }
}
