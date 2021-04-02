using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Chater.Controllers;
using Chater.Dtos.Message.Form;
using Chater.Exception;
using Chater.Models;
using Chater.Repository.Abstract;
using Chater.Service.Abstract;
using Chater.Service.Concrete;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace UnitTest.Controllers
{
    public class MessageControllerTest
    {
        private readonly Mock<IRoomRepository> _roomRepository = new();
        private readonly Mock<IUserToRoomRepository> _userToRoom = new();
        private readonly Mock<IIdentityService> _identityService = new();

        private NewMessageForm GenerateNewMessageForm() => new NewMessageForm()
        {
            Text = "blablabla"
        };
        
        [Fact]
        public async Task NewMessage_UserIsNotInRoom_ReturnsBadRequest()
        {
            // Arrange
            var room = GlobalHelper.GenerateRoom();
            var user = GlobalHelper.FakeAuthenticationUser();
            var message = GenerateNewMessageForm();
            var mongoUser = GlobalHelper.GenerateExampleUser();
            
            _roomRepository.Setup(repo => repo.GetRoomAsync(It.IsAny<string>())).ReturnsAsync(room);
            _userToRoom.Setup(
                    repo => repo.GetUserToRoomAsync(It.IsAny<User>(), It.IsAny<Room>()))
                            .ReturnsAsync((UserToRoom) null);
            _identityService.Setup(_identityService => _identityService.GetCurrentUserAsync(It.IsAny<ClaimsIdentity>()))
                .ReturnsAsync(mongoUser);

            ClaimsIdentity claimsIdentity = new ClaimsIdentity();

            
            var helper = new Chater.Service.Concrete.HelperService.HelperService(_roomRepository.Object, _userToRoom.Object);
            
            var service = new MessageService(_roomRepository.Object, helper);

            var controller = new MessageController(service, _identityService.Object);
            controller.ControllerContext = new ControllerContext();  
            controller.ControllerContext.HttpContext = new DefaultHttpContext { User = user };

            // Act
            var result = await controller.NewMessage(It.IsAny<string>(), message);
            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task NewMessage_RoomDoesntExist_ReturnsBadRequest()
        {
            Room room = null;
            var user = GlobalHelper.FakeAuthenticationUser();
            var mongoUser = GlobalHelper.GenerateExampleUser();
            var message = GenerateNewMessageForm();
            _roomRepository.Setup(repo => repo.GetRoomAsync(It.IsAny<string>())).ReturnsAsync((Room) null);
            _userToRoom.Setup(
                    repo => repo.GetUserToRoomAsync(It.IsAny<User>(), It.IsAny<Room>()))
                .ReturnsAsync((UserToRoom)null);
            _identityService.Setup(_identityService => _identityService.GetCurrentUserAsync(It.IsAny<ClaimsIdentity>()))
                .ReturnsAsync(mongoUser);

            var helper = new Chater.Service.Concrete.HelperService.HelperService(_roomRepository.Object, _userToRoom.Object);
            
            var service = new MessageService(_roomRepository.Object, helper);
            
            
            var controller = new MessageController(service, _identityService.Object);
            controller.ControllerContext = new ControllerContext();  
            controller.ControllerContext.HttpContext = new DefaultHttpContext { User = user };
            // Act
            var result = await controller.NewMessage(It.IsAny<string>(), message);
            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();

        }

        [Fact]
        public async Task NewMessage_ValidData_ReturnsOk()
        {
            Room room = GlobalHelper.GenerateRoom();
            var message = GenerateNewMessageForm();
            var user = GlobalHelper.FakeAuthenticationUser();
            var mongoUser = GlobalHelper.GenerateExampleUser();
            var utr = GlobalHelper.AssignUserToRoom(mongoUser, room);
            _identityService.Setup(_identityService => _identityService.GetCurrentUserAsync(It.IsAny<ClaimsIdentity>()))
                .ReturnsAsync(mongoUser);

            
            _roomRepository.Setup(repo => repo.GetRoomAsync(It.IsAny<string>())).ReturnsAsync(room);
            _userToRoom.Setup(
                    repo => repo.UserIsOnRoomAsync(It.IsAny<User>(), It.IsAny<Room>()))
                .ReturnsAsync(true);

            var helper = new Chater.Service.Concrete.HelperService.HelperService(_roomRepository.Object, _userToRoom.Object);
            
            var service = new MessageService(_roomRepository.Object, helper);
            
            
            var controller = new MessageController(service, _identityService.Object);
            controller.ControllerContext = new ControllerContext();  
            controller.ControllerContext.HttpContext = new DefaultHttpContext { User = user };
            
            // Act
            var result = await controller.NewMessage(It.IsAny<string>(), message);
            // Assert
            result.Should().BeOfType<OkResult>();
        }
    }
}