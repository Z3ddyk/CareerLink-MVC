You are building **CareerLink**, an ASP.NET Core MVC (.NET 8) job portal for Kenya. Think of it as a simpler LinkedIn focused on connecting Kenyan employers with job seekers and reducing unemployment. This is a university unit project (CMT 307 — .NET Programming) with a **live demo presentation on 1st July 2026**. It must actually run — no files are submitted, just a working demo.

## Environment

- **OS:** Fedora Linux
- **IDE:** VS Code with C# Dev Kit
- **Framework:** ASP.NET Core MVC, .NET 8
- **Database:** SQLite via EF Core (connection string: `Data Source=CareerLink.db`)
- **Front-end:** Bootstrap 5 (CDN), Bootstrap Icons
- **Auth:** Manual session-based (HttpContext.Session), password hashing via `Microsoft.AspNetCore.Identity.PasswordHasher<User>`
- **No Docker, no SQL Server, no external APIs**

## What Already Exists

The project scaffold is already created and builds successfully. Here's what's done:

- `CareerLink.csproj` with all NuGet packages (EF Core SQLite, Identity.EntityFrameworkCore)
- `Program.cs` configured with SQLite DbContext, session middleware
- `Data/ApplicationDbContext.cs` with all 10 DbSets and unique index constraints
- All 10 model classes in `Models/` with proper FK relationships and navigation properties
- `Views/Shared/_Layout.cshtml` — Bootstrap 5 layout with navbar (shows Login/Register when logged out, username + Logout when logged in, using session)
- `Views/Home/Index.cshtml` — landing page (working)
- `Controllers/HomeController.cs` — basic home controller
- `Migrations/` — InitialCreate migration already applied, `CareerLink.db` exists
- `.gitignore`, `.vscode/` configs

**Everything else is empty stub folders.** The Views/Account, Views/Job, Views/Employer, Views/Admin folders exist but are empty. No controllers exist except HomeController. All navbar links (Browse Jobs, Login, Register) lead to errors.

## What You Must Build

Build the complete working application. Every page must be functional with real database CRUD operations, not placeholder/static content. After you finish, I should be able to: register as a job seeker, log in, browse jobs, apply to a job, save a job; register as an employer, log in, post a job, view applicants, change application status; log in as admin, see dashboard stats, manage users and jobs.

## Marking Criteria (50 marks total)

1. **Database in a DBMS (10 marks)** — DONE (SQLite + EF Core migrations)
2. **Well designed Front-end (10 marks)** — Job seeker experience: home page, job browsing, job detail, application flow, profile, saved jobs, my applications
3. **Well designed Admin panel (10 marks)** — Admin dashboard with stats, user management, job management, reports
4. **Front-end connected to DB (5 marks)** — All job seeker pages must read/write real data
5. **Admin panel connected to DB (5 marks)** — All admin pages must read/write real data
6. **Correct Logic (10 marks)** — Auth works, role-based access, proper CRUD, status workflows, validation

## Functional Requirements

- **FR1 — User Registration & Authentication:** Register as job_seeker or employer with email + password. Login redirects to role-appropriate dashboard. Session stores UserId, UserName, UserRole. Logout clears session. Passwords hashed with PasswordHasher.
- **FR2 — Job Advertisement Management:** Employers can create, edit, and delete job postings (title, description, requirements, job_type, location, salary range, deadline). Jobs belong to the employer who posted them.
- **FR3 — Job Search & Filtering:** Public job browse page with search by keyword, filter by job_type, filter by location. Pagination. Each listing links to a detail page.
- **FR4 — Online Job Application:** Logged-in job seekers can apply to any job with a cover letter. One application per job per seeker. Applications table tracks status.
- **FR5 — Application Tracking Dashboard:** Employers see all applications for their jobs, can change status (pending → reviewed → shortlisted → rejected → hired), can add internal notes.

## Database Schema (10 tables, already created as C# models)

### users
id (UUID PK), email (UNIQUE NOT NULL), password_hash (NOT NULL), role (ENUM: job_seeker|employer|admin), full_name (VARCHAR 150), avatar_url (TEXT NULL), is_verified (BOOL default false), is_active (BOOL default true), last_login_at (TIMESTAMP NULL), created_at, updated_at

### job_seeker_profiles
id (UUID PK), user_id (FK→users, UNIQUE), headline (VARCHAR 255 NULL), bio (TEXT NULL), phone (VARCHAR 20 NULL), location (VARCHAR 100 NULL), resume_url (TEXT NULL), linkedin_url (TEXT NULL), github_url (TEXT NULL), portfolio_url (TEXT NULL), years_experience (SMALLINT NULL), availability (ENUM: immediately|two_weeks|one_month|not_looking NULL), created_at

