using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FluentValidation;
using FluentValidation.TestHelper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using WebApi.BLL.Exceptions;
using WebApi.BLL.Interfaces;
using WebApi.BLL.Models;
using WebApi.BLL.Services;
using WebApi.BLL.Validators;
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

        private static async Task<DbContextOptions<ApplicationContext>> GetUnitTestDbContextOptionsAsync()
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
            var context = new ApplicationContext(await GetUnitTestDbContextOptionsAsync());
            var expected = await context.Users.FirstAsync(user => user.UserName == name);
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var userLoginValidatorMock = new Mock<IValidator<UserLoginModel>>();

            unitOfWorkMock
                .Setup(unitOfWork => unitOfWork.UserRepository.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(expected);
            unitOfWorkMock
                .Setup(unitOfWork => unitOfWork.UserRepository.CheckPasswordAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
                .ReturnsAsync(true);
            unitOfWorkMock
                .Setup(unitOfWork => unitOfWork.UserRepository.GetRolesAsync(It.IsAny<IdentityUser>()))
                .ReturnsAsync(new List<string>() { "User" });

            var jwtCreationService = new JwtCreationService(configuration);

            var authService = new AuthService(configuration, jwtCreationService, userLoginValidatorMock.Object, null, unitOfWorkMock.Object);

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
            var context = new ApplicationContext(await GetUnitTestDbContextOptionsAsync());
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

        [Test]
        public void AuthService_LoginAsync_ThrowsValidationExceptionIfUsernameIsEmpty()
        {
            //Arrange
            var userLoginValidator = new UserLoginValidator();

            var userLogin = new UserLoginModel()
            {
                UserName = String.Empty,
                Password = "password"
            };

            //Act
            var validationTest = userLoginValidator.TestValidate(userLogin);

            //Assert
            validationTest.ShouldHaveValidationErrorFor(user => user.UserName);
        }

        [Test]
        public void AuthService_LoginAsync_ThrowsValidationExceptionIfUsernameIsNull()
        {
            //Arrange
            var userLoginValidator = new UserLoginValidator();

            var userLogin = new UserLoginModel()
            {
                UserName = null,
                Password = "password"
            };

            //Act
            var validationTest = userLoginValidator.TestValidate(userLogin);

            //Assert
            validationTest.ShouldHaveValidationErrorFor(user => user.UserName);
        }

        [Test]
        public void AuthService_LoginAsync_ThrowsValidationExceptionIfUsernameIsLessThan3Symbols()
        {
            //Arrange
            var userLoginValidator = new UserLoginValidator();

            var userLogin = new UserLoginModel()
            {
                UserName = "12",
                Password = "password"
            };

            //Act
            var validationTest = userLoginValidator.TestValidate(userLogin);

            //Assert
            validationTest.ShouldHaveValidationErrorFor(user => user.UserName);
        }

        [Test]
        public void AuthService_LoginAsync_ThrowsValidationExceptionIfUsernameIsMoreThan20()
        {
            //Arrange
            var userLoginValidator = new UserLoginValidator();

            var userLogin = new UserLoginModel()
            {
                UserName = "123456789123456789123456789",
                Password = "password"
            };

            //Act
            var validationTest = userLoginValidator.TestValidate(userLogin);

            //Assert
            validationTest.ShouldHaveValidationErrorFor(user => user.UserName);
        }

        [Test]
        public void AuthService_LoginAsync_ThrowsValidationExceptionIfPasswordIsEmpty()
        {
            //Arrange
            var userLoginValidator = new UserLoginValidator();

            var userLogin = new UserLoginModel()
            {
                UserName = "12345678",
                Password = String.Empty,
            };

            //Act
            var validationTest = userLoginValidator.TestValidate(userLogin);

            //Assert
            validationTest.ShouldHaveValidationErrorFor(user => user.Password);
        }

        [Test]
        public void AuthService_LoginAsync_ThrowsValidationExceptionIfPasswordIsNull()
        {
            //Arrange
            var userLoginValidator = new UserLoginValidator();

            var userLogin = new UserLoginModel()
            {
                UserName = "12345678",
                Password = null,
            };

            //Act
            var validationTest = userLoginValidator.TestValidate(userLogin);

            //Assert
            validationTest.ShouldHaveValidationErrorFor(user => user.Password);
        }

        [Test]
        [TestCase("UserNew1", "email@email.com", "password")]
        [TestCase("UserNew2", "email1@email.com", "password")]
        public async Task AuthService_RegisterAsync_ReturnsSuccesMessage(string name, string email, string password)
        {
            //Arrange
            var context = new ApplicationContext(await GetUnitTestDbContextOptionsAsync());
            var expected = new MessageDTO() { Message = "User created successfully!" };
            var userStoreMock = new Mock<IUserStore<IdentityUser>>();
            var userManagerMock = new Mock<UserManager<IdentityUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
            var roleStoreMock = new Mock<IRoleStore<IdentityRole>>();
            var roleManagerMock = new Mock<RoleManager<IdentityRole>>(roleStoreMock.Object, null, null, null, null);
            var unitOfWorkMock = new Mock<UnitOfWork>(context, roleManagerMock.Object, userManagerMock.Object);
            var userLoginValidatorMock = new Mock<IValidator<UserLoginModel>>();
            var userRegisterValidatorMock = new Mock<IValidator<UserRegisterModel>>();

            var jwtCreationService = new JwtCreationService(configuration);

            var authService = new AuthService(configuration, jwtCreationService, userLoginValidatorMock.Object, userRegisterValidatorMock.Object, unitOfWorkMock.Object);

            var userRegister = new UserRegisterModel()
            {
                UserName = name,
                Email = email,
                Password = password
            };

            userManagerMock.Setup(userManager => userManager.CreateAsync(It.IsAny<IdentityUser>(), password)).ReturnsAsync(IdentityResult.Success);
            roleManagerMock.Setup(roleManager => roleManager.RoleExistsAsync(It.IsAny<string>())).ReturnsAsync(true);
            //Act
            var message = await authService.RegisterAsync(userRegister);

            //Assert
            message.Should().BeEquivalentTo(expected);
        }

        [Test]
        [TestCase("UserNew1", "email@email.com", "password")]
        [TestCase("UserNew2", "email1@email.com", "password")]
        public async Task AuthService_RegisterAsync_ThrowsAuthentificationExceptionIfThereIsUserWithSameUsername(string name, string email, string password)
        {
            //Arrange
            var context = new ApplicationContext(await GetUnitTestDbContextOptionsAsync());
            var userStoreMock = new Mock<IUserStore<IdentityUser>>();
            var userManagerMock = new Mock<UserManager<IdentityUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
            var roleStoreMock = new Mock<IRoleStore<IdentityRole>>();
            var roleManagerMock = new Mock<RoleManager<IdentityRole>>(roleStoreMock.Object, null, null, null, null);
            var unitOfWorkMock = new Mock<UnitOfWork>(context, roleManagerMock.Object, userManagerMock.Object);
            var userLoginValidatorMock = new Mock<IValidator<UserLoginModel>>();
            var userRegisterValidatorMock = new Mock<IValidator<UserRegisterModel>>();

            var jwtCreationService = new JwtCreationService(configuration);

            var authService = new AuthService(configuration, jwtCreationService, userLoginValidatorMock.Object, userRegisterValidatorMock.Object, unitOfWorkMock.Object);

            var userRegister = new UserRegisterModel()
            {
                UserName = name,
                Email = email,
                Password = password
            };

            var identityUser = new IdentityUser(name);

            userManagerMock.Setup(userManager => userManager.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(identityUser);

            //Act
            Func<Task> registerAsync = async () => await authService.RegisterAsync(userRegister);

            //Assert
            await registerAsync.Should().ThrowAsync<AuthentificationException>().WithMessage("User already exists");
        }
    }
}