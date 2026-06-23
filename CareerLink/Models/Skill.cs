using System.ComponentModel.DataAnnotations;

namespace CareerLink.Models
{
    public class Skill
    {
        public Guid Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;

        public string? Category { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation
        public ICollection<JobSkill> JobSkills { get; set; } = new List<JobSkill>();
        public ICollection<SeekerSkill> SeekerSkills { get; set; } = new List<SeekerSkill>();
    }
}