### employer_profiles
id (UUID PK), user_id (FK→users, UNIQUE), company_name (VARCHAR 200 NOT NULL), company_logo_url (TEXT NULL), industry (VARCHAR 100 NULL), company_size (ENUM: 1-10|11-50|51-200|201-500|500+ NULL), website_url (TEXT NULL), description (TEXT NULL), headquarters (VARCHAR 150 NULL), founded_year (SMALLINT NULL), is_verified (BOOL default false), created_at, updated_at

### jobs
id (UUID PK), employer_id (FK→employer_profiles), title (VARCHAR 200 NOT NULL), description (TEXT NOT NULL), requirements (TEXT NULL), job_type (ENUM: full-time|part-time|contract|internship|remote NOT NULL), location (VARCHAR 100 NOT NULL), salary_min (DECIMAL 12,2 NULL), salary_max (DECIMAL 12,2 NULL), currency (VARCHAR 5 default 'KES'), experience_years (VARCHAR 20 NULL), status (ENUM: active|closed|draft|paused default 'active'), deadline (DATE NULL), views_count (INT default 0), created_at, updated_at

### skills
id (UUID PK), name (VARCHAR 100 UNIQUE NOT NULL), category (VARCHAR 100 NULL), created_at

### job_skills
id (UUID PK), job_id (FK→jobs), skill_id (FK→skills), UNIQUE(job_id, skill_id)

### seeker_skills
id (UUID PK), seeker_id (FK→job_seeker_profiles), skill_id (FK→skills), proficiency (ENUM: beginner|intermediate|advanced|expert NULL), UNIQUE(seeker_id, skill_id)

### applications
id (UUID PK), job_id (FK→jobs), seeker_id (FK→job_seeker_profiles), cover_letter (TEXT NULL), resume_url (TEXT NULL), status (ENUM: pending|reviewed|shortlisted|rejected|hired default 'pending'), employer_notes (TEXT NULL), applied_at, updated_at

### saved_jobs
id (UUID PK), seeker_id (FK→job_seeker_profiles), job_id (FK→jobs), saved_at, UNIQUE(seeker_id, job_id)

### admin_logs
id (UUID PK), admin_id (FK→users), action (TEXT NOT NULL), target_type (TEXT NULL), target_id (UUID NULL), details (TEXT NULL), created_at

## UI Design Specifications (from submitted mockups)

Match these mockup layouts using Bootstrap 5. Use a clean, professional design with the existing blue primary color scheme.

### Navbar (already in _Layout.cshtml)
- Logo "CareerLink" on left
- Links: Home, Browse Jobs
- Right side: Login/Register (when logged out) OR "Welcome, [Name]" + role-specific dashboard link + Logout (when logged in)
- Admin navbar adds: ADMIN dropdown with Dashboard, Users, Jobs, Reports tabs

### Landing Page (already done)
- Hero: "Find Your Next Career Opportunity" with search and post-a-job buttons
- Three feature cards: Find Jobs, Post Jobs, Track Applications

### Job Browse Page (`/Job` or `/Job/Index`)
- Search bar with keyword input
- Filter dropdowns: Job Type, Location
- Job listing cards showing: title, company name, location, salary range (KES format), job type badge, posted date, skills tags
- "Showing X jobs" count

### Job Detail Page (`/Job/Details/{id}`)
- Full job info: title, company, location, salary, type, experience, deadline
- Description and requirements sections
- "Apply Now" button (for logged-in seekers), "Save Job" bookmark button
- Company info sidebar

### Job Seeker Dashboard — My Applications (`/Account/MyApplications`)
- "My Applications" heading with subtitle "Track your job applications and their status"
- Table/cards showing: job title, company, applied date, status badge (color-coded: pending=yellow, reviewed=blue, shortlisted=green, rejected=red, hired=purple)
- Empty state: "No Applications Yet" with "Browse Jobs" button

### Employer Dashboard (`/Employer/Dashboard`)
- Stats cards row: Active Jobs count, Total Applications, To Review count
- "Post New Job" button (top right)
- "Your Job Listings" section with table: title, status, applications count, posted date, actions (Edit, View Applications, Delete)

### Admin Dashboard (`/Admin/Dashboard`)
- Tabs: Overview, Users, Jobs, Reports
- **Overview tab:** Stats cards — Total Users (with breakdown), Active Companies, Total Applications, System Health %, Total Salary Value
- Application Status Distribution: colored cards showing count per status (Pending, Reviewed, Shortlisted, Rejected)
- **Users tab:** Table of all users with columns: name, email, role badge, active status, joined date, actions (Suspend/Activate)
- **Jobs tab:** Table of all jobs platform-wide with: title, company, status, applications count, actions (Close/Remove)
- **Reports tab:** Top Positions list with salary ranges, Job Types Distribution breakdown

## Additional Unique Features (to make it stand out)

Add these simple but impressive features:

1. **Skill Matching** — On the job detail page, if a logged-in seeker has skills matching the job's required skills, show a "X% Match" badge. Simple: count overlapping skills / total job skills × 100.

