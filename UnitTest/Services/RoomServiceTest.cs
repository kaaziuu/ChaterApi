using System;
using System.Threading.Tasks;
using Chater.Dtos.Room.From;
using Chater.Exception;
using Chater.Models;
using Chater.Repository.Abstract;
using Chater.Service.Abstract.HelperService;
using Chater.Service.Concrete;
using Chater.Service.Concrete.HelperService;
using FluentAssertions;
using Moq;
using Xunit;

namespace UnitTest.Services
{
    public class RoomServiceTest
    {
        private readonly Mock<IUserToRoomRepository> _userToRoomRepository = new();
        private readonly Mock<IRoomRepository> _roomRepository = new();
        private readonly Mock<IRoomServiceHelper> _roomServiceHelper = new();
        
        [Fact]
        public async Task CreatRoom_RoomWithThisNameExist_ThrowRoomWithThisNameExist()
        {
            // Arrange
            var room = GlobalHelper.GenerateRoom();
            var existRoom = GlobalHelper.GenerateRoom();
            var owner = GlobalHelper.GenerateExampleUser();
            existRoom.Name = room.Name;
            _roomRepository.Setup(repo => repo.GetRoomByNameAsync(room.Name)).ReturnsAsync(existRoom);
            RoomServiceHelper roomServiceHelper = new RoomServiceHelper(_roomRepository.Object, _userToRoomRepository.Object);

            var service = new RoomService(_roomRepository.Object, _userToRoomRepository.Object, roomServiceHelper);
            CreateUpdateRoomDto newRoom = new()
            {
                Name = room.Name,
                Password = null
            };
            // Act
            bool isCatch = false;
            try
            {
                var result = await service.CreateRoomAsync(newRoom, owner);
            }
            catch(RoomWithThisNameExist e)
            {
                isCatch = true;
                // // Assert
                e.Message.Should().Equals("Room with this name exist");
            }
            Assert.True(isCatch, "service doesn't throw exception ");
        }

        [Fact]
        public async Task CreateRoom_withValidData_ReturnsRoomActionWithoutError()
        {
            // Arrange
            var room = GlobalHelper.GenerateRoom();
            _roomRepository.Setup(repo => repo.GetRoomByNameAsync(room.Name)).ReturnsAsync((Room)null);
            var owner = GlobalHelper.GenerateExampleUser();
            var service = new RoomService(_roomRepository.Object, _userToRoomRepository.Object, _roomServiceHelper.Object);
            CreateUpdateRoomDto newRoom = new()
            {
                Name = room.Name,
                Password = null
            };
            // Act
            var result = await service.CreateRoomAsync(newRoom, owner);
            // Assert
            result.IsSuccessfully.Should().Equals(true);
            result.Error.Should().BeNullOrEmpty();
            result.Room.Name.Should().Equals(newRoom.Name);

        }

        [Fact]
        public async Task UpdateRoom_roomDoesntExist_ReturnsRoomActionWithError()
        {
            
            // Arrange
            var room = GlobalHelper.GenerateRoom();
            _roomRepository.Setup(repo => repo.GetRoomByNameAsync(room.Name)).ReturnsAsync((Room) null);

            RoomServiceHelper roomServiceHelper = new RoomServiceHelper(_roomRepository.Object, _userToRoomRepository.Object);
            var owner = GlobalHelper.GenerateExampleUser();
            
            var service = new RoomService(_roomRepository.Object, _userToRoomRepository.Object, roomServiceHelper);
            CreateUpdateRoomDto updateRoom = new()
            {
                Name = new Guid().ToString(),
                Password = null
            };
            // Act
            bool isCatch = false;
            try
            {
                var result = await service.UpdateRoomAsync(updateRoom, owner);
            }
            catch (RoomDoesntExistExceptionException e)
            {
                isCatch = true;
                // Assert
                e.Message.Should().Be("Invalid name of room");
            }

            isCatch.Should().BeTrue("service doesnt throw exceptions");
        }

        [Fact]
        public async Task UpdateRoom_invalidRole_ThrowInvalidRoleException()
        {
            
            // Arrange
            var room = GlobalHelper.GenerateRoom();
            _roomRepository.Setup(repo => repo.GetRoomByNameAsync(room.Name)).ReturnsAsync(room);
            var user = GlobalHelper.GenerateExampleUser();
            var utr = GlobalHelper.AssignUserToRoom(user, room, UserToRoom.SimpleUser);
            
            RoomServiceHelper roomServiceHelper = new RoomServiceHelper(_roomRepository.Object, _userToRoomRepository.Object);
            _userToRoomRepository.Setup(repo => repo.GetUserToRoomAsync(user, room)).ReturnsAsync(utr);

            var service = new RoomService(_roomRepository.Object, _userToRoomRepository.Object, roomServiceHelper);
            CreateUpdateRoomDto updateRoom = new()
            {
                Name = new Guid().ToString(),
                Password = BCrypt.Net.BCrypt.HashPassword("test")
            };
            // Act
            bool isCatch = false;
            try
            {
                var result = await service.UpdateRoomAsync(updateRoom, user);
            }
            catch (InvalidRoleException e)
            {
                // Assert
                isCatch = true;
                e.Message.Should().Equals("Invalid roles");
            }

            isCatch.Should().BeTrue("service doesnt throw error");
        }

