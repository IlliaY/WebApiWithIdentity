using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using WebApi.BLL.Exceptions;
using WebApi.BLL.Interfaces;
using WebApi.BLL.Models;
using WebApi.BLL.Services;
using WebApi.DAL.Data;
using WebApi.DAL.Interfaces;

namespace WebApi.Tests
{
    public class Tests
    {
        private IConfiguration configuration;

        [SetUp]
        public void Setup()
        {
            var inMemorySettings = new Dictionary<string, string>
            {
                { "ConnectionString", "Data Source=:memory:" },
                { "JWT:SecretKey", "sdknvpajkhfijsaoifjdslkfnmop@@@Ojidsfopajiu" },
                { "jwt:Issuer", "http://localhost:5000" },
                { "jwt:Audience", "http://localhost:5000" }
            };

            configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();
        }

        private static async Task<DbContextOptions<ApplicationContext>> GetUnitTestDbContextOptions()
        {
            var options = new DbContextOptionsBuilder<ApplicationContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using (var context = new ApplicationContext(options))
            {
                await context.Users.AddAsync(new IdentityUser("User1"));
                await context.Users.AddAsync(new IdentityUser("User2"));
                await context.SaveChangesAsync();
                var user = await context.Users.FirstOrDefaultAsync(user => user.UserName == "User1");
                user.PasswordHash = "password";
                var user2 = await context.Users.FirstOrDefaultAsync(user => user.UserName == "User2");
                user2.PasswordHash = "password";
                await context.SaveChangesAsync();
            }

            return options;
        }

        [Test]
        [TestCase("User1")]
        [TestCase("User2")]
        public async Task AuthService_LoginAsync_ReturnsToken(string name)
        {
            //Arrange
            var context = new ApplicationContext(await GetUnitTestDbContextOptions());
            var expected = await context.Users.FirstAsync(user => user.UserName == name);
            var userStoreMock = new Mock<IUserStore<IdentityUser>>();
            var userManagerMock = new Mock<UserManager<IdentityUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
            userManagerMock.Setup(userManager => userManager.FindByNameAsync(name)).ReturnsAsync(expected);
            userManagerMock.Setup(userManager => userManager.CheckPasswordAsync(expected, expected.PasswordHash)).ReturnsAsync(true);
            userManagerMock.Setup(userManager => userManager.GetRolesAsync(expected)).ReturnsAsync(new List<string>() { "User" });
            var unitOfWorkMock = new Mock<UnitOfWork>(context, null, userManagerMock.Object);
            var userLoginValidatorMock = new Mock<IValidator<UserLoginModel>>();
            var userRegisterValidatorMock = new Mock<IValidator<UserRegisterModel>>();

            var jwtCreationService = new JwtCreationService(configuration);

            var authService = new AuthService(configuration, jwtCreationService, userLoginValidatorMock.Object, userRegisterValidatorMock.Object, unitOfWorkMock.Object);

            var userLogin = new UserLoginModel()
            {
                UserName = name,
                Password = "password"
            };
            //Act
            var token = await authService.LoginAsync(userLogin);

            //Assert    
            token.Should().NotBeNull();
        }

        [Test]
        [TestCase("UserThatDoesn'tExist")]
        [TestCase("UserThatDoesn'tExist2")]
        public async Task AuthService_LoginAsync_ThrowsAuthorizationExceptionIfThereIsNoUserWithInsertedUsername(string name)
        {
            //Arrange
            var context = new ApplicationContext(await GetUnitTestDbContextOptions());
            var userStoreMock = new Mock<IUserStore<IdentityUser>>();
            var userManagerMock = new Mock<UserManager<IdentityUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
            var unitOfWorkMock = new Mock<UnitOfWork>(context, null, userManagerMock.Object);
            var userLoginValidatorMock = new Mock<IValidator<UserLoginModel>>();
            var userRegisterValidatorMock = new Mock<IValidator<UserRegisterModel>>();

            var jwtCreationService = new JwtCreationService(configuration);

            var authService = new AuthService(configuration, jwtCreationService, userLoginValidatorMock.Object, userRegisterValidatorMock.Object, unitOfWorkMock.Object);

            var userLogin = new UserLoginModel()
            {
                UserName = name,
                Password = "password"
            };

            //Act
            Func<Task> loginAsync = async () => await authService.LoginAsync(userLogin);

            //Assert
            await loginAsync.Should().ThrowAsync<AuthentificationException>().WithMessage("Wrong username or password");
        }
    }
}