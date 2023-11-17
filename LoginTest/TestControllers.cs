using LoginManager.Models;
using LoginServer.Controllers;
using LoginServer.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Security.Claims;

namespace LoginTest
{
    internal class TestControllers
    {
        [Test]
        public async Task Test_GetUserToken()
        {
            // Sets up the usage of an in-memory DB rather than any sort of SQL.
            var dbContextOptions = new DbContextOptionsBuilder<UserInteraction>()
                .UseInMemoryDatabase(databaseName: "InMemoryDatabase")
                .Options;

            // Generates a mock IConfiguration for the token key.
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(x => x.GetSection("AppSettings:Token")
                .Value).Returns("A longer key used in testing this random stuff");

            // Checks if a DB exists and ensures it is populated with the test user.
            using (var context = new UserInteraction(dbContextOptions))
            {
                context.Database.EnsureCreated();

                var testUser = new User { Username = "TestUser", PasswordHash = "123" };
                context.UserInfo.Add(testUser);
                context.SaveChanges();
            }

            // Sets up the user with the in-memory DB.
            var serviceProvider = new ServiceCollection()
                .AddDbContext<UserInteraction>(options => options.UseInMemoryDatabase("InMemoryDatabase"))
                .BuildServiceProvider();

            // Generates a few more controller mocks for testing.
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var mockControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
            };

            var userController = new UsersController(serviceProvider.GetRequiredService<UserInteraction>(), mockConfiguration.Object)
            {
                ControllerContext = mockControllerContext
            };

            // Tests if it is possible to get the user's token.
            var result = await userController.GetUserToken("TestUser", "123");

            // Check that we do, in fact, get the user's token.
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
        }
    }
}
