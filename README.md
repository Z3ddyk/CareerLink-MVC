# CareerLink — Kenya Job Portal

A full-stack job portal web application built with **ASP.NET Core MVC (.NET 8)** and **SQLite**, designed to connect Kenyan job seekers with employers. The platform supports three distinct user roles (Job Seekers, Employers, Admins) with role-based access control, job matching via skill overlap, and full hiring pipeline management.

---

## Table of Contents

1. [Tech Stack](#tech-stack)
2. [Project Structure](#project-structure)
3. [Database Schema](#database-schema)
4. [Authentication & Session Management](#authentication--session-management)
5. [Controllers & Routes](#controllers--routes)
6. [Features by Role](#features-by-role)
7. [Skill Matching Algorithm](#skill-matching-algorithm)
8. [Seed Data](#seed-data)
9. [Running the Application](#running-the-application)
10. [Demo Accounts](#demo-accounts)

---

## Tech Stack

| Layer | Technology |
|---|---|
| Framework | ASP.NET Core MVC (.NET 8) |
| Database | SQLite via Entity Framework Core 8.0.11 |
| ORM | EF Core with Code-First migrations |
| UI | Bootstrap 5.3.2 (CDN) + Bootstrap Icons 1.11.3 |
| Auth | Manual session-based (no ASP.NET Identity middleware) |
| Password hashing | `Microsoft.AspNetCore.Identity.PasswordHasher<T>` |
| Templating | Razor Views with Tag Helpers |
| Language | C# 12 with nullable reference types enabled |

No Docker, no SQL Server, no cloud dependencies — runs entirely from `dotnet run`.

---

## Project Structure

```
CareerLink/
├── Controllers/
│   ├── HomeController.cs         # Landing page with live stats
│   ├── AccountController.cs      # Register, login, logout, profile, applications
│   ├── JobController.cs          # Browse, details, apply, save jobs
│   ├── EmployerController.cs     # Employer dashboard, post/edit/delete jobs, manage applications
│   └── AdminController.cs        # Admin dashboard, user and job moderation
│
├── Models/
│   ├── User.cs                   # Core user (all roles)
│   ├── JobSeekerProfile.cs       # Extended profile for job seekers
│   ├── EmployerProfile.cs        # Extended profile for employers
│   ├── Job.cs                    # Job listing
│   ├── Application.cs            # Job application (seeker ↔ job)
│   ├── Skill.cs                  # Skill name (shared pool)
│   ├── JobSkill.cs               # Many-to-many: Job ↔ Skill
│   ├── SeekerSkill.cs            # Many-to-many: JobSeekerProfile ↔ Skill
│   ├── SavedJob.cs               # Bookmarked jobs per seeker
│   └── AdminLog.cs               # Audit log for admin actions
│
├── ViewModels/
│   ├── RegisterViewModel.cs      # Registration form (email, name, password, role)
│   ├── LoginViewModel.cs         # Login form
│   ├── ProfileViewModel.cs       # Job seeker profile edit form
│   ├── JobIndexViewModel.cs      # Job listing page (jobs + filter state + pagination)
│   ├── JobDetailViewModel.cs     # Job detail page (job + match %, application state)
│   ├── ApplyViewModel.cs         # Cover letter application form
│   ├── PostJobViewModel.cs       # Post/edit job form (shared between PostJob & EditJob)
│   ├── EmployerDashboardViewModel.cs  # Employer stats + job list
│   ├── ViewApplicationsViewModel.cs   # Applications list for a single job
│   └── AdminDashboardViewModel.cs     # All admin stats + users + jobs + reports
│
├── Data/
│   ├── ApplicationDbContext.cs   # EF Core DbContext with all constraints
│   └── SeedData.cs               # Static seeder (runs once on startup)
│
├── Views/
│   ├── Shared/
│   │   └── _Layout.cshtml        # Master layout: navbar, alerts, footer
│   ├── Home/Index.cshtml         # Hero + live stats + feature cards + CTA
│   ├── Account/                  # Register, Login, Profile, MyApplications, ApplicationDetail
│   ├── Job/                      # Index (browse), Details, Apply, SavedJobs
│   ├── Employer/                 # Dashboard, PostJob, EditJob, ViewApplications
│   └── Admin/                    # Dashboard (4 tabs: Overview, Users, Jobs, Reports)
│
├── Migrations/                   # EF Core migration history
├── Program.cs                    # App bootstrap, DI, middleware, seed call
├── appsettings.json              # Connection string (SQLite)
└── CareerLink.db                 # SQLite database file (auto-created)
```

---

## Database Schema

### `Users`
The central identity table for all roles.

| Column | Type | Notes |
|---|---|---|
| Id | GUID (PK) | |
| Email | string | Unique index |
| PasswordHash | string | BCrypt-style hash via `PasswordHasher<User>` |
| Role | string | `job_seeker` \| `employer` \| `admin` |
| FullName | string | Max 150 chars |
| IsActive | bool | `false` = suspended by admin |
| IsVerified | bool | Reserved for future email verification |
| LastLoginAt | DateTime? | Updated on every successful login |
| CreatedAt / UpdatedAt | DateTime | |

### `JobSeekerProfiles`
One-to-one extension of `Users` (unique index on `UserId`).

| Column | Type | Notes |
|---|---|---|
| Headline | string? | e.g. "Full-Stack Developer" |
| Bio | string? | Professional summary |
| Phone | string? | |
| Location | string? | e.g. "Nairobi" |
| YearsExperience | int? | |
| LinkedinUrl / GithubUrl / PortfolioUrl | string? | External links |
| Availability | string? | `immediately` \| `two_weeks` \| `one_month` \| `not_looking` |

### `EmployerProfiles`
One-to-one extension of `Users` (unique index on `UserId`).

| Column | Type | Notes |
|---|---|---|
| CompanyName | string | Required |
| Industry | string? | e.g. "Telecommunications" |
| CompanySize | string? | e.g. `51-200` |
| WebsiteUrl | string? | |
| Headquarters | string? | City/region |
| FoundedYear | int? | |
| IsVerified | bool | Employer verification flag |
| Description | string? | Company about text |

### `Jobs`

| Column | Type | Notes |
|---|---|---|
| EmployerId | GUID (FK) | → `EmployerProfiles` |
| Title | string | Max 200 chars |
| Description | string | Full job description |
| Requirements | string? | Bullet-point requirements |
| JobType | string | `full-time` \| `part-time` \| `contract` \| `internship` \| `remote` |
| Location | string | e.g. "Nairobi, Kenya" |
| SalaryMin / SalaryMax | decimal? | In KES |
| Currency | string | Always `KES` |
| ExperienceYears | string? | e.g. "3-5 years" |
| Status | string | `active` \| `draft` \| `paused` \| `closed` |
| Deadline | DateTime? | Application closing date |
| ViewsCount | int | Incremented on every detail page load |

### `Applications`
Composite unique index on `(JobSeekerProfileId, JobId)` — one application per seeker per job, enforced at the database level.

| Column | Type | Notes |
|---|---|---|
| JobId | GUID (FK) | |
| JobSeekerProfileId | GUID (FK) | |
| CoverLetter | string? | Min 50 chars enforced in ViewModel |
| Status | string | `pending` → `reviewed` → `shortlisted` → `hired` \| `rejected` |
| EmployerNotes | string? | Private employer notes, never shown to seeker |

### `Skills` / `JobSkills` / `SeekerSkills`
Shared skill pool with many-to-many join tables. Skill names have a unique index so the same skill name is never duplicated. Both join tables have composite unique indexes to prevent duplicate pairing.

### `AdminLogs`
Audit trail for every admin action (suspend user, activate user, close job, remove job). Stores action type, target type, target ID, and a description string.

---

## Authentication & Session Management

CareerLink uses **manual session-based authentication** — no middleware like ASP.NET Core Identity is involved in the auth pipeline.

**How login works:**
1. User submits email + password via `POST /Account/Login`
2. `AccountController` fetches the user from the database by email
3. `PasswordHasher<User>.VerifyHashedPassword()` compares the submitted password against the stored hash
4. On success, three values are written to the server-side session:
   - `"UserId"` — the user's GUID as a string
   - `"UserName"` — the user's full name (for navbar display)
   - `"UserRole"` — `job_seeker`, `employer`, or `admin`
5. The user is redirected based on role: employers → Employer Dashboard, admins → Admin Dashboard, seekers → My Applications

**How role guards work:**
Each controller reads `HttpContext.Session.GetString("UserRole")` and redirects unauthenticated or wrong-role requests before doing any database work. There is no `[Authorize]` attribute — the checks are manual method guards that return `Forbid()` or redirect to login.

**Session timeout:** 60 minutes of idle time. HttpOnly cookie.

**Logout:** `GET /Account/Logout` calls `HttpContext.Session.Clear()` and redirects to the home page.

**Password hashing:** `Microsoft.AspNetCore.Identity.PasswordHasher<User>` produces PBKDF2-SHA256 hashes with a random per-password salt. The plain `Microsoft.AspNetCore.Identity` package is used only for this utility; no other Identity services are active.

---

## Controllers & Routes

### `HomeController`
| Route | Action | Description |
|---|---|---|
| `GET /` | `Index` | Renders home page with live stats from DB: active job count, number of hiring companies, and applications submitted this month (passed via `ViewBag`) |

### `AccountController`
| Route | Action | Description |
|---|---|---|
| `GET /Account/Register` | `Register` | Registration form with role selector |
| `POST /Account/Register` | `Register` | Creates `User` + role-specific profile, logs user in, redirects by role |
| `GET /Account/Login` | `Login` | Login form |
| `POST /Account/Login` | `Login` | Verifies credentials, updates `LastLoginAt`, sets session, redirects by role |
| `GET /Account/Logout` | `Logout` | Clears session |
| `GET /Account/Profile` | `Profile` | Loads job seeker's profile + skills (job_seeker only) |
| `POST /Account/Profile` | `Profile` | Saves profile fields; replaces skills: removes all existing `SeekerSkill` rows then re-inserts from comma-separated input, creating new `Skill` records if needed |
| `GET /Account/MyApplications` | `MyApplications` | Lists all applications for the logged-in seeker, ordered newest first, with job and employer eagerly loaded |
| `GET /Account/ApplicationDetail/{id}` | `ApplicationDetail` | Detail view for a single application — seeker can only see their own |

### `JobController`
| Route | Action | Description |
|---|---|---|
| `GET /Job` | `Index` | Browse active jobs. Supports `keyword` (searches title, description, company name), `jobType`, and `location` query parameters. Paginated at 9 jobs per page using `Skip/Take`. |
| `GET /Job/Details/{id}` | `Details` | Full job detail page. Increments `ViewsCount` on every load. If viewer is a job seeker, also computes skill match %, checks if already applied, and checks if saved. |
| `GET /Job/Apply/{id}` | `Apply` | Shows cover letter form. Guards: must be job_seeker, job must be active, must not have already applied. |
| `POST /Job/Apply` | `Apply` | Submits application. Double-checks duplicate constraint before inserting. |
| `POST /Job/SaveJob` | `SaveJob` | Toggle: if not saved, creates `SavedJob`; if already saved, deletes it. Redirects back to job detail. |
| `GET /Job/SavedJobs` | `SavedJobs` | Lists all saved jobs for the seeker, with job and skill info eagerly loaded. |

### `EmployerController`
| Route | Action | Description |
|---|---|---|
| `GET /Employer/Dashboard` | `Dashboard` | Summary stats (active jobs, total applications, pending-review count) + table of all posted jobs with per-job application counts |
| `GET /Employer/PostJob` | `PostJob` | Job posting form |
| `POST /Employer/PostJob` | `PostJob` | Creates `Job` record, then calls `AddSkillsToJobAsync` to parse comma-separated skills, find or create each `Skill`, and insert `JobSkill` join rows |
| `GET /Employer/EditJob/{id}` | `EditJob` | Pre-fills the job form. Verifies the job belongs to the logged-in employer. |
| `POST /Employer/EditJob` | `EditJob` | Updates all job fields. Deletes all existing `JobSkill` rows for the job, then re-inserts from the updated skills input. |
| `POST /Employer/DeleteJob/{id}` | `DeleteJob` | Manually deletes all child records first (`JobSkills`, `Applications`, `SavedJobs`) to satisfy foreign key constraints, then deletes the `Job`. |
| `GET /Employer/ViewApplications/{id}` | `ViewApplications` | All applications for one job, with applicant name, email, headline, experience, skills, cover letter preview, and current status shown per applicant. |
| `POST /Employer/UpdateApplicationStatus` | `UpdateApplicationStatus` | Updates `Status` and `EmployerNotes` on an application. Verifies the application belongs to a job owned by the employer. |

The private `AddSkillsToJobAsync` helper method handles the shared skill-creation logic: splits the input by commas, trims whitespace, deduplicates case-insensitively, then for each skill name: queries for an existing `Skill`, or creates one if not found, and inserts a `JobSkill` join row.

### `AdminController`
| Route | Action | Description |
|---|---|---|
| `GET /Admin/Dashboard` | `Dashboard` | Loads all users, jobs (with applications), and applications in one pass. Computes all stats server-side and passes to `AdminDashboardViewModel`. Accepts `?tab=overview\|users\|jobs\|reports` to select active tab. |
| `POST /Admin/SuspendUser/{id}` | `SuspendUser` | Sets `User.IsActive = false`. Suspended users cannot log in. Logs action to `AdminLogs`. |
| `POST /Admin/ActivateUser/{id}` | `ActivateUser` | Sets `User.IsActive = true`. Logs action to `AdminLogs`. |
| `POST /Admin/CloseJob/{id}` | `CloseJob` | Sets `Job.Status = "closed"`. Job disappears from the public browse list. Logs action. |
| `POST /Admin/RemoveJob/{id}` | `RemoveJob` | Deletes job and all child records (`JobSkills`, `Applications`, `SavedJobs`). Logs action. |

All admin write actions are guarded by `if (!IsAdmin()) return Forbid()` and all use `[ValidateAntiForgeryToken]` to prevent CSRF.

---

## Features by Role

### Job Seeker
- **Register / Login** — create a job seeker account; session established immediately after registration
- **Profile** — edit headline, bio, phone, location, years of experience, LinkedIn, GitHub, portfolio URL, availability status, and skills (free-text comma list)
- **Browse Jobs** — search by keyword (title/description/company), filter by job type and location, paginated grid of job cards
- **Job Detail** — view full description, requirements, required skills, company info, salary, deadline, and view count
- **Skill Match** — a percentage badge shows how many of the job's required skills the seeker's profile already has, using the formula: `overlapping skills ÷ total job skills × 100`
- **Apply** — submit a cover letter (minimum 50 characters enforced); one application per job enforced at both app and DB level
- **Save Jobs** — bookmark jobs for later; toggle unsave from the saved jobs list or the job detail page
- **My Applications** — table of all submitted applications with colour-coded status badges (pending / reviewed / shortlisted / hired / rejected)
- **Application Detail** — visual timeline showing current stage in the hiring pipeline

### Employer
- **Dashboard** — stats row (active jobs, total applications, pending review) + table of all posted jobs
- **Post Job** — two-column form: job details on the left, compensation and deadline on the right; skills as a comma-separated input
- **Edit Job** — same form pre-filled; skills re-parsed and replaced on save
- **Delete Job** — cascades child record deletion manually before removing the job
- **View Applications** — per-applicant cards showing name, email, headline, experience, skill tags, cover letter preview, applied date; inline form to update status and add private notes

### Admin
- **Overview Tab** — total users (by role), total companies, total jobs (active vs. total), total applications (by status: pending / reviewed / shortlisted / rejected / hired), applications this month, and an audit log viewer
- **Users Tab** — full user table with role badge, join date, status, and Suspend/Activate action buttons
- **Jobs Tab** — all jobs with company, type, location, application count, and Close/Remove action buttons
- **Reports Tab** — job type distribution breakdown + top 5 most-applied-to positions with salary range and application count
- **Audit Log** — every admin action is written to `AdminLogs` with the action name, target type, target ID, and a description

---

## Skill Matching Algorithm

When a logged-in job seeker views a job detail page, the controller computes a match percentage:

```csharp
var jobSkillIds = job.JobSkills.Select(js => js.SkillId).ToHashSet();
var seekerSkillIds = profile.SeekerSkills.Select(ss => ss.SkillId).ToHashSet();

if (jobSkillIds.Count > 0)
{
    var matchCount = jobSkillIds.Intersect(seekerSkillIds).Count();
    vm.SkillMatchPercent = (int)Math.Round((double)matchCount / jobSkillIds.Count * 100);
}
```

The badge colour adapts to the score: green (≥70%), yellow (≥40%), grey (<40%). If the seeker has no skills or the job requires no skills, no badge is shown.

---

## Data Layer & EF Core Setup

`ApplicationDbContext` registers all 10 `DbSet<T>` tables and configures the following constraints in `OnModelCreating`:

| Constraint | Table | Columns |
|---|---|---|
| Unique email | Users | Email |
| Unique profile per user | JobSeekerProfiles | UserId |
| Unique profile per user | EmployerProfiles | UserId |
| One application per seeker per job | Applications | (JobSeekerProfileId, JobId) |
| One saved entry per seeker per job | SavedJobs | (JobSeekerProfileId, JobId) |
| One skill per job | JobSkills | (JobId, SkillId) |
| One skill per seeker | SeekerSkills | (JobSeekerProfileId, SkillId) |
| Unique skill names | Skills | Name |

The database is created on first run via `context.Database.EnsureCreated()` in `Program.cs`, which applies the schema without running migrations explicitly.

---

## Seed Data

`SeedData.InitializeAsync` runs at startup and checks `if (await context.Users.AnyAsync()) return;` — so it runs exactly once. It seeds:

- **1 admin** account
- **4 employer accounts** with full company profiles:
  - Safaricom PLC (Telecommunications, 500+ employees)
  - Equity Bank Kenya (Banking & Finance)
  - KCB Group (Banking & Finance)
  - Nation Media Group (Media & Publishing)
- **4 job seeker accounts** with profiles and skills:
  - John Mwangi — Software Engineer (Python, Django, JavaScript, React, SQL, Git)
  - Mary Wambua — Data Analyst (Python, R, SQL, Power BI, Excel)
  - Peter Otieno — Network Engineer (Cisco, Linux, TCP/IP, Networking)
  - Amina Hassan — Content Writer (Content Writing, SEO, WordPress)
- **12 skills** in the shared pool
- **10 job listings** across all four companies, covering full-time, part-time, contract, internship, and remote types
- **6 applications** from seekers to various jobs
- **4 saved jobs**

---

## Running the Application

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

### Steps

```bash
# Clone the repository
git clone <repo-url>
cd CareerLink

# Run the application
dotnet run
```

The app starts on `http://localhost:5000` (or `https://localhost:5001`). The SQLite database (`CareerLink.db`) and all seed data are created automatically on the first run.

To run on a specific port:
```bash
dotnet run --urls="http://localhost:5100"
```

To reset the database and re-seed from scratch:
```bash
rm CareerLink.db
dotnet run
```

---

## Demo Accounts

All accounts below are pre-seeded and ready to use immediately after `dotnet run`.

### Admin
| Email | Password |
|---|---|
| admin@careerlink.co.ke | Admin@123 |

### Employers
| Email | Password | Company |
|---|---|---|
| hr@safaricom.co.ke | Safaricom@123 | Safaricom PLC |
| careers@equitybank.co.ke | Equity@123 | Equity Bank Kenya |
| talent@kcbgroup.com | KCB@12345 | KCB Group |

### Job Seekers
| Email | Password | Name |
|---|---|---|
| john.mwangi@gmail.com | John@123 | John Mwangi |
| mary.wambua@gmail.com | Mary@123 | Mary Wambua |
| peter.otieno@gmail.com | Peter@123 | Peter Otieno |
| amina.hassan@gmail.com | Amina@123 | Amina Hassan |

---

## NuGet Packages

| Package | Version | Purpose |
|---|---|---|
| Microsoft.EntityFrameworkCore | 8.0.11 | ORM core |
| Microsoft.EntityFrameworkCore.Sqlite | 8.0.11 | SQLite database provider |
| Microsoft.EntityFrameworkCore.Tools | 8.0.11 | CLI migration tools (`dotnet ef`) |
| Microsoft.EntityFrameworkCore.Design | 8.0.11 | Design-time EF Core support |
| Microsoft.AspNetCore.Identity.EntityFrameworkCore | 8.0.11 | `PasswordHasher<T>` utility |
