using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CareerLink.Data;
using CareerLink.Models;
using CareerLink.ViewModels;

namespace CareerLink.Controllers
{
    public class EmployerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EmployerController(ApplicationDbContext context)
        {
            _context = context;
        }

        private Guid? GetUserId()
        {
            var idStr = HttpContext.Session.GetString("UserId");
            return idStr != null ? Guid.Parse(idStr) : null;
        }

        private bool IsEmployer() => HttpContext.Session.GetString("UserRole") == "employer";

        private async Task<EmployerProfile?> GetEmployerProfileAsync()
        {
            var userId = GetUserId();
            if (userId == null) return null;
            return await _context.EmployerProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
        }

        // GET: /Employer/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            var userId = GetUserId();
            if (userId == null) return RedirectToAction("Login", "Account");
            if (!IsEmployer()) return RedirectToAction("Index", "Home");

            var profile = await _context.EmployerProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
            if (profile == null) return RedirectToAction("Login", "Account");

            var jobs = await _context.Jobs
                .Include(j => j.Applications)
                .Where(j => j.EmployerId == profile.Id)
                .OrderByDescending(j => j.CreatedAt)
                .ToListAsync();

            var vm = new EmployerDashboardViewModel
            {
                EmployerProfile = profile,
                ActiveJobsCount = jobs.Count(j => j.Status == "active"),
                TotalApplicationsCount = jobs.Sum(j => j.Applications.Count),
                ToReviewCount = jobs.Sum(j => j.Applications.Count(a => a.Status == "pending")),
                JobsWithStats = jobs.Select(j => new JobWithStats
                {
                    Job = j,
                    ApplicationCount = j.Applications.Count
                }).ToList()
            };

