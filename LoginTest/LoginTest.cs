using LoginManager.Controllers;
using LoginManager.Models;
using LoginServer;

namespace LoginTest
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
            DebugSetup.Setup();
        }

        [Test]
        public void AccessUserTest()
        {
            string expectedUsername = "misha";
            string expectedPassword = "123";


            Assert.IsTrue(AccountController.AccountList.ContainsKey(expectedUsername), "The test user was not on the list.");
            UserDto user = AccountController.AccountList[expectedUsername];

            Assert.IsTrue(user.Username == expectedUsername, "The test user's username is incorrect.");
            Assert.IsTrue(user.PasswordHash == expectedPassword, "The test user's password was not the expected password.");
        }
    }
}