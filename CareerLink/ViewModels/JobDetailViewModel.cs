using CareerLink.Models;

namespace CareerLink.ViewModels
{
    public class JobDetailViewModel
    {
        public Job Job { get; set; } = null!;
        public bool HasApplied { get; set; }
        public bool IsSaved { get; set; }
        public int SkillMatchPercent { get; set; }
        public Application? ExistingApplication { get; set; }
    }
}
