using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CareerLink.Models
{
    public class AdminLog
    {
        public Guid Id { get; set; }

        public Guid AdminId { get; set; }
        [ForeignKey("AdminId")]
        public User Admin { get; set; } = null!;

        [Required]
        public string Action { get; set; } = string.Empty;

        public string? TargetType { get; set; } // "user" | "job" | "application"
        public Guid? TargetId { get; set; }
        public string? Details { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