        [Fact]
        public async Task UpdateRoom_withValidData_ReturnsRoomActionWithError()
        {
            
            // Arrange
            var room = GlobalHelper.GenerateRoom();
            room.Password = BCrypt.Net.BCrypt.HashPassword("test");
            var user = GlobalHelper.GenerateExampleUser();
            var utr = GlobalHelper.AssignUserToRoom(user, room, UserToRoom.Administration);


            _roomRepository.Setup(repo => repo.GetRoomByNameAsync(room.Name)).ReturnsAsync(room);
            RoomServiceHelper roomServiceHelper = new RoomServiceHelper(_roomRepository.Object, _userToRoomRepository.Object);
            _userToRoomRepository.Setup(repo => repo.GetUserToRoomAsync(user, room)).ReturnsAsync(utr);

            
            var service = new RoomService(_roomRepository.Object, _userToRoomRepository.Object, roomServiceHelper);
            CreateUpdateRoomDto updateRoom = new()
            {
                Name = new Guid().ToString(),
                Password = BCrypt.Net.BCrypt.HashPassword("test")
            };
            // Act
            var result = await service.UpdateRoomAsync(updateRoom, user);
            // Assert
            result.IsSuccessfully.Should().Equals(true);
            result.Error.Should().BeNullOrEmpty();
            result.Room.Name.Should().Equals(updateRoom.Name);
        }

        [Fact]
        public async Task AddUserToRoom_InvalidPassword_ThrowInvalidPassword()
        {
            // Arrange
            var user = GlobalHelper.GenerateExampleUser();
            var room = GlobalHelper.GenerateRoom();
            room.Password = BCrypt.Net.BCrypt.HashPassword("test");
            RoomServiceHelper helper = new RoomServiceHelper(_roomRepository.Object, _userToRoomRepository.Object);

            RoomService service = new RoomService(_roomRepository.Object, _userToRoomRepository.Object, helper);
            bool isCatch = false;
            // Act
            try
            {
                await service.AddUserToRoomAsync(user, room, UserToRoom.SimpleUser, "123");
            }
            catch (InvalidPasswordException e)
            {
                // Assert
                isCatch = true;
                e.Should().Equals("Invalid password");
            }

            isCatch.Should().BeTrue("service have to throw InvalidPasswordException");

        }

        [Fact]
        public async Task AddUserToRoom_InvalidRoles_ThrowExceptions()
        {
            // Arrange
            var user = GlobalHelper.GenerateExampleUser();
            var room = GlobalHelper.GenerateRoom();
            room.Password = BCrypt.Net.BCrypt.HashPassword("test");
            RoomServiceHelper helper = new RoomServiceHelper(_roomRepository.Object, _userToRoomRepository.Object);

            RoomService service = new RoomService(_roomRepository.Object, _userToRoomRepository.Object, helper);
            bool isCatch = false;
            // Act
            try
            {
                await service.AddUserToRoomAsync(user, room, 5, "test");
            }
            catch (NotImplementedException e)
            {
            }
            catch (Exception e)
            {
                // Assert
                isCatch = true;
                e.Should().Equals("Invalid Role");
            }

            isCatch.Should().BeTrue("service have to throw InvalidPasswordException");

        }

        [Fact]
        public async Task AddUserToRoom_UserIsInRoom_ThrowError()
        {
            // Arrange
            var user = GlobalHelper.GenerateExampleUser();
            var room = GlobalHelper.GenerateRoom();
            room.Password = BCrypt.Net.BCrypt.HashPassword("test");
            RoomServiceHelper helper = new RoomServiceHelper(_roomRepository.Object, _userToRoomRepository.Object);
            _userToRoomRepository.Setup(repo => repo.UserIsOnRoomAsync(user, room)).ReturnsAsync(true);
            RoomService service = new RoomService(_roomRepository.Object, _userToRoomRepository.Object, helper);
            bool isCatch = false;
            // Act
            try
            {
                await service.AddUserToRoomAsync(user, room, UserToRoom.Administration, "test");
            }
            catch (NotImplementedException e)
            {
            }
            catch (Exception e)
            {
                // Assert
                isCatch = true;
                e.Should().Equals("User is in room");
            }

            isCatch.Should().BeTrue("service have to throw InvalidPasswordException");

        }
        
        [Fact]
        public async Task AddUserToRoom_ValidData_EndMethodWithoutError()
        {
            // Arrange
            var user = GlobalHelper.GenerateExampleUser();
            var room = GlobalHelper.GenerateRoom();
            room.Password = BCrypt.Net.BCrypt.HashPassword("test");
            RoomServiceHelper helper = new RoomServiceHelper(_roomRepository.Object, _userToRoomRepository.Object);

            RoomService service = new RoomService(_roomRepository.Object, _userToRoomRepository.Object, helper);
            bool isCatch = false;
            // Act
            try
            {
                await service.AddUserToRoomAsync(user, room, UserToRoom.Administration, "test");
            }
            catch (Exception e)
            {
                // Assert
                isCatch = true;
            }

            isCatch.Should().BeFalse("service shouldn't throw any exception");

        }
        
        

    }
}