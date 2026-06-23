using System.ComponentModel.DataAnnotations;

namespace CareerLink.Models
{
    public class User
    {
        public Guid Id { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = "job_seeker"; // "job_seeker" | "employer" | "admin"

        [Required, StringLength(150)]
        public string FullName { get; set; } = string.Empty;

        public string? AvatarUrl { get; set; }
        public bool IsVerified { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public DateTime? LastLoginAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        public JobSeekerProfile? JobSeekerProfile { get; set; }
        public EmployerProfile? EmployerProfile { get; set; }
    }
}
