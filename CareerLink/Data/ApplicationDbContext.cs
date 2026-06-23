using Microsoft.EntityFrameworkCore;
using CareerLink.Models;

namespace CareerLink.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<JobSeekerProfile> JobSeekerProfiles { get; set; }
        public DbSet<EmployerProfile> EmployerProfiles { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<JobSkill> JobSkills { get; set; }
        public DbSet<SeekerSkill> SeekerSkills { get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<SavedJob> SavedJobs { get; set; }
        public DbSet<AdminLog> AdminLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Unique email constraint
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // One-to-one: User <-> JobSeekerProfile
            modelBuilder.Entity<JobSeekerProfile>()
                .HasIndex(p => p.UserId)
                .IsUnique();

            // One-to-one: User <-> EmployerProfile
            modelBuilder.Entity<EmployerProfile>()
                .HasIndex(p => p.UserId)
                .IsUnique();

            // Prevent duplicate applications (same seeker + same job)
            modelBuilder.Entity<Application>()
                .HasIndex(a => new { a.JobSeekerProfileId, a.JobId })
                .IsUnique();

            // Prevent duplicate saved jobs
            modelBuilder.Entity<SavedJob>()
                .HasIndex(s => new { s.JobSeekerProfileId, s.JobId })
                .IsUnique();

            // Prevent duplicate job-skill pairs
            modelBuilder.Entity<JobSkill>()
                .HasIndex(js => new { js.JobId, js.SkillId })
                .IsUnique();

            // Prevent duplicate seeker-skill pairs
            modelBuilder.Entity<SeekerSkill>()
                .HasIndex(ss => new { ss.JobSeekerProfileId, ss.SkillId })
                .IsUnique();

            // Unique skill names
            modelBuilder.Entity<Skill>()
                .HasIndex(s => s.Name)
                .IsUnique();
        }
    }
}
