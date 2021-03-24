using System;
using System.Linq;
using System.Threading.Tasks;
using Chater.Dtos.Room.From;
using Chater.Dtos.Room.Response;
using Chater.Models;
using Chater.Repository.Abstract;
using Chater.Repository.Contrete;
using Chater.Service.Concrete;
using FluentAssertions;
using Moq;
using Xunit;

namespace UnitTest
{
    public class RoomServiceTest
    {
        private readonly Mock<IUserToRoomRepository> _userToRoomService = new();
        private readonly Mock<IRoomRepository> _roomRepository = new();
        
        [Fact]
        public async Task CreatRoom_RoomWithThisNameExist_ReturnsRoomActionWithError()
        {
            // Arrange
            var room = GlobalHelper.GenerateRoom();
            var existRoom = GlobalHelper.GenerateRoom();
            var owner = GlobalHelper.GenerateExampleUser();
            existRoom.Name = room.Name;
            _roomRepository.Setup(repo => repo.GetRoomByNameAsync(room.Name)).ReturnsAsync(existRoom);
            var service = new RoomService(_roomRepository.Object, _userToRoomService.Object);
            CreateUpdateRoomDto newRoom = new()
            {
                Name = room.Name,
                Password = null
            };
            // Act
            var result = await service.CreateRoomAsync(newRoom, owner);
            // // Assert
            result.IsSuccessfully.Should().Equals(false);
            result.Error.Equals("Room with this name exist");
            result.Room.Should().Be(null);
        }

        [Fact]
        public async Task CreateRoom_withValidData_ReturnsRoomActionWithoutError()
        {
            // Arrange
            var room = GlobalHelper.GenerateRoom();
            _roomRepository.Setup(repo => repo.GetRoomByNameAsync(room.Name)).ReturnsAsync((Room)null);
            var owner = GlobalHelper.GenerateExampleUser();
            var service = new RoomService(_roomRepository.Object, _userToRoomService.Object);
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
            _roomRepository.Setup(repo => repo.GetRoomByNameAsync(room.Name)).ReturnsAsync((Room)null);
            var owner = GlobalHelper.GenerateExampleUser();

            var service = new RoomService(_roomRepository.Object, _userToRoomService.Object);
            CreateUpdateRoomDto updateRoom = new()
            {
                Name = new Guid().ToString(),
                Password = null
            };
            // Act
            var result = await service.UpdateRoomAsync(updateRoom, owner);
            // Assert
            result.IsSuccessfully.Should().Equals(false);
            result.Error.Equals("Room with this name doesnt exist");
            result.Room.Name.Should().Equals(room.Name);
        }

        [Fact]
        public async Task UpdateRoom_invalidPassword_ReturnsRoomActionWithError()
        {
            
            // Arrange
            var room = GlobalHelper.GenerateRoom();
            _roomRepository.Setup(repo => repo.GetRoomByNameAsync(room.Name)).ReturnsAsync(room);
            var owner = GlobalHelper.GenerateExampleUser();

            var service = new RoomService(_roomRepository.Object, _userToRoomService.Object);
            CreateUpdateRoomDto updateRoom = new()
            {
                Name = new Guid().ToString(),
                Password = BCrypt.Net.BCrypt.HashPassword("test")
            };
            // Act
            var result = await service.UpdateRoomAsync(updateRoom, owner);
            // Assert
            result.IsSuccessfully.Should().Equals(false);
            result.Error.Equals("Invalid password");
            result.Room.Name.Should().Equals(room.Name);
        }

        [Fact]
        public async Task UpdateRoom_withValidData_ReturnsRoomActionWithError()
        {
            
            // Arrange
            var room = GlobalHelper.GenerateRoom();
            room.Password = BCrypt.Net.BCrypt.HashPassword("test");
            var owner = GlobalHelper.GenerateExampleUser();

            _roomRepository.Setup(repo => repo.GetRoomByNameAsync(room.Name)).ReturnsAsync(room);
            var service = new RoomService(_roomRepository.Object, _userToRoomService.Object);
            CreateUpdateRoomDto updateRoom = new()
            {
                Name = new Guid().ToString(),
                Password = BCrypt.Net.BCrypt.HashPassword("test")
            };
            // Act
            var result = await service.UpdateRoomAsync(updateRoom, owner);
            // Assert
            result.IsSuccessfully.Should().Equals(true);
            result.Error.Should().BeNullOrEmpty();
            result.Room.Name.Should().Equals(updateRoom.Name);
        }

        [Fact]
        public async Task PasswordVerification_InvalidPassword_ReturnsFalse()
        {
            // Arrange
            var room = GlobalHelper.GenerateRoom();
            room.Password = BCrypt.Net.BCrypt.HashPassword("test");
            var service = new RoomService(_roomRepository.Object, _userToRoomService.Object);
            
            // Act
            var result = service.PasswordVerificationAsync(room, "123");
            // Assert
            result.Should().Equals(false);
        }
        
       
        [Fact]
        public async Task PasswordVerification_ValidPassword_ReturnsFalse()
        {
            // Arrange
            var room = GlobalHelper.GenerateRoom();
            room.Password = BCrypt.Net.BCrypt.HashPassword("test");
            var service = new RoomService(_roomRepository.Object, _userToRoomService.Object);
            
            // Act
            var result = service.PasswordVerificationAsync(room, "test");
            // Assert
            result.Should().Equals(true);
        }
        [Fact]
        public async Task PasswordVerificationByRoomName_InvalidPassword_ReturnsFalse()
        {
            // Arrange
            var room = GlobalHelper.GenerateRoom();
            room.Password = BCrypt.Net.BCrypt.HashPassword("test");
            _roomRepository.Setup(repo => repo.GetRoomByNameAsync(room.Name)).ReturnsAsync(room);
            var service = new RoomService(_roomRepository.Object, _userToRoomService.Object);
            
            // Act
            var result = await service.PasswordVerificationByRoomNameAsync(room.Name, "123");
            // Assert
            result.Should().Equals(false);
        }
        
       
        [Fact]
        public async Task PasswordVerificationByRoomName_ValidPassword_ReturnsFalse()
        {
            // Arrange
            var room = GlobalHelper.GenerateRoom();
            room.Password = BCrypt.Net.BCrypt.HashPassword("test");
            _roomRepository.Setup(repo => repo.GetRoomByNameAsync(room.Name)).ReturnsAsync(room);

            var service = new RoomService(_roomRepository.Object, _userToRoomService.Object);
            
            // Act
            var result = await service.PasswordVerificationByRoomNameAsync(room.Name, "test");
            // Assert
            result.Should().Equals(true);
        }

        

    }
}