using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using CareerLink.Data;
using CareerLink.Models;
using CareerLink.ViewModels;

namespace CareerLink.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordHasher<User> _hasher = new();

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        private Guid? GetUserId()
        {
            var idStr = HttpContext.Session.GetString("UserId");
            return idStr != null ? Guid.Parse(idStr) : null;
        }

        private string? GetUserRole() => HttpContext.Session.GetString("UserRole");

        // GET: /Account/Register
        public IActionResult Register() => View(new RegisterViewModel());

        // POST: /Account/Register
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (model.Role == "employer" && string.IsNullOrWhiteSpace(model.CompanyName))
                ModelState.AddModelError("CompanyName", "Company name is required for employer accounts.");

            if (!ModelState.IsValid) return View(model);

            if (await _context.Users.AnyAsync(u => u.Email == model.Email))
            {
                ModelState.AddModelError("Email", "This email is already registered.");
                return View(model);
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = model.Email,
                FullName = model.FullName,
                Role = model.Role,
                IsActive = true,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            user.PasswordHash = _hasher.HashPassword(user, model.Password);

            _context.Users.Add(user);

            if (model.Role == "job_seeker")
            {
                _context.JobSeekerProfiles.Add(new JobSeekerProfile
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                });
            }
            else if (model.Role == "employer")
            {
                _context.EmployerProfiles.Add(new EmployerProfile
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    CompanyName = model.CompanyName!,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                });
            }

            await _context.SaveChangesAsync();

            HttpContext.Session.SetString("UserId", user.Id.ToString());
            HttpContext.Session.SetString("UserName", user.FullName);
            HttpContext.Session.SetString("UserRole", user.Role);

            return model.Role switch
            {
                "employer" => RedirectToAction("Dashboard", "Employer"),
                _ => RedirectToAction("MyApplications", "Account")
            };
        }

        // GET: /Account/Login
        public IActionResult Login() => View(new LoginViewModel());

        // POST: /Account/Login
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
                return View(model);
            }

            if (!user.IsActive)
            {
                ModelState.AddModelError(string.Empty, "Your account has been suspended. Please contact support.");
                return View(model);
            }

            var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, model.Password);
            if (result == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
                return View(model);
            }

            user.LastLoginAt = DateTime.Now;
            await _context.SaveChangesAsync();

            HttpContext.Session.SetString("UserId", user.Id.ToString());
            HttpContext.Session.SetString("UserName", user.FullName);
            HttpContext.Session.SetString("UserRole", user.Role);

            return user.Role switch
            {
                "employer" => RedirectToAction("Dashboard", "Employer"),
                "admin" => RedirectToAction("Dashboard", "Admin"),
                _ => RedirectToAction("MyApplications", "Account")
            };
        }

        // GET: /Account/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        // GET: /Account/Profile
        public async Task<IActionResult> Profile()
        {
            var userId = GetUserId();
            if (userId == null) return RedirectToAction("Login");
            if (GetUserRole() != "job_seeker") return RedirectToAction("Index", "Home");

            var profile = await _context.JobSeekerProfiles
                .Include(p => p.SeekerSkills)
                .ThenInclude(ss => ss.Skill)
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (profile == null) return RedirectToAction("Login");

            var vm = new ProfileViewModel
            {
                Headline = profile.Headline,
                Bio = profile.Bio,
                Phone = profile.Phone,
                Location = profile.Location,
                YearsExperience = profile.YearsExperience,
                LinkedinUrl = profile.LinkedinUrl,
                GithubUrl = profile.GithubUrl,
                PortfolioUrl = profile.PortfolioUrl,
                Availability = profile.Availability,
                SkillsText = string.Join(", ", profile.SeekerSkills.Select(ss => ss.Skill.Name))
            };

            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            return View(vm);
        }

        // POST: /Account/Profile
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(ProfileViewModel model)
        {
            var userId = GetUserId();
            if (userId == null) return RedirectToAction("Login");
            if (GetUserRole() != "job_seeker") return RedirectToAction("Index", "Home");

            var profile = await _context.JobSeekerProfiles
                .Include(p => p.SeekerSkills)
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (profile == null) return RedirectToAction("Login");

            profile.Headline = model.Headline;
            profile.Bio = model.Bio;
            profile.Phone = model.Phone;
            profile.Location = model.Location;
            profile.YearsExperience = model.YearsExperience;
            profile.LinkedinUrl = model.LinkedinUrl;
            profile.GithubUrl = model.GithubUrl;
            profile.PortfolioUrl = model.PortfolioUrl;
            profile.Availability = model.Availability;
            profile.UpdatedAt = DateTime.Now;

            // Update skills
            _context.SeekerSkills.RemoveRange(profile.SeekerSkills);

            if (!string.IsNullOrWhiteSpace(model.SkillsText))
            {
                var skillNames = model.SkillsText
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

                    _context.SeekerSkills.Add(new SeekerSkill
                    {
                        Id = Guid.NewGuid(),
                        JobSeekerProfileId = profile.Id,
                        SkillId = skill.Id
                    });
                }
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "Profile updated successfully!";
            return RedirectToAction("Profile");
        }

        // GET: /Account/MyApplications
        public async Task<IActionResult> MyApplications()
        {
            var userId = GetUserId();
            if (userId == null) return RedirectToAction("Login");
            if (GetUserRole() != "job_seeker") return RedirectToAction("Index", "Home");

            var profile = await _context.JobSeekerProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
            if (profile == null) return RedirectToAction("Login");

            var applications = await _context.Applications
                .Include(a => a.Job)
                .ThenInclude(j => j.Employer)
                .Where(a => a.JobSeekerProfileId == profile.Id)
                .OrderByDescending(a => a.AppliedAt)
                .ToListAsync();

            return View(applications);
        }

        // GET: /Account/ApplicationDetail/{id}
        public async Task<IActionResult> ApplicationDetail(Guid id)
        {
            var userId = GetUserId();
            if (userId == null) return RedirectToAction("Login");
            if (GetUserRole() != "job_seeker") return RedirectToAction("Index", "Home");

            var profile = await _context.JobSeekerProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
            if (profile == null) return RedirectToAction("Login");

            var application = await _context.Applications
                .Include(a => a.Job)
                .ThenInclude(j => j.Employer)
                .ThenInclude(e => e.User)
                .FirstOrDefaultAsync(a => a.Id == id && a.JobSeekerProfileId == profile.Id);

            if (application == null) return NotFound();

            return View(application);
        }
    }
}
