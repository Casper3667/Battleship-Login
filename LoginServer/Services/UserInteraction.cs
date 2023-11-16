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
            //base.OnConfiguring(optionsBuilder);
            //optionsBuilder.UseSqlite("Data Source = User.db");
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
