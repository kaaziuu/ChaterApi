using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Chater.Controllers;
using Chater.Dtos.Room.Response;
using Chater.Dtos.User.Response;
using Chater.Models;
using Chater.Service.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace UnitTest
{
    public class RoomControllerTest
    {
        private readonly Mock<IRoomService> _roomService = new();
        private readonly Mock<IUserService> _userService = new();
        private readonly Mock<IIdentityService> _identityService = new();
        private readonly Random _random = new();
     


        
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

            _identityService.Setup(_identityService => _identityService.GetCurrentUserAsync(exitingUsers[0].Username))
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