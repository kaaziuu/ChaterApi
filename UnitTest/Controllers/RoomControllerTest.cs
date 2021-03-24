using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Chater.Controllers;
using Chater.Dtos.Room.From;
using Chater.Dtos.Room.Response;
using Chater.Dtos.User.Response;
using Chater.Models;
using Chater.Repository.Abstract;
using Chater.Service.Abstract;
using Chater.Service.Concrete;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace UnitTest
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
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim(ClaimTypes.NameIdentifier, "SomeValueHere"),
                new Claim(ClaimTypes.Name, "gunnar@somecompany.com")
                // other required and custom claims
            },"TestAuthentication"));

            _roomService.Setup(
                s => s.CreateRoomAsync(It.IsAny<CreateUpdateRoomDto>(), It.IsAny<User>())
            ).ReturnsAsync(roomAction);
            
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
                Room = new Room()
                {
                    Name = createForm.Name,
                    Id = new Guid().ToString()
                },
                Error = null
                
            };
            ClaimsIdentity claimsIdentity = new ClaimsIdentity();
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim(ClaimTypes.NameIdentifier, "SomeValueHere"),
                new Claim(ClaimTypes.Name, "gunnar@somecompany.com")
                // other required and custom claims
            },"TestAuthentication"));
            
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

            _identityService.Setup(_identityService => _identityService.GetCurrentUserAsync(claimsIdentity))
                .ReturnsAsync(exitingUsers[0]);
            var controller = new RoomController(_identityService.Object, _userService.Object, _roomService.Object);

            
            // Act
            var result = await controller.GetRoomsAsync();
            // Assert
            IEnumerable<RoomDto> objects = result.Value;
            objects.Should().OnlyContain(
                room => room.Name == existingRoom[0].Name || room.Name == existingRoom[2].Name
            );
            

        }
    }
}