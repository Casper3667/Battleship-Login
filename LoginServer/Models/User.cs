using System.ComponentModel.DataAnnotations;

namespace LoginManager.Models
{
    public class User
    {
        //public int Id { get; set; }
        [Key]
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;

        public DateTime AccountCreation { get; set; }

        public User()
        {
            AccountCreation = DateTime.UtcNow;
        }
    }
}