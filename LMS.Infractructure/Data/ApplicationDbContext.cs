using Domain.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infractructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }
        public DbSet<Course> Courses => Set<Course>();
        public DbSet<Module> Modules => Set<Module>();
        public DbSet<ModuleActivity> Activities => Set<ModuleActivity>();
        public DbSet<ActivityType> ActivityTypes => Set<ActivityType>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>()
                .HasOne(u => u.Course)
                .WithMany(c => c.Students)
                .HasForeignKey(u => u.CourseId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<Module>()
                .HasOne(m => m.Course)
                .WithMany(c => c.Modules)
                .HasForeignKey(m => m.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ModuleActivity>()
                .HasOne(a => a.Module)
                .WithMany(m => m.Activities)
                .HasForeignKey(a => a.ModuleId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ModuleActivity>()
                .HasOne(a => a.Type)
                .WithMany(t => t.Activities)
                .HasForeignKey(a => a.TypeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
