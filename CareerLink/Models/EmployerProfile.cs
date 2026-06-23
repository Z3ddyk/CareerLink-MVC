using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CareerLink.Models
{
    public class EmployerProfile
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; } = null!;

        [Required, StringLength(200)]
        public string CompanyName { get; set; } = string.Empty;

        public string? CompanyLogoUrl { get; set; }
        public string? Industry { get; set; }
        public string? CompanySize { get; set; } // 1-10 | 11-50 | 51-200 | 201-500 | 500+
        public string? WebsiteUrl { get; set; }
        public string? Description { get; set; }
        public string? Headquarters { get; set; }
        public int? FoundedYear { get; set; }
        public bool IsVerified { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // Navigation
        public ICollection<Job> Jobs { get; set; } = new List<Job>();
    }
}