2. **Application Status Timeline** — On the seeker's "My Applications" detail view, show a visual timeline/progress bar of the application status stages (pending → reviewed → shortlisted → hired), highlighting the current stage.

3. **Quick Stats on Home Page** — Show live counts on the landing page: "X Active Jobs", "Y Companies Hiring", "Z Applications This Month" pulled from the database. Makes the platform feel alive.

4. **Employer Application Notes** — When an employer reviews an application, they can leave private notes (employer_notes field) and change status via a dropdown — this shows good use of the database.

5. **Seed Data** — Pre-populate the database with realistic Kenyan data: companies like Safaricom, Equity Bank, KCB, M-PESA; jobs in Nairobi, Mombasa, Kisumu; skills like Python, React, Data Analysis, Project Management. Include 2-3 employer accounts, 3-4 seeker accounts, 8-10 jobs, some applications. Also create one admin account (email: admin@careerlink.co.ke, password: Admin@123). This makes the demo look polished.

## Architecture Rules

- **Controllers:** AccountController (Register, Login, Logout, Profile, MyApplications), JobController (Index, Details, Apply, SaveJob, Search), EmployerController (Dashboard, PostJob, EditJob, DeleteJob, ViewApplications, UpdateApplicationStatus), AdminController (Dashboard, Users, Jobs, Reports, SuspendUser, ActivateUser, CloseJob)
- **Session keys:** "UserId" (Guid as string), "UserName" (string), "UserRole" (string: job_seeker/employer/admin)
- **Auth guard pattern:** At the top of every action that requires auth, check `HttpContext.Session.GetString("UserId")`. If null, redirect to Login. For role-specific pages, also check "UserRole". Create a simple helper or do inline checks.
- **No Identity framework scaffolding** — just manual session-based auth with PasswordHasher for hashing.
- **EF Core queries:** Use `_context.Jobs.Include(j => j.Employer).ThenInclude(e => e.User)` patterns for eager loading. Use `.Where()`, `.OrderByDescending()`, `.Take()` for filtering/sorting/pagination.
- **Views:** Use Razor with tag helpers (`asp-controller`, `asp-action`, `asp-for`, `asp-validation-for`). Use Bootstrap 5 classes for all styling. Use Bootstrap Icons for icons.
- **Decimal display:** Format salaries as KES with thousands separator: `@job.SalaryMin?.ToString("N0")` → "100,000"
- **Date display:** Use `.ToString("MMM dd, yyyy")` for human-readable dates.
- **ViewModels:** Create ViewModels in a `ViewModels/` folder for pages that need data from multiple tables (e.g., AdminDashboardViewModel with counts, EmployerDashboardViewModel).

## Model Updates Needed

The existing models need a few fields added to match the full schema from the design document. Specifically:

- `JobSeekerProfile`: rename `Summary` to `Bio`, add `GithubUrl`, `Availability`, `YearsExperience` (change from string to int?). The `PhoneNumber` field is already `Phone` in schema — keep consistent.
- `EmployerProfile`: add `Headquarters`, `FoundedYear`, `IsVerified`, `CompanySize` fields if missing.

Check each model file against the schema above and update any missing fields. Then run `dotnet ef migrations add UpdateModels` and `dotnet ef database update` to apply changes.

## Order of Work

Build in this order so each step can be tested:

1. **Fix models** — Update any model fields to match the schema exactly. Add new migration.
2. **AccountController + Views** — Register, Login, Logout, Profile. Test: can register and log in.
3. **Seed data** — Create `Data/SeedData.cs` with a static method called from Program.cs to populate demo data on first run.
4. **JobController + Views** — Browse, Search/Filter, Details, Apply, Save. Test: can browse and apply.
5. **EmployerController + Views** — Dashboard, Post/Edit/Delete Job, View Applications, Update Status. Test: can manage jobs and review applicants.
6. **AdminController + Views** — Dashboard with stats, Users management, Jobs management, Reports. Test: admin can see everything and suspend users.
7. **Unique features** — Skill matching, status timeline, home page stats.
8. **Polish** — Consistent styling, error messages, validation, empty states, loading states.
9. **Final test** — Full flow: register seeker → login → browse → apply → switch to employer → review → switch to admin → view dashboard. Everything must work.

## Important Notes

- Always run `dotnet build` after changes to catch compile errors early.
- The project is at `~/Documents/Claude/Projects/.NET project/CareerLink/`
- This is a university project — keep it clean and well-structured but don't over-engineer. Simple and working beats complex and broken.
- Every page should look professional with Bootstrap 5. No raw unstyled HTML.
- Use consistent color coding for status badges across all views.
- The presenter (me) needs to demo the system live. Make sure there's seed data so I don't have to manually create test data during the presentation.

Now build the complete application. Start with fixing models if needed, then work through the order above. Build each controller with all its actions and corresponding views before moving to the next one. Make every page functional with real database operations.
