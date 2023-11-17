using LoginManager.Models;
using Microsoft.EntityFrameworkCore;

namespace LoginServer.Services
{
    public class UserInteraction : DbContext
    {
        public UserInteraction(DbContextOptions<UserInteraction> options) : base(options)
        {
        }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!IsUnitTest())
                optionsBuilder.UseSqlite("Data Source = User.db");
        }

        private bool IsUnitTest()
        {
            bool value = AppDomain.CurrentDomain.GetAssemblies()?
                .Any(a => a.FullName != null && a.FullName.ToLowerInvariant()
                .StartsWith("nunit.framework")) ?? false;

            return value;
        }

        public DbSet<User> UserInfo { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>().Property(c => c.Username).ValueGeneratedOnAdd();
            modelBuilder.Entity<User>().HasData(
                new User() { Username = "Misha", PasswordHash = "123" },
                new User() { Username = "Tester", PasswordHash = "Supertest" }
                );
        }
    }
}
