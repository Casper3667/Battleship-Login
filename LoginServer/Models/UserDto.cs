namespace LoginManager.Models
{
    public class UserDto
    {
        public string Username { get; set; } = string.Empty;
        public string EncryptionHash { get; set; } = string.Empty;
    }
}
