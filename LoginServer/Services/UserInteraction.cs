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
                new User() { Username = "Misha", PasswordHash = "123" ,AccountCreation=DateTime.Now},
                new User() { Username = "Tester", PasswordHash = "Supertest", AccountCreation = DateTime.Now },
                new User() { Username = "Username", PasswordHash = "DC647EB65E6711E155375218212B3964", AccountCreation = DateTime.Now }, // The Password Hash is the hash you get from: Password (Case sensitive)
                new User() { Username = "Casper", PasswordHash = "E864A59D5D3FB7D439DD4DDD4797AB22", AccountCreation = DateTime.Now },
                new User() { Username = "Sofie", PasswordHash = "D8C5EBEDCA49B89FE1A845B536CADFE2", AccountCreation = DateTime.Now }
                );
        }
    }
}
