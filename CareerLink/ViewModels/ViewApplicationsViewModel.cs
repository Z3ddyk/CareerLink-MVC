using CareerLink.Models;

namespace CareerLink.ViewModels
{
    public class ViewApplicationsViewModel
    {
        public Job Job { get; set; } = null!;
        public List<Application> Applications { get; set; } = new();
    }
}
