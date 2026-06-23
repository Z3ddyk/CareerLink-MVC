using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using CareerLink.Models;

namespace CareerLink.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(ApplicationDbContext context)
        {
            if (await context.Users.AnyAsync()) return;

            var hasher = new PasswordHasher<User>();

            // ==================== ADMIN ====================
            var admin = new User
            {
                Id = Guid.NewGuid(),
                Email = "admin@careerlink.co.ke",
                FullName = "System Administrator",
                Role = "admin",
                IsVerified = true,
                IsActive = true,
                CreatedAt = DateTime.Now.AddDays(-60)
            };
            admin.PasswordHash = hasher.HashPassword(admin, "Admin@123");

            // ==================== EMPLOYER USERS ====================
            var safaricomUser = new User
            {
                Id = Guid.NewGuid(),
                Email = "hr@safaricom.co.ke",
                FullName = "Safaricom HR Team",
                Role = "employer",
                IsVerified = true,
                IsActive = true,
                CreatedAt = DateTime.Now.AddDays(-50)
            };
            safaricomUser.PasswordHash = hasher.HashPassword(safaricomUser, "Safaricom@123");

            var equityUser = new User
            {
                Id = Guid.NewGuid(),
                Email = "careers@equitybank.co.ke",
                FullName = "Equity Bank Careers",
                Role = "employer",
                IsVerified = true,
                IsActive = true,
                CreatedAt = DateTime.Now.AddDays(-45)
            };
            equityUser.PasswordHash = hasher.HashPassword(equityUser, "Equity@123");

            var kcbUser = new User
            {
                Id = Guid.NewGuid(),
                Email = "talent@kcbgroup.com",
                FullName = "KCB Talent Acquisition",
                Role = "employer",
                IsVerified = true,
                IsActive = true,
                CreatedAt = DateTime.Now.AddDays(-40)
            };
            kcbUser.PasswordHash = hasher.HashPassword(kcbUser, "KCB@12345");

            var nationUser = new User
            {
                Id = Guid.NewGuid(),
                Email = "jobs@nationmedia.com",
                FullName = "Nation Media Group HR",
                Role = "employer",
                IsVerified = false,
                IsActive = true,
                CreatedAt = DateTime.Now.AddDays(-35)
            };
            nationUser.PasswordHash = hasher.HashPassword(nationUser, "Nation@123");

            // ==================== JOB SEEKER USERS ====================
            var johnUser = new User
            {
                Id = Guid.NewGuid(),
                Email = "john.mwangi@gmail.com",
                FullName = "John Mwangi",
                Role = "job_seeker",
                IsVerified = true,
                IsActive = true,
                CreatedAt = DateTime.Now.AddDays(-30)
            };
            johnUser.PasswordHash = hasher.HashPassword(johnUser, "John@123");

            var maryUser = new User
            {
                Id = Guid.NewGuid(),
                Email = "mary.wambua@gmail.com",
                FullName = "Mary Wambua",
                Role = "job_seeker",
                IsVerified = true,
                IsActive = true,
                CreatedAt = DateTime.Now.AddDays(-25)
            };
            maryUser.PasswordHash = hasher.HashPassword(maryUser, "Mary@123");

            var peterUser = new User
            {
                Id = Guid.NewGuid(),
                Email = "peter.otieno@gmail.com",
                FullName = "Peter Otieno",
                Role = "job_seeker",
                IsVerified = false,
                IsActive = true,
                CreatedAt = DateTime.Now.AddDays(-20)
            };
            peterUser.PasswordHash = hasher.HashPassword(peterUser, "Peter@123");

            var aminaUser = new User
            {
                Id = Guid.NewGuid(),
                Email = "amina.hassan@gmail.com",
                FullName = "Amina Hassan",
                Role = "job_seeker",
                IsVerified = true,
                IsActive = true,
                CreatedAt = DateTime.Now.AddDays(-15)
            };
            aminaUser.PasswordHash = hasher.HashPassword(aminaUser, "Amina@123");

            // Save all users
            context.Users.AddRange(admin, safaricomUser, equityUser, kcbUser, nationUser,
                johnUser, maryUser, peterUser, aminaUser);
            await context.SaveChangesAsync();

            // ==================== EMPLOYER PROFILES ====================
            var safaricomProfile = new EmployerProfile
            {
                Id = Guid.NewGuid(),
                UserId = safaricomUser.Id,
                CompanyName = "Safaricom PLC",
                Industry = "Telecommunications",
                CompanySize = "500+",
                WebsiteUrl = "https://www.safaricom.co.ke",
                Description = "Safaricom is Kenya's largest telecommunications company and the operator of M-PESA, Africa's most successful mobile money platform. We connect over 40 million Kenyans and power millions of businesses.",
                Headquarters = "Nairobi, Kenya",
                FoundedYear = 2000,
                IsVerified = true,
                CreatedAt = DateTime.Now.AddDays(-50)
            };

            var equityProfile = new EmployerProfile
            {
                Id = Guid.NewGuid(),
                UserId = equityUser.Id,
                CompanyName = "Equity Bank Kenya",
                Industry = "Banking & Finance",
                CompanySize = "500+",
                WebsiteUrl = "https://equitygroupholdings.com",
                Description = "Equity Bank is a commercial bank headquartered in Nairobi, Kenya. The bank has the largest customer base in the region with operations in Kenya, Uganda, Tanzania, Rwanda, South Sudan and DRC.",
                Headquarters = "Nairobi, Kenya",
                FoundedYear = 1984,
                IsVerified = true,
                CreatedAt = DateTime.Now.AddDays(-45)
            };

            var kcbProfile = new EmployerProfile
            {
                Id = Guid.NewGuid(),
                UserId = kcbUser.Id,
                CompanyName = "KCB Group",
                Industry = "Banking & Finance",
                CompanySize = "500+",
                WebsiteUrl = "https://ke.kcbgroup.com",
                Description = "KCB Group is the largest commercial bank in Kenya by assets. KCB Group has operations in Kenya, Uganda, Tanzania, Rwanda, Burundi, Ethiopia and South Sudan.",
                Headquarters = "Nairobi, Kenya",
                FoundedYear = 1896,
                IsVerified = true,
                CreatedAt = DateTime.Now.AddDays(-40)
            };

            var nationProfile = new EmployerProfile
            {
                Id = Guid.NewGuid(),
                UserId = nationUser.Id,
                CompanyName = "Nation Media Group",
                Industry = "Media & Publishing",
                CompanySize = "201-500",
                WebsiteUrl = "https://www.nationmedia.com",
                Description = "Nation Media Group is the largest independent media house in East and Central Africa with operations in Kenya, Uganda, Tanzania and Rwanda.",
                Headquarters = "Nairobi, Kenya",
                FoundedYear = 1959,
                IsVerified = false,
                CreatedAt = DateTime.Now.AddDays(-35)
            };

            context.EmployerProfiles.AddRange(safaricomProfile, equityProfile, kcbProfile, nationProfile);
            await context.SaveChangesAsync();

            // ==================== JOB SEEKER PROFILES ====================
            var johnProfile = new JobSeekerProfile
            {
                Id = Guid.NewGuid(),
                UserId = johnUser.Id,
                Headline = "Full Stack Software Engineer",
                Bio = "Passionate software engineer with 4 years of experience building scalable web applications. Experienced with React, Node.js, Python and cloud technologies. Looking for challenging roles in tech.",
                Phone = "+254 712 345 678",
                Location = "Nairobi, Kenya",
                YearsExperience = 4,
                LinkedinUrl = "https://linkedin.com/in/johnmwangi",
                GithubUrl = "https://github.com/johnmwangi",
                Availability = "immediately",
                CreatedAt = DateTime.Now.AddDays(-30)
            };

            var maryProfile = new JobSeekerProfile
            {
                Id = Guid.NewGuid(),
                UserId = maryUser.Id,
                Headline = "Data Analyst & Business Intelligence Specialist",
                Bio = "Analytical professional with 3 years of experience transforming complex data into actionable insights. Proficient in Python, SQL, Tableau and Excel. Passionate about data-driven decision making.",
                Phone = "+254 723 456 789",
                Location = "Nairobi, Kenya",
                YearsExperience = 3,
                LinkedinUrl = "https://linkedin.com/in/marywambua",
                Availability = "two_weeks",
                CreatedAt = DateTime.Now.AddDays(-25)
            };

            var peterProfile = new JobSeekerProfile
            {
                Id = Guid.NewGuid(),
                UserId = peterUser.Id,
                Headline = "Senior Project Manager | PMP Certified",
                Bio = "Results-driven project manager with 6 years of experience delivering complex projects on time and within budget. PMP certified with expertise in Agile and traditional project management methodologies.",
                Phone = "+254 734 567 890",
                Location = "Kisumu, Kenya",
                YearsExperience = 6,
                LinkedinUrl = "https://linkedin.com/in/peterotieno",
                Availability = "one_month",
                CreatedAt = DateTime.Now.AddDays(-20)
            };

            var aminaProfile = new JobSeekerProfile
            {
                Id = Guid.NewGuid(),
                UserId = aminaUser.Id,
                Headline = "Digital Marketing & Content Strategist",
                Bio = "Creative digital marketer with 2 years of experience in SEO, social media management and content creation. Experienced with Google Analytics, Facebook Ads and email marketing campaigns.",
                Phone = "+254 745 678 901",
                Location = "Mombasa, Kenya",
                YearsExperience = 2,
                LinkedinUrl = "https://linkedin.com/in/aminahassan",
                Availability = "immediately",
                CreatedAt = DateTime.Now.AddDays(-15)
            };

            context.JobSeekerProfiles.AddRange(johnProfile, maryProfile, peterProfile, aminaProfile);
            await context.SaveChangesAsync();

            // ==================== SKILLS ====================
            var skills = new[]
            {
                new Skill { Id = Guid.NewGuid(), Name = "Python", Category = "Programming", CreatedAt = DateTime.Now },
                new Skill { Id = Guid.NewGuid(), Name = "React", Category = "Frontend", CreatedAt = DateTime.Now },
                new Skill { Id = Guid.NewGuid(), Name = "JavaScript", Category = "Programming", CreatedAt = DateTime.Now },
                new Skill { Id = Guid.NewGuid(), Name = "Java", Category = "Programming", CreatedAt = DateTime.Now },
                new Skill { Id = Guid.NewGuid(), Name = "SQL", Category = "Database", CreatedAt = DateTime.Now },
                new Skill { Id = Guid.NewGuid(), Name = "Data Analysis", Category = "Analytics", CreatedAt = DateTime.Now },
                new Skill { Id = Guid.NewGuid(), Name = "Project Management", Category = "Management", CreatedAt = DateTime.Now },
                new Skill { Id = Guid.NewGuid(), Name = "Digital Marketing", Category = "Marketing", CreatedAt = DateTime.Now },
                new Skill { Id = Guid.NewGuid(), Name = "Leadership", Category = "Soft Skills", CreatedAt = DateTime.Now },
                new Skill { Id = Guid.NewGuid(), Name = "Communication", Category = "Soft Skills", CreatedAt = DateTime.Now },
                new Skill { Id = Guid.NewGuid(), Name = "Node.js", Category = "Backend", CreatedAt = DateTime.Now },
                new Skill { Id = Guid.NewGuid(), Name = "AWS", Category = "Cloud", CreatedAt = DateTime.Now }
            };

            context.Skills.AddRange(skills);
            await context.SaveChangesAsync();

            var skillDict = skills.ToDictionary(s => s.Name, s => s.Id);

            // ==================== SEEKER SKILLS ====================
            var seekerSkills = new List<SeekerSkill>
            {
                // John: Python, React, JavaScript, Node.js, SQL, AWS
                new() { Id = Guid.NewGuid(), JobSeekerProfileId = johnProfile.Id, SkillId = skillDict["Python"], ProficiencyLevel = "advanced" },
                new() { Id = Guid.NewGuid(), JobSeekerProfileId = johnProfile.Id, SkillId = skillDict["React"], ProficiencyLevel = "advanced" },
                new() { Id = Guid.NewGuid(), JobSeekerProfileId = johnProfile.Id, SkillId = skillDict["JavaScript"], ProficiencyLevel = "expert" },
                new() { Id = Guid.NewGuid(), JobSeekerProfileId = johnProfile.Id, SkillId = skillDict["Node.js"], ProficiencyLevel = "advanced" },
                new() { Id = Guid.NewGuid(), JobSeekerProfileId = johnProfile.Id, SkillId = skillDict["SQL"], ProficiencyLevel = "intermediate" },
                new() { Id = Guid.NewGuid(), JobSeekerProfileId = johnProfile.Id, SkillId = skillDict["AWS"], ProficiencyLevel = "intermediate" },

                // Mary: Python, SQL, Data Analysis, Communication
                new() { Id = Guid.NewGuid(), JobSeekerProfileId = maryProfile.Id, SkillId = skillDict["Python"], ProficiencyLevel = "advanced" },
                new() { Id = Guid.NewGuid(), JobSeekerProfileId = maryProfile.Id, SkillId = skillDict["SQL"], ProficiencyLevel = "expert" },
                new() { Id = Guid.NewGuid(), JobSeekerProfileId = maryProfile.Id, SkillId = skillDict["Data Analysis"], ProficiencyLevel = "expert" },
                new() { Id = Guid.NewGuid(), JobSeekerProfileId = maryProfile.Id, SkillId = skillDict["Communication"], ProficiencyLevel = "advanced" },

                // Peter: Project Management, Leadership, Communication
                new() { Id = Guid.NewGuid(), JobSeekerProfileId = peterProfile.Id, SkillId = skillDict["Project Management"], ProficiencyLevel = "expert" },
                new() { Id = Guid.NewGuid(), JobSeekerProfileId = peterProfile.Id, SkillId = skillDict["Leadership"], ProficiencyLevel = "advanced" },
                new() { Id = Guid.NewGuid(), JobSeekerProfileId = peterProfile.Id, SkillId = skillDict["Communication"], ProficiencyLevel = "expert" },

                // Amina: Digital Marketing, Communication
                new() { Id = Guid.NewGuid(), JobSeekerProfileId = aminaProfile.Id, SkillId = skillDict["Digital Marketing"], ProficiencyLevel = "advanced" },
                new() { Id = Guid.NewGuid(), JobSeekerProfileId = aminaProfile.Id, SkillId = skillDict["Communication"], ProficiencyLevel = "advanced" },
            };

            context.SeekerSkills.AddRange(seekerSkills);
            await context.SaveChangesAsync();

            // ==================== JOBS ====================
            var job1 = new Job
            {
                Id = Guid.NewGuid(),
                EmployerId = safaricomProfile.Id,
                Title = "Senior Software Engineer",
                Description = "We are looking for a talented Senior Software Engineer to join our Digital & DevOps team. You will be responsible for designing and implementing scalable software solutions that power our mobile and web applications serving over 40 million customers.\n\nYou will work in an agile environment, collaborating with cross-functional teams to deliver high-quality software products. This is an exciting opportunity to work on Africa's leading technology company.",
                Requirements = "• 5+ years of software development experience\n• Proficiency in Python, JavaScript or Java\n• Experience with React or similar frontend frameworks\n• Strong understanding of cloud services (AWS/GCP)\n• Experience with microservices architecture\n• Bachelor's degree in Computer Science or related field\n• Excellent problem-solving skills",
                JobType = "full-time",
                Location = "Nairobi, Kenya",
                SalaryMin = 150000,
                SalaryMax = 250000,
                Currency = "KES",
                ExperienceYears = "5+ years",
                Status = "active",
                Deadline = DateTime.Now.AddDays(30),
                ViewsCount = 145,
                CreatedAt = DateTime.Now.AddDays(-10)
            };

            var job2 = new Job
            {
                Id = Guid.NewGuid(),
                EmployerId = equityProfile.Id,
                Title = "Data Analyst",
                Description = "Equity Bank is seeking a Data Analyst to join our Business Intelligence team. You will be responsible for analyzing large datasets to provide insights that drive strategic business decisions across our operations in East Africa.\n\nThe ideal candidate is comfortable working with complex data sets and can translate data findings into clear business narratives.",
                Requirements = "• 2-4 years of data analysis experience\n• Proficiency in Python and SQL\n• Experience with data visualization tools (Tableau, Power BI)\n• Strong analytical and statistical skills\n• Experience with Excel and data modeling\n• Degree in Statistics, Mathematics, Computer Science or related field",
                JobType = "full-time",
                Location = "Nairobi, Kenya",
                SalaryMin = 80000,
                SalaryMax = 130000,
                Currency = "KES",
                ExperienceYears = "2-4 years",
                Status = "active",
                Deadline = DateTime.Now.AddDays(21),
                ViewsCount = 89,
                CreatedAt = DateTime.Now.AddDays(-8)
            };

            var job3 = new Job
            {
                Id = Guid.NewGuid(),
                EmployerId = kcbProfile.Id,
                Title = "Senior Project Manager",
                Description = "KCB Group is looking for an experienced Senior Project Manager to lead strategic technology transformation projects. You will manage complex, multi-stakeholder projects across our operations in 7 countries.\n\nThis role requires strong leadership skills, excellent stakeholder management, and proven ability to deliver large-scale projects on time and within budget.",
                Requirements = "• 5+ years project management experience\n• PMP or PRINCE2 certification preferred\n• Experience in banking or financial services\n• Strong leadership and communication skills\n• Proficiency in project management tools (MS Project, Jira)\n• Degree in Business, IT or related field",
                JobType = "full-time",
                Location = "Nairobi, Kenya",
                SalaryMin = 180000,
                SalaryMax = 280000,
                Currency = "KES",
                ExperienceYears = "5+ years",
                Status = "active",
                Deadline = DateTime.Now.AddDays(25),
                ViewsCount = 67,
                CreatedAt = DateTime.Now.AddDays(-7)
            };

            var job4 = new Job
            {
                Id = Guid.NewGuid(),
                EmployerId = safaricomProfile.Id,
                Title = "Junior Software Developer",
                Description = "Are you a fresh graduate or early-career developer looking to kick-start your career? Safaricom is offering an exciting opportunity for Junior Software Developers to join our growing engineering team.\n\nYou will work alongside experienced engineers, learning best practices while contributing to real products used by millions of Kenyans.",
                Requirements = "• 0-2 years of software development experience\n• Proficiency in at least one programming language (Java, Python, JavaScript)\n• Understanding of web technologies (HTML, CSS, JavaScript)\n• Familiarity with version control (Git)\n• Degree in Computer Science, Software Engineering or related field\n• Strong willingness to learn",
                JobType = "full-time",
                Location = "Nairobi, Kenya",
                SalaryMin = 60000,
                SalaryMax = 90000,
                Currency = "KES",
                ExperienceYears = "0-2 years",
                Status = "active",
                Deadline = DateTime.Now.AddDays(35),
                ViewsCount = 210,
                CreatedAt = DateTime.Now.AddDays(-6)
            };

            var job5 = new Job
            {
                Id = Guid.NewGuid(),
                EmployerId = nationProfile.Id,
                Title = "Digital Marketing Specialist",
                Description = "Nation Media Group is seeking a creative and data-driven Digital Marketing Specialist to manage our social media presence, email marketing campaigns, and digital advertising across all our media brands.\n\nYou will develop and execute digital marketing strategies that grow our audience and drive subscriptions to our digital platforms.",
                Requirements = "• 2+ years of digital marketing experience\n• Experience with social media management (Facebook, Twitter, Instagram)\n• Proficiency in Google Analytics and Facebook Ads Manager\n• SEO/SEM experience\n• Excellent written and verbal communication skills\n• Degree in Marketing, Communications or related field",
                JobType = "full-time",
                Location = "Nairobi, Kenya",
                SalaryMin = 70000,
                SalaryMax = 110000,
                Currency = "KES",
                ExperienceYears = "2+ years",
                Status = "active",
                Deadline = DateTime.Now.AddDays(20),
                ViewsCount = 98,
                CreatedAt = DateTime.Now.AddDays(-5)
            };

            var job6 = new Job
            {
                Id = Guid.NewGuid(),
                EmployerId = equityProfile.Id,
                Title = "Backend Java Developer",
                Description = "Join Equity Bank's technology team in Mombasa as a Backend Java Developer. You will design and build RESTful APIs and microservices that power our core banking system and mobile banking applications.\n\nThis is an opportunity to work on high-impact financial technology that serves millions of customers across East Africa.",
                Requirements = "• 3+ years of Java development experience\n• Experience with Spring Boot and REST API development\n• Strong understanding of SQL and database design\n• Experience with microservices and Docker\n• Knowledge of banking systems is an advantage\n• Degree in Computer Science or related field",
                JobType = "full-time",
                Location = "Mombasa, Kenya",
                SalaryMin = 100000,
                SalaryMax = 160000,
                Currency = "KES",
                ExperienceYears = "3+ years",
                Status = "active",
                Deadline = DateTime.Now.AddDays(28),
                ViewsCount = 54,
                CreatedAt = DateTime.Now.AddDays(-4)
            };

            var job7 = new Job
            {
                Id = Guid.NewGuid(),
                EmployerId = kcbProfile.Id,
                Title = "Business Analyst",
                Description = "KCB Group Kisumu Branch is looking for a Business Analyst to bridge the gap between business needs and technology solutions. You will analyze business processes, gather requirements, and work with development teams to implement solutions.\n\nThis role is ideal for someone who combines strong analytical skills with excellent communication abilities.",
                Requirements = "• 3+ years of business analysis experience\n• Strong data analysis and reporting skills\n• Experience with process mapping and documentation\n• Project management experience preferred\n• Knowledge of banking operations is advantageous\n• Degree in Business, IT or related field",
                JobType = "full-time",
                Location = "Kisumu, Kenya",
                SalaryMin = 90000,
                SalaryMax = 140000,
                Currency = "KES",
                ExperienceYears = "3+ years",
                Status = "active",
                Deadline = DateTime.Now.AddDays(18),
                ViewsCount = 41,
                CreatedAt = DateTime.Now.AddDays(-3)
            };

            var job8 = new Job
            {
                Id = Guid.NewGuid(),
                EmployerId = safaricomProfile.Id,
                Title = "Frontend React Developer",
                Description = "Safaricom is hiring a Frontend React Developer to work on our customer-facing web applications. You will build responsive, high-performance user interfaces that deliver exceptional experiences to our millions of users.\n\nThis is a remote-friendly position with occasional visits to our Nairobi offices.",
                Requirements = "• 3+ years of React development experience\n• Expert-level JavaScript/TypeScript skills\n• Experience with state management (Redux, Context API)\n• Understanding of web performance optimization\n• Experience with REST APIs and GraphQL\n• Strong eye for UI/UX design",
                JobType = "remote",
                Location = "Nairobi, Kenya (Remote)",
                SalaryMin = 120000,
                SalaryMax = 190000,
                Currency = "KES",
                ExperienceYears = "3+ years",
                Status = "active",
                Deadline = DateTime.Now.AddDays(22),
                ViewsCount = 178,
                CreatedAt = DateTime.Now.AddDays(-2)
            };

            var job9 = new Job
            {
                Id = Guid.NewGuid(),
                EmployerId = kcbProfile.Id,
                Title = "IT Project Manager - Contract",
                Description = "KCB Group is seeking an experienced IT Project Manager for a 6-month contract to lead our core banking system upgrade project. You will coordinate with vendors, internal teams, and stakeholders to ensure successful delivery.\n\nThis is a high-visibility project with significant impact on our operations across East Africa.",
                Requirements = "• 4+ years of IT project management experience\n• PMP certification preferred\n• Experience with banking system implementations\n• Strong vendor management skills\n• Excellent risk management capabilities\n• Available to start within 2 weeks",
                JobType = "contract",
                Location = "Nairobi, Kenya",
                SalaryMin = 200000,
                SalaryMax = 300000,
                Currency = "KES",
                ExperienceYears = "4+ years",
                Status = "active",
                Deadline = DateTime.Now.AddDays(14),
                ViewsCount = 63,
                CreatedAt = DateTime.Now.AddDays(-1)
            };

            var job10 = new Job
            {
                Id = Guid.NewGuid(),
                EmployerId = nationProfile.Id,
                Title = "Social Media Content Creator - Part Time",
                Description = "Nation Media Group is looking for a creative Content Creator to produce engaging content for our social media platforms. This is a part-time role suitable for students or professionals looking for flexible work.\n\nYou will create written, visual and video content for our brands including Nation, Business Daily and NTV.",
                Requirements = "• Experience creating social media content\n• Strong writing and communication skills\n• Basic knowledge of graphic design tools (Canva, Adobe)\n• Understanding of social media analytics\n• Passion for media and storytelling\n• Available for 20 hours per week",
                JobType = "part-time",
                Location = "Nairobi, Kenya",
                SalaryMin = 30000,
                SalaryMax = 50000,
                Currency = "KES",
                ExperienceYears = "0-1 years",
                Status = "active",
                Deadline = DateTime.Now.AddDays(40),
                ViewsCount = 127,
                CreatedAt = DateTime.Now
            };

            context.Jobs.AddRange(job1, job2, job3, job4, job5, job6, job7, job8, job9, job10);
            await context.SaveChangesAsync();

            // ==================== JOB SKILLS ====================
            var jobSkillData = new List<(Guid JobId, string[] Skills)>
            {
                (job1.Id, new[] { "Python", "JavaScript", "React", "Node.js", "AWS" }),
                (job2.Id, new[] { "Python", "SQL", "Data Analysis" }),
                (job3.Id, new[] { "Project Management", "Leadership", "Communication" }),
                (job4.Id, new[] { "Java", "JavaScript", "SQL" }),
                (job5.Id, new[] { "Digital Marketing", "Communication" }),
                (job6.Id, new[] { "Java", "SQL" }),
                (job7.Id, new[] { "Data Analysis", "Project Management", "Communication" }),
                (job8.Id, new[] { "React", "JavaScript" }),
                (job9.Id, new[] { "Project Management", "Leadership" }),
                (job10.Id, new[] { "Digital Marketing", "Communication" })
            };

            foreach (var (jobId, skillNames) in jobSkillData)
            {
                foreach (var skillName in skillNames)
                {
                    if (skillDict.TryGetValue(skillName, out var skillId))
                    {
                        context.JobSkills.Add(new JobSkill
                        {
                            Id = Guid.NewGuid(),
                            JobId = jobId,
                            SkillId = skillId
                        });
                    }
                }
            }

            await context.SaveChangesAsync();

            // ==================== APPLICATIONS ====================
            var applications = new List<Application>
            {
                // John applied to Senior Software Engineer @ Safaricom
                new()
                {
                    Id = Guid.NewGuid(),
                    JobId = job1.Id,
                    JobSeekerProfileId = johnProfile.Id,
                    CoverLetter = "Dear Hiring Manager,\n\nI am excited to apply for the Senior Software Engineer position at Safaricom PLC. With 4 years of experience building scalable web applications using Python, React, and Node.js, I believe I am well-qualified for this role.\n\nAt my previous role, I led the development of a payment processing system that handled over 100,000 transactions daily. I have strong experience with microservices architecture and cloud services, particularly AWS.\n\nI am passionate about building technology that makes a real difference in people's lives, and Safaricom's mission to transform lives through innovation resonates deeply with me.\n\nI would welcome the opportunity to discuss how my skills and experience can contribute to your team.\n\nSincerely,\nJohn Mwangi",
                    Status = "reviewed",
                    EmployerNotes = "Strong technical background. Good match for the role. Schedule technical interview.",
                    AppliedAt = DateTime.Now.AddDays(-7),
                    UpdatedAt = DateTime.Now.AddDays(-3)
                },
                // John applied to Frontend React Developer @ Safaricom
                new()
                {
                    Id = Guid.NewGuid(),
                    JobId = job8.Id,
                    JobSeekerProfileId = johnProfile.Id,
                    CoverLetter = "Dear Hiring Manager,\n\nI am applying for the Frontend React Developer position at Safaricom. As an expert JavaScript developer with advanced React skills, I am confident I can make an immediate contribution to your frontend team.\n\nI have built numerous complex React applications including dashboards, e-commerce platforms, and real-time data visualization tools. I have experience with Redux, Context API, and performance optimization techniques.\n\nThe opportunity to work on products used by millions of Kenyans is tremendously exciting, and I would bring enthusiasm and technical excellence to this role.\n\nBest regards,\nJohn Mwangi",
                    Status = "shortlisted",
                    EmployerNotes = "Excellent React skills. Top candidate. Proceed to final interview.",
                    AppliedAt = DateTime.Now.AddDays(-5),
                    UpdatedAt = DateTime.Now.AddDays(-1)
                },
                // Mary applied to Data Analyst @ Equity Bank
                new()
                {
                    Id = Guid.NewGuid(),
                    JobId = job2.Id,
                    JobSeekerProfileId = maryProfile.Id,
                    CoverLetter = "Dear Hiring Team,\n\nI am delighted to apply for the Data Analyst position at Equity Bank. With 3 years of experience in data analysis and business intelligence, and expert-level proficiency in Python, SQL, and data visualization, I am well-positioned to contribute to your BI team.\n\nIn my current role, I have developed automated reporting dashboards that reduced manual reporting time by 70% and identified fraud patterns that saved the organization significant funds. I am passionate about turning data into actionable insights that drive business growth.\n\nEquity Bank's pan-African reach and commitment to financial inclusion make it an organization I would be proud to work for.\n\nWarm regards,\nMary Wambua",
                    Status = "shortlisted",
                    EmployerNotes = "Excellent analytical skills. Very impressed with her portfolio. Moving to final round.",
                    AppliedAt = DateTime.Now.AddDays(-6),
                    UpdatedAt = DateTime.Now.AddDays(-2)
                },
                // Peter applied to Senior Project Manager @ KCB
                new()
                {
                    Id = Guid.NewGuid(),
                    JobId = job3.Id,
                    JobSeekerProfileId = peterProfile.Id,
                    CoverLetter = "Dear Hiring Manager,\n\nI am writing to express my strong interest in the Senior Project Manager position at KCB Group. With 6 years of project management experience and PMP certification, I have consistently delivered complex projects on time and within budget.\n\nI have managed projects with budgets exceeding KES 50 million, coordinated teams of 20+ people across multiple locations, and maintained stakeholder satisfaction throughout. My experience in the financial services sector gives me valuable context for working within KCB's environment.\n\nI am excited about the opportunity to contribute to KCB Group's continued growth and digital transformation journey.\n\nKind regards,\nPeter Otieno",
                    Status = "pending",
                    AppliedAt = DateTime.Now.AddDays(-4),
                    UpdatedAt = DateTime.Now.AddDays(-4)
                },
                // Amina applied to Digital Marketing Specialist @ Nation Media
                new()
                {
                    Id = Guid.NewGuid(),
                    JobId = job5.Id,
                    JobSeekerProfileId = aminaProfile.Id,
                    CoverLetter = "Dear Hiring Team,\n\nI am thrilled to apply for the Digital Marketing Specialist position at Nation Media Group. As a creative and data-driven digital marketer with 2 years of experience, I have a proven track record of growing social media followings and driving digital engagement.\n\nI have managed social media accounts with combined followings of over 50,000 followers, run successful Google and Facebook ad campaigns with ROI of 300%+, and developed content strategies that increased website traffic by 45%.\n\nNation Media Group's legacy in East African journalism and its growing digital presence make it the perfect environment for me to apply and grow my skills.\n\nBest regards,\nAmina Hassan",
                    Status = "pending",
                    AppliedAt = DateTime.Now.AddDays(-3),
                    UpdatedAt = DateTime.Now.AddDays(-3)
                },
                // Mary applied to Business Analyst @ KCB
                new()
                {
                    Id = Guid.NewGuid(),
                    JobId = job7.Id,
                    JobSeekerProfileId = maryProfile.Id,
                    CoverLetter = "Dear Hiring Team,\n\nI am applying for the Business Analyst position at KCB Group in Kisumu. My background in data analysis, combined with strong project management skills, makes me an excellent fit for bridging business needs with technology solutions.\n\nI have extensive experience in process analysis, requirements gathering, and data-driven reporting. I am comfortable presenting complex findings to both technical and non-technical stakeholders.\n\nI am originally from Kisumu and would welcome the opportunity to return home while contributing to KCB Group's regional growth.\n\nRegards,\nMary Wambua",
                    Status = "pending",
                    AppliedAt = DateTime.Now.AddDays(-2),
                    UpdatedAt = DateTime.Now.AddDays(-2)
                }
            };

            context.Applications.AddRange(applications);
            await context.SaveChangesAsync();

            // ==================== SAVED JOBS ====================
            var savedJobs = new List<SavedJob>
            {
                new() { Id = Guid.NewGuid(), JobSeekerProfileId = johnProfile.Id, JobId = job6.Id, SavedAt = DateTime.Now.AddDays(-5) },
                new() { Id = Guid.NewGuid(), JobSeekerProfileId = johnProfile.Id, JobId = job9.Id, SavedAt = DateTime.Now.AddDays(-3) },
                new() { Id = Guid.NewGuid(), JobSeekerProfileId = maryProfile.Id, JobId = job3.Id, SavedAt = DateTime.Now.AddDays(-4) },
                new() { Id = Guid.NewGuid(), JobSeekerProfileId = aminaProfile.Id, JobId = job10.Id, SavedAt = DateTime.Now.AddDays(-2) },
            };

            context.SavedJobs.AddRange(savedJobs);
            await context.SaveChangesAsync();
        }
    }
}
