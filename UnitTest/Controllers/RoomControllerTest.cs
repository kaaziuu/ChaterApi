using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Chater.Controllers;
using Chater.Dtos.Room.From;
using Chater.Dtos.Room.Response;
using Chater.Exception;
using Chater.Extensions;
using Chater.Models;
using Chater.Repository.Abstract;
using Chater.Service.Abstract;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace UnitTest.Controllers
{
    public class RoomControllerTest
    {
        private readonly Mock<IRoomService> _roomService = new();
        private readonly Mock<IUserService> _userService = new();
        private readonly Mock<IIdentityService> _identityService = new();
        private readonly Mock<IRoomRepository> _roomRepository = new();


        [Fact]
        public async Task CreateRoom_WithInvalidData_ReturnsBadRequest()
        {
            
            // Arrange
            var existRoom = GlobalHelper.GenerateRoom();

            CreateUpdateRoomDto createForm = new()
            {
                Name = existRoom.Name,
                Password = "test"
            };
            var owner = GlobalHelper.GenerateExampleUser();
            RoomAction roomAction = new()
            {
                IsSuccessfully = false,
                Room = null,
                Error = "Room with this name exist"
                
            };
            

            ClaimsIdentity claimsIdentity = new ClaimsIdentity();
            var user = GlobalHelper.FakeAuthenticationUser();

            _roomService.Setup(
                s => s.CreateRoomAsync(It.IsAny<CreateUpdateRoomDto>(), It.IsAny<User>())
            ).Throws(new RoomDoesntExistExceptionException("Room with this name exist"));
            
            var controller = new RoomController(_identityService.Object, _userService.Object, _roomService.Object);
            
            controller.ControllerContext = new ControllerContext();  
            controller.ControllerContext.HttpContext = new DefaultHttpContext { User = user };
            
            // Act
            
            var result = await controller.CreateRoomAsync(createForm);
            
            // Assert
            var resultRoomAction = (result.Result as BadRequestObjectResult).Value as RoomAction;
            
            
            result.Result.Should().BeOfType<BadRequestObjectResult>();
            resultRoomAction.IsSuccessfully.Should().BeFalse();
            resultRoomAction.Error.Should().Equals("Room with this name exist");
            resultRoomAction.Room.Should().BeNull();

        }

        [Fact]
        public async Task CreateRoom_WithValidData_ReturnsOkAndCreatedRoom()
        {
            // Arrange
            CreateUpdateRoomDto createForm = new()
            {
                Name = "pokoj 1",
                Password = null
            };
            RoomAction roomAction = new()
            {
                IsSuccessfully = true,
                Room = new RoomDto()
                {
                    Name = createForm.Name,
                    Id = new Guid().ToString()
                },
                Error = null
                
            };
            ClaimsIdentity claimsIdentity = new ClaimsIdentity();
            var user = GlobalHelper.FakeAuthenticationUser();

            
            _roomService.Setup(s => s.CreateRoomAsync(It.IsAny<CreateUpdateRoomDto>(), It.IsAny<User>()))
                .ReturnsAsync(roomAction);
            
            var owner = GlobalHelper.GenerateExampleUser();

            var controller = new RoomController(_identityService.Object, _userService.Object, _roomService.Object);
            controller.ControllerContext = new ControllerContext();  
            controller.ControllerContext.HttpContext = new DefaultHttpContext { User = user };
            // Act
            var result = await controller.CreateRoomAsync(createForm);
            // Assert
            var resultRoomAction = (result.Result as OkObjectResult).Value as RoomAction;
            result.Result.Should().BeOfType<OkObjectResult>();
            resultRoomAction.IsSuccessfully.Should().BeTrue();
            resultRoomAction.Room.Name.Should().Equals(createForm.Name);

        }

        
        [Fact]
        public async Task GetRooms_WithExistingItems_ReturnsAllUserRooms()
        {
            // Arrange
            Room[] existingRoom = {GlobalHelper.GenerateRoom(), GlobalHelper.GenerateRoom(), GlobalHelper.GenerateRoom()};
            User[] exitingUsers = {GlobalHelper.GenerateExampleUser(), GlobalHelper.GenerateExampleUser()};
            /*
             * u0 -> r0, r2
             * u1 -> r1, r0
             */
            GlobalHelper.AssignUserToRoom(exitingUsers[0], existingRoom[0]);
            GlobalHelper.AssignUserToRoom(exitingUsers[1], existingRoom[1]);
            GlobalHelper.AssignUserToRoom(exitingUsers[0], existingRoom[2]);
            GlobalHelper.AssignUserToRoom(exitingUsers[1], existingRoom[0]);

            ClaimsIdentity claimsIdentity = new ClaimsIdentity();
            var user = GlobalHelper.FakeAuthenticationUser();

            ICollection<RoomDto> returnedRoomDtos = new List<RoomDto>()
            {
                existingRoom[0].asDto(),
                existingRoom[2].asDto()
            };

            _identityService.Setup(_identityService => _identityService.GetCurrentUserAsync(claimsIdentity))
                .ReturnsAsync(exitingUsers[0]);
            _userService.Setup(service => service.GetUserRoomsAsync(It.IsAny<User>())).ReturnsAsync(returnedRoomDtos);
            
            var controller = new RoomController(_identityService.Object, _userService.Object, _roomService.Object);
            controller.ControllerContext = new ControllerContext();  
            controller.ControllerContext.HttpContext = new DefaultHttpContext { User = user };
            
            // Act
            var result = await controller.GetRoomsAsync();
            // Assert

            IEnumerable <RoomDto> objects = (result.Result as OkObjectResult).Value as IEnumerable<RoomDto>;

            objects.Should().OnlyContain(
                room => room.Name == existingRoom[0].Name || room.Name == existingRoom[2].Name
            );
            

        }
    }
}