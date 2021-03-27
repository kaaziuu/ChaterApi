using System.Threading.Tasks;
using Chater.Controllers;
using Chater.Dtos.User.From;
using Chater.Models;
using Chater.Repository.Abstract;
using Chater.Service.Abstract;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace UnitTest.Controllers
{
    public class UserControllerTest
    {
        private readonly Mock<IUserRepository> _userRepositoryStub = new();
        private readonly Mock<IIdentityService> _identityServiceStub = new();
        [Fact]
        public async Task CreateAccount_withDuplicateUsername_ReturnsBadRequest()
        {
            // Arrange
            var newUser = new CreateUserDto()
            {
                Username = "test",
                Password = "test",
                Name = "test",
                Surname = "test"
            };

            var exitingUser = new User()
            {
                Username = newUser.Username,
                Password = newUser.Password,
                Name = newUser.Name,
                Surname = newUser.Surname
            };

            _userRepositoryStub.Setup(repo => repo.GetUserByUsernameAsync(newUser.Username)).ReturnsAsync(exitingUser);
            var controller = new UserController(_userRepositoryStub.Object, _identityServiceStub.Object);

            // Act
            var result = await controller.CreateUserAsync(newUser);

            // Assert

            result.Result.Should().BeOfType<BadRequestResult>();

        }

        [Fact]
        public async Task CreateAccount_withNewUser_ReturnsToken()
        {
            // Arrange
            var newUser = new CreateUserDto()
            {
                Username = "test",
                Password = "test",
                Name = "test",
                Surname = "test"
            };

            var exampleToken = "exampleToken";

            _userRepositoryStub.Setup(repo => repo.GetUserByUsernameAsync(newUser.Username)).ReturnsAsync((User) null);
            _identityServiceStub.Setup(serv => serv.AuthenticateAsync(newUser.Username, newUser.Password))
                .ReturnsAsync(exampleToken);
            var controller = new UserController(_userRepositoryStub.Object, _identityServiceStub.Object);
            
            // Act
            var result = await controller.CreateUserAsync(newUser);
            // Assert
            result.Value.Token.Should().BeEquivalentTo(exampleToken);
            
        }
        
        
        [Fact]
        public async Task Login_WithInvalidUsername_ReturnsBadRequest()
        {
            // Arrange
            var loginForm = new LoginDto()
            {
                Username = "test",
                Password = "test"
            };
            _userRepositoryStub.Setup(repo => repo.GetUserByUsernameAsync(loginForm.Username)).ReturnsAsync((User)null);
            var controller = new UserController(_userRepositoryStub.Object, _identityServiceStub.Object);
            // Act
            var result = await controller.LoginAsync(loginForm);
            // Assert
            result.Result.Should().BeOfType<BadRequestResult>();
        }

        [Fact]
        public async Task Login_WithInvalidPassword_ReturnsBadRequest()
        {
            
            // Arrange
            var loginForm = new LoginDto()
            {
                Username = "test",
                Password = "test"
            };
            _identityServiceStub.Setup(service => service.AuthenticateAsync(loginForm.Username, loginForm.Password))
                .ReturnsAsync((string) null);
            var controller = new UserController(_userRepositoryStub.Object, _identityServiceStub.Object);
            // Act
            var result = await controller.LoginAsync(loginForm);
            // Assert
            result.Result.Should().BeOfType<BadRequestResult>();
        }

        [Fact]
        public async Task Login_WithValidData_ReturnsToken()
        {
            
            // Arrange
            var loginForm = new LoginDto()
            {
                Username = "test",
                Password = "test"
            };
            string exampleToken = "exampleToken";
            
            _identityServiceStub.Setup(service => service.AuthenticateAsync(loginForm.Username, loginForm.Password))
                .ReturnsAsync(exampleToken);
            var controller = new UserController(_userRepositoryStub.Object, _identityServiceStub.Object);
            // Act
            var result = await controller.LoginAsync(loginForm);
            // Assert
            result.Value.Token.Should().BeEquivalentTo(exampleToken);
        }
    }
    
    
    
    
}