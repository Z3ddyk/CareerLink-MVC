using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CareerLink.Data;
using CareerLink.Models;
using CareerLink.ViewModels;

namespace CareerLink.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        private Guid? GetUserId()
        {
            var idStr = HttpContext.Session.GetString("UserId");
            return idStr != null ? Guid.Parse(idStr) : null;
        }

        private bool IsAdmin() => HttpContext.Session.GetString("UserRole") == "admin";

        // GET: /Admin/Dashboard
        public async Task<IActionResult> Dashboard(string tab = "overview")
        {
            if (GetUserId() == null) return RedirectToAction("Login", "Account");
            if (!IsAdmin()) return RedirectToAction("Index", "Home");

            var users = await _context.Users.OrderByDescending(u => u.CreatedAt).ToListAsync();

            var jobs = await _context.Jobs
                .Include(j => j.Employer)
                .ThenInclude(e => e.User)
                .Include(j => j.Applications)
                .OrderByDescending(j => j.CreatedAt)
                .ToListAsync();

            var applications = await _context.Applications.ToListAsync();

            var thisMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            var jobTypeCounts = jobs
                .GroupBy(j => j.JobType)
                .Select(g => new JobTypeCount { JobType = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .ToList();

            var topPositions = jobs
                .Where(j => j.Applications.Count > 0)
                .OrderByDescending(j => j.Applications.Count)
                .Take(5)
                .Select(j => new TopPosition
                {
                    Title = j.Title,
                    SalaryMin = j.SalaryMin,
                    SalaryMax = j.SalaryMax,
                    ApplicationCount = j.Applications.Count
                })
                .ToList();

            var vm = new AdminDashboardViewModel
            {
                TotalUsers = users.Count,
                JobSeekerCount = users.Count(u => u.Role == "job_seeker"),
                EmployerCount = users.Count(u => u.Role == "employer"),
                AdminCount = users.Count(u => u.Role == "admin"),
                ActiveCompanies = await _context.EmployerProfiles.CountAsync(),
                TotalApplications = applications.Count,
                PendingApplications = applications.Count(a => a.Status == "pending"),
                ReviewedApplications = applications.Count(a => a.Status == "reviewed"),
                ShortlistedApplications = applications.Count(a => a.Status == "shortlisted"),
                RejectedApplications = applications.Count(a => a.Status == "rejected"),
                HiredApplications = applications.Count(a => a.Status == "hired"),
                TotalJobs = jobs.Count,
                ActiveJobs = jobs.Count(j => j.Status == "active"),
                TotalSalaryValue = jobs.Where(j => j.SalaryMax.HasValue).Sum(j => j.SalaryMax!.Value),
                ApplicationsThisMonth = applications.Count(a => a.AppliedAt >= thisMonth),
                Users = users,
                Jobs = jobs,
                JobTypeCounts = jobTypeCounts,
                TopPositions = topPositions,
                ActiveTab = tab
            };

            return View(vm);
        }

        // POST: /Admin/SuspendUser/{id}
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> SuspendUser(Guid id)
        {
            if (!IsAdmin()) return Forbid();

            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            user.IsActive = false;
            user.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();

            await LogActionAsync("Suspend User", "user", id, $"Suspended user: {user.Email}");
            TempData["Success"] = $"User {user.Email} has been suspended.";
            return RedirectToAction("Dashboard", new { tab = "users" });
        }

        // POST: /Admin/ActivateUser/{id}
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ActivateUser(Guid id)
        {
            if (!IsAdmin()) return Forbid();

            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            user.IsActive = true;
            user.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();

            await LogActionAsync("Activate User", "user", id, $"Activated user: {user.Email}");
            TempData["Success"] = $"User {user.Email} has been reactivated.";
            return RedirectToAction("Dashboard", new { tab = "users" });
        }

        // POST: /Admin/CloseJob/{id}
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CloseJob(Guid id)
        {
            if (!IsAdmin()) return Forbid();

            var job = await _context.Jobs.FindAsync(id);
            if (job == null) return NotFound();

            job.Status = "closed";
            job.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();

            await LogActionAsync("Close Job", "job", id, $"Closed job: {job.Title}");
            TempData["Success"] = "Job has been closed.";
            return RedirectToAction("Dashboard", new { tab = "jobs" });
        }

        // POST: /Admin/RemoveJob/{id}
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveJob(Guid id)
        {
            if (!IsAdmin()) return Forbid();

            var job = await _context.Jobs
                .Include(j => j.JobSkills)
                .Include(j => j.Applications)
                .Include(j => j.SavedJobs)
                .FirstOrDefaultAsync(j => j.Id == id);

            if (job == null) return NotFound();

            var title = job.Title;
            _context.JobSkills.RemoveRange(job.JobSkills);
            _context.Applications.RemoveRange(job.Applications);
            _context.SavedJobs.RemoveRange(job.SavedJobs);
            _context.Jobs.Remove(job);
            await _context.SaveChangesAsync();

            await LogActionAsync("Remove Job", "job", id, $"Removed job: {title}");
            TempData["Success"] = "Job has been removed.";
            return RedirectToAction("Dashboard", new { tab = "jobs" });
        }

        private async Task LogActionAsync(string action, string targetType, Guid targetId, string details)
        {
            var adminId = GetUserId();
            if (adminId == null) return;

            _context.AdminLogs.Add(new AdminLog
            {
                Id = Guid.NewGuid(),
                AdminId = adminId.Value,
                Action = action,
                TargetType = targetType,
                TargetId = targetId,
                Details = details,
                CreatedAt = DateTime.Now
            });

            await _context.SaveChangesAsync();
        }
    }
}
