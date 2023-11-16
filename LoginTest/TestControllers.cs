using LoginManager.Models;
using LoginServer.Controllers;
using LoginServer.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace LoginTest
{
    internal class TestControllers
    {
        [Test]
        public async Task GetUserToken_ValidCredentials_ReturnsOkObjectResult()
        {
            // Arrange
            var dbContextOptions = new DbContextOptionsBuilder<UserInteraction>()
                .UseInMemoryDatabase(databaseName: "InMemoryDatabase")
                .Options;

            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(x => x.GetSection("AppSettings:Token").Value).Returns("The secrets to make a token key is to do this");

            using (var context = new UserInteraction(dbContextOptions))
            {
                context.Database.EnsureCreated();

                var testUser = new User { Username = "testuser", PasswordHash = "testpasswordhash" };
                context.UserInfo.Add(testUser);
                context.SaveChanges();
            }

            var serviceProvider = new ServiceCollection()
                .AddDbContext<UserInteraction>(options => options.UseInMemoryDatabase("InMemoryDatabase"))
                .BuildServiceProvider();

            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var mockControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
            };

            var userController = new UsersController(serviceProvider.GetRequiredService<UserInteraction>(), mockConfiguration.Object)
            {
                ControllerContext = mockControllerContext
            };

            // Act
            var result = await userController.GetUserToken("testuser", "testpasswordhash");

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
        }
    }
}