            return View(vm);
        }

        // GET: /Employer/PostJob
        public IActionResult PostJob()
        {
            if (GetUserId() == null) return RedirectToAction("Login", "Account");
            if (!IsEmployer()) return RedirectToAction("Index", "Home");
            return View(new PostJobViewModel());
        }

        // POST: /Employer/PostJob
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> PostJob(PostJobViewModel model)
        {
            var userId = GetUserId();
            if (userId == null) return RedirectToAction("Login", "Account");
            if (!IsEmployer()) return RedirectToAction("Index", "Home");

            if (!ModelState.IsValid) return View(model);

            var profile = await GetEmployerProfileAsync();
            if (profile == null) return RedirectToAction("Login", "Account");

            var job = new Job
            {
                Id = Guid.NewGuid(),
                EmployerId = profile.Id,
                Title = model.Title,
                Description = model.Description,
                Requirements = model.Requirements,
                JobType = model.JobType,
                Location = model.Location,
                SalaryMin = model.SalaryMin,
                SalaryMax = model.SalaryMax,
                Currency = "KES",
                ExperienceYears = model.ExperienceYears,
                Status = model.Status,
                Deadline = model.Deadline,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _context.Jobs.Add(job);
            await _context.SaveChangesAsync();

            await AddSkillsToJobAsync(job.Id, model.SkillsText);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Job posted successfully!";
            return RedirectToAction("Dashboard");
        }

        // GET: /Employer/EditJob/{id}
        public async Task<IActionResult> EditJob(Guid id)
        {
            var userId = GetUserId();
            if (userId == null) return RedirectToAction("Login", "Account");
            if (!IsEmployer()) return RedirectToAction("Index", "Home");

            var profile = await GetEmployerProfileAsync();
            if (profile == null) return NotFound();

            var job = await _context.Jobs
                .Include(j => j.JobSkills)
                .ThenInclude(js => js.Skill)
                .FirstOrDefaultAsync(j => j.Id == id && j.EmployerId == profile.Id);

            if (job == null) return NotFound();

            var vm = new PostJobViewModel
            {
                JobId = job.Id,
                Title = job.Title,
                Description = job.Description,
                Requirements = job.Requirements,
                JobType = job.JobType,
                Location = job.Location,
                SalaryMin = job.SalaryMin,
                SalaryMax = job.SalaryMax,
                ExperienceYears = job.ExperienceYears,
                Status = job.Status,
                Deadline = job.Deadline,
                SkillsText = string.Join(", ", job.JobSkills.Select(js => js.Skill.Name))
            };

            return View(vm);
        }

        // POST: /Employer/EditJob
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditJob(PostJobViewModel model)
        {
            var userId = GetUserId();
            if (userId == null) return RedirectToAction("Login", "Account");
            if (!IsEmployer()) return RedirectToAction("Index", "Home");

            if (!ModelState.IsValid) return View(model);

            var profile = await GetEmployerProfileAsync();
            if (profile == null) return NotFound();

            var job = await _context.Jobs
                .Include(j => j.JobSkills)
                .FirstOrDefaultAsync(j => j.Id == model.JobId && j.EmployerId == profile.Id);

            if (job == null) return NotFound();

            job.Title = model.Title;
            job.Description = model.Description;
            job.Requirements = model.Requirements;
            job.JobType = model.JobType;
            job.Location = model.Location;
            job.SalaryMin = model.SalaryMin;
            job.SalaryMax = model.SalaryMax;
            job.ExperienceYears = model.ExperienceYears;
            job.Status = model.Status;
            job.Deadline = model.Deadline;
            job.UpdatedAt = DateTime.Now;

            _context.JobSkills.RemoveRange(job.JobSkills);
            await _context.SaveChangesAsync();

            await AddSkillsToJobAsync(job.Id, model.SkillsText);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Job updated successfully!";
            return RedirectToAction("Dashboard");
        }

        // POST: /Employer/DeleteJob
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteJob(Guid id)
        {
            var userId = GetUserId();
            if (userId == null) return RedirectToAction("Login", "Account");
            if (!IsEmployer()) return RedirectToAction("Index", "Home");

            var profile = await GetEmployerProfileAsync();
            if (profile == null) return NotFound();

            var job = await _context.Jobs
                .Include(j => j.JobSkills)
                .Include(j => j.Applications)
                .Include(j => j.SavedJobs)
                .FirstOrDefaultAsync(j => j.Id == id && j.EmployerId == profile.Id);

            if (job == null) return NotFound();

            _context.JobSkills.RemoveRange(job.JobSkills);
            _context.Applications.RemoveRange(job.Applications);
            _context.SavedJobs.RemoveRange(job.SavedJobs);
            _context.Jobs.Remove(job);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Job deleted successfully.";
            return RedirectToAction("Dashboard");
        }

        // GET: /Employer/ViewApplications/{id}
        public async Task<IActionResult> ViewApplications(Guid id)
        {
            var userId = GetUserId();
            if (userId == null) return RedirectToAction("Login", "Account");
            if (!IsEmployer()) return RedirectToAction("Index", "Home");

            var profile = await GetEmployerProfileAsync();
            if (profile == null) return NotFound();

            var job = await _context.Jobs.FirstOrDefaultAsync(j => j.Id == id && j.EmployerId == profile.Id);
            if (job == null) return NotFound();

            var applications = await _context.Applications
                .Include(a => a.JobSeekerProfile)
                .ThenInclude(p => p.User)
                .Include(a => a.JobSeekerProfile)
                .ThenInclude(p => p.SeekerSkills)
                .ThenInclude(ss => ss.Skill)
                .Where(a => a.JobId == id)
                .OrderByDescending(a => a.AppliedAt)
                .ToListAsync();

            var vm = new ViewApplicationsViewModel
            {
                Job = job,
                Applications = applications
            };

            return View(vm);
        }

        // POST: /Employer/UpdateApplicationStatus
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateApplicationStatus(Guid applicationId, string status, string? notes, Guid jobId)
        {
            var userId = GetUserId();
            if (userId == null) return RedirectToAction("Login", "Account");
            if (!IsEmployer()) return RedirectToAction("Index", "Home");

            var profile = await GetEmployerProfileAsync();
            if (profile == null) return NotFound();

            var application = await _context.Applications
                .Include(a => a.Job)
                .FirstOrDefaultAsync(a => a.Id == applicationId && a.Job.EmployerId == profile.Id);

            if (application == null) return NotFound();

            application.Status = status;
            application.EmployerNotes = notes;
            application.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            TempData["Success"] = "Application status updated successfully.";
            return RedirectToAction("ViewApplications", new { id = jobId });
        }

        private async Task AddSkillsToJobAsync(Guid jobId, string? skillsText)
        {
            if (string.IsNullOrWhiteSpace(skillsText)) return;

            var skillNames = skillsText
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .Where(s => !string.IsNullOrEmpty(s))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            foreach (var skillName in skillNames)
            {
                var skill = await _context.Skills.FirstOrDefaultAsync(s => s.Name == skillName);
                if (skill == null)
                {
                    skill = new Skill { Id = Guid.NewGuid(), Name = skillName, CreatedAt = DateTime.Now };
                    _context.Skills.Add(skill);
                    await _context.SaveChangesAsync();
                }

                var exists = await _context.JobSkills.AnyAsync(js => js.JobId == jobId && js.SkillId == skill.Id);
                if (!exists)
                {
                    _context.JobSkills.Add(new JobSkill { Id = Guid.NewGuid(), JobId = jobId, SkillId = skill.Id });
                }
            }
        }
    }
}
