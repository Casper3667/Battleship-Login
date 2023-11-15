using LoginManager.Controllers;
using LoginManager.Models;

namespace LoginServer
{
    public class DebugSetup
    {
        internal static void Setup()
        {
            UserDto account = new UserDto();
            account.Username = "misha";
            account.PasswordHash = "123";
            AccountController.AccountList.Add("misha", account);
        }
    }
}
