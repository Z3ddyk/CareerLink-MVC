using System.ComponentModel.DataAnnotations;

namespace CareerLink.ViewModels
{
    public class ProfileViewModel
    {
        [Display(Name = "Professional Headline")]
        public string? Headline { get; set; }

        [Display(Name = "Bio / About Me")]
        public string? Bio { get; set; }

        [Display(Name = "Phone Number")]
        public string? Phone { get; set; }

        [Display(Name = "Location")]
        public string? Location { get; set; }

        [Display(Name = "Years of Experience")]
        public int? YearsExperience { get; set; }

        [Display(Name = "LinkedIn URL")]
        public string? LinkedinUrl { get; set; }

        [Display(Name = "GitHub URL")]
        public string? GithubUrl { get; set; }

        [Display(Name = "Portfolio URL")]
        public string? PortfolioUrl { get; set; }

        [Display(Name = "Availability")]
        public string? Availability { get; set; }

        [Display(Name = "Skills (comma-separated)")]
        public string? SkillsText { get; set; }
    }
}
