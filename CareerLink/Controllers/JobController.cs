using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CareerLink.Data;
using CareerLink.Models;
using CareerLink.ViewModels;

namespace CareerLink.Controllers
{
    public class JobController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const int PageSize = 9;

        public JobController(ApplicationDbContext context)
        {
            _context = context;
        }

        private Guid? GetUserId()
        {
            var idStr = HttpContext.Session.GetString("UserId");
            return idStr != null ? Guid.Parse(idStr) : null;
        }

        // GET: /Job
        public async Task<IActionResult> Index(string? keyword, string? jobType, string? location, int page = 1)
        {
            var query = _context.Jobs
                .Include(j => j.Employer)
                .Include(j => j.JobSkills)
                .ThenInclude(js => js.Skill)
                .Include(j => j.Applications)
                .Where(j => j.Status == "active")
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
                query = query.Where(j =>
                    j.Title.Contains(keyword) ||
                    j.Description.Contains(keyword) ||
                    j.Employer.CompanyName.Contains(keyword));

            if (!string.IsNullOrWhiteSpace(jobType))
                query = query.Where(j => j.JobType == jobType);

            if (!string.IsNullOrWhiteSpace(location))
                query = query.Where(j => j.Location.Contains(location));

            var total = await query.CountAsync();
            var jobs = await query
                .OrderByDescending(j => j.CreatedAt)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            var vm = new JobIndexViewModel
            {
                Jobs = jobs,
                Keyword = keyword,
                JobType = jobType,
                Location = location,
                TotalCount = total,
                Page = page,
                TotalPages = (int)Math.Ceiling(total / (double)PageSize)
            };

            return View(vm);
        }

        // GET: /Job/Details/{id}
        public async Task<IActionResult> Details(Guid id)
        {
            var job = await _context.Jobs
                .Include(j => j.Employer)
                .ThenInclude(e => e.User)
                .Include(j => j.JobSkills)
                .ThenInclude(js => js.Skill)
                .FirstOrDefaultAsync(j => j.Id == id);

            if (job == null) return NotFound();

            job.ViewsCount++;
            await _context.SaveChangesAsync();

            var vm = new JobDetailViewModel { Job = job };
            var userId = GetUserId();

            if (userId != null && HttpContext.Session.GetString("UserRole") == "job_seeker")
            {
                var profile = await _context.JobSeekerProfiles
                    .Include(p => p.SeekerSkills)
                    .FirstOrDefaultAsync(p => p.UserId == userId);

                if (profile != null)
                {
                    vm.ExistingApplication = await _context.Applications
                        .FirstOrDefaultAsync(a => a.JobId == id && a.JobSeekerProfileId == profile.Id);

                    vm.HasApplied = vm.ExistingApplication != null;

                    vm.IsSaved = await _context.SavedJobs
                        .AnyAsync(s => s.JobId == id && s.JobSeekerProfileId == profile.Id);

                    var jobSkillIds = job.JobSkills.Select(js => js.SkillId).ToHashSet();
                    var seekerSkillIds = profile.SeekerSkills.Select(ss => ss.SkillId).ToHashSet();
                    if (jobSkillIds.Count > 0)
                    {
                        var matchCount = jobSkillIds.Intersect(seekerSkillIds).Count();
                        vm.SkillMatchPercent = (int)Math.Round((double)matchCount / jobSkillIds.Count * 100);
                    }
                }
            }

            return View(vm);
        }

        // GET: /Job/Apply/{id}
        public async Task<IActionResult> Apply(Guid id)
        {
            var userId = GetUserId();
            if (userId == null) return RedirectToAction("Login", "Account");
            if (HttpContext.Session.GetString("UserRole") != "job_seeker")
            {
                TempData["Error"] = "Only job seekers can apply for jobs.";
                return RedirectToAction("Details", new { id });
            }

            var job = await _context.Jobs
                .Include(j => j.Employer)
                .FirstOrDefaultAsync(j => j.Id == id && j.Status == "active");

            if (job == null) return NotFound();

            var profile = await _context.JobSeekerProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
            if (profile == null) return NotFound();

            if (await _context.Applications.AnyAsync(a => a.JobId == id && a.JobSeekerProfileId == profile.Id))
            {
                TempData["Error"] = "You have already applied to this job.";
                return RedirectToAction("Details", new { id });
            }

            var vm = new ApplyViewModel
            {
                JobId = id,
                JobTitle = job.Title,
                CompanyName = job.Employer.CompanyName
            };

            return View(vm);
        }

        // POST: /Job/Apply
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Apply(ApplyViewModel model)
        {
            var userId = GetUserId();
            if (userId == null) return RedirectToAction("Login", "Account");
            if (HttpContext.Session.GetString("UserRole") != "job_seeker")
                return RedirectToAction("Details", new { id = model.JobId });

            if (!ModelState.IsValid) return View(model);

            var profile = await _context.JobSeekerProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
            if (profile == null) return NotFound();

            if (await _context.Applications.AnyAsync(a => a.JobId == model.JobId && a.JobSeekerProfileId == profile.Id))
            {
                TempData["Error"] = "You have already applied to this job.";
                return RedirectToAction("Details", new { id = model.JobId });
            }

            _context.Applications.Add(new Application
            {
                Id = Guid.NewGuid(),
                JobId = model.JobId,
                JobSeekerProfileId = profile.Id,
                CoverLetter = model.CoverLetter,
                Status = "pending",
                AppliedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            });

            await _context.SaveChangesAsync();

            TempData["Success"] = "Application submitted successfully! Good luck!";
            return RedirectToAction("MyApplications", "Account");
        }

        // POST: /Job/SaveJob
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveJob(Guid jobId)
        {
            var userId = GetUserId();
            if (userId == null) return RedirectToAction("Login", "Account");
            if (HttpContext.Session.GetString("UserRole") != "job_seeker")
                return RedirectToAction("Details", new { id = jobId });

            var profile = await _context.JobSeekerProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
            if (profile == null) return RedirectToAction("Details", new { id = jobId });

            var existing = await _context.SavedJobs
                .FirstOrDefaultAsync(s => s.JobId == jobId && s.JobSeekerProfileId == profile.Id);

            if (existing == null)
            {
                _context.SavedJobs.Add(new SavedJob
                {
                    Id = Guid.NewGuid(),
                    JobId = jobId,
                    JobSeekerProfileId = profile.Id,
                    SavedAt = DateTime.Now
                });
                TempData["Success"] = "Job saved to your list!";
            }
            else
            {
                _context.SavedJobs.Remove(existing);
                TempData["Success"] = "Job removed from saved list.";
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Details", new { id = jobId });
        }

        // GET: /Job/SavedJobs
        public async Task<IActionResult> SavedJobs()
        {
            var userId = GetUserId();
            if (userId == null) return RedirectToAction("Login", "Account");
            if (HttpContext.Session.GetString("UserRole") != "job_seeker")
                return RedirectToAction("Index", "Home");

            var profile = await _context.JobSeekerProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
            if (profile == null) return RedirectToAction("Login", "Account");

            var savedJobs = await _context.SavedJobs
                .Include(s => s.Job)
                .ThenInclude(j => j.Employer)
                .Include(s => s.Job)
                .ThenInclude(j => j.JobSkills)
                .ThenInclude(js => js.Skill)
                .Where(s => s.JobSeekerProfileId == profile.Id)
                .OrderByDescending(s => s.SavedAt)
                .ToListAsync();

            return View(savedJobs);
        }
    }
}
