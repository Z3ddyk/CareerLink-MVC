using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CareerLink.Data;
using CareerLink.Models;

namespace CareerLink.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var thisMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            ViewBag.ActiveJobs = await _context.Jobs.CountAsync(j => j.Status == "active");
            ViewBag.CompaniesHiring = await _context.EmployerProfiles.CountAsync();
            ViewBag.ApplicationsThisMonth = await _context.Applications.CountAsync(a => a.AppliedAt >= thisMonth);

            return View();
        }

        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
