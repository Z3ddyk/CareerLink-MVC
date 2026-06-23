using System.ComponentModel.DataAnnotations;

namespace CareerLink.ViewModels
{
    public class ApplyViewModel
    {
        public Guid JobId { get; set; }
        public string JobTitle { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Cover letter is required.")]
        [Display(Name = "Cover Letter")]
        [MinLength(50, ErrorMessage = "Cover letter should be at least 50 characters.")]
        public string CoverLetter { get; set; } = string.Empty;
    }
}
