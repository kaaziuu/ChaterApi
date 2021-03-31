using System;
using System.Threading.Tasks;
using Chater.Dtos.Room;
using Chater.Dtos.Room.Form;
using Chater.Exception;
using Chater.Models;
using Chater.Repository.Abstract;
using Chater.Service.Concrete.HelperService;
using FluentAssertions;
using Moq;
using Xunit;

namespace UnitTest.Services.HelperService
{
    public class RoomServiceHelperTest
    {
        
        private readonly Mock<IUserToRoomRepository> _userToRoomService = new();
        private readonly Mock<IRoomRepository> _roomRepository = new();
        [Fact]
        public async Task PasswordVerification_InvalidPassword_ReturnsFalse()
        {
            // Arrange
            var room = GlobalHelper.GenerateRoom();
            room.Password = BCrypt.Net.BCrypt.HashPassword("test");
            var service = new RoomServiceHelper(_roomRepository.Object, _userToRoomService.Object);
            bool isCatch = false;
            // Act
            try
            {
                var result = service.PasswordVerification(room, "123");
            }
            catch(InvalidPasswordException e)
            {
                isCatch = true;
                // Assert
                e.Message.Should().Equals("Invalid password");
            }

            isCatch.Should().BeTrue("service have to throw InvalidPasswordException");
        }
        
       
        [Fact]
        public async Task PasswordVerification_ValidPassword_ReturnsFalse()
        {
            // Arrange
            var room = GlobalHelper.GenerateRoom();
            room.Password = BCrypt.Net.BCrypt.HashPassword("test");
            var service = new RoomServiceHelper(_roomRepository.Object, _userToRoomService.Object);
            
            // Act
            var result = service.PasswordVerification(room, "test");
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
            var service = new RoomServiceHelper(_roomRepository.Object, _userToRoomService.Object);
            bool isCatch = false;
            // Act
            try
            {
                await service.PasswordVerificationByRoomNameAsync(room.Name, "123");
            }
            catch(InvalidPasswordException e)
            {
                isCatch = true;
                // Assert
                e.Message.Should().Equals("Invalid password");
            }

            isCatch.Should().BeTrue("service have to throw InvalidPasswordException");
        }
        
       
        [Fact]
        public async Task PasswordVerificationByRoomName_ValidPassword_ReturnsFalse()
        {
            // Arrange
            var room = GlobalHelper.GenerateRoom();
            room.Password = BCrypt.Net.BCrypt.HashPassword("test");
            _roomRepository.Setup(repo => repo.GetRoomByNameAsync(room.Name)).ReturnsAsync(room);

            var service = new RoomServiceHelper(_roomRepository.Object, _userToRoomService.Object);
            
            // Act
            var result = await service.PasswordVerificationByRoomNameAsync(room.Name, "test");
            // Assert
            result.Should().Equals(true);
        }

        [Fact]
        public async Task RoomExist_RoomExist_ReturnsTrue()
        {
            // Arrange
            var existRoom = new Room()
            {
                Name = new Guid().ToString(),
                Id = new Guid().ToString(),
                Password = new Guid().ToString()
            };

            _roomRepository.Setup(repo => repo.GetRoomByNameAsync(existRoom.Name)).ReturnsAsync(existRoom);
            var service = new RoomServiceHelper(_roomRepository.Object, _userToRoomService.Object);

            // Act
            var result = await service.RoomIsExistAsync(existRoom.Name);
            // Assert
            result.Should().Be(true);

        }
        
        
        [Fact]
        public async Task RoomExist_RoomNotExist_ReturnsFalse()
        {
            // Arrange
            string exampleName = "Test";
            
            _roomRepository.Setup(repo => repo.GetRoomByNameAsync(exampleName)).ReturnsAsync((Room)null);
            var service = new RoomServiceHelper(_roomRepository.Object, _userToRoomService.Object);

            // Act
            var result = await service.RoomIsExistAsync(exampleName);
            // Assert
            result.Should().Be(false);

        }

        [Fact]
        public async Task VerificationDataBeforeUpdate_RoomWithNewNameExist_ThrowRoomDoesntExist()
        {
            // Arrange
            UpdateRoomForm updateForm = new()
            {
                Name = new Guid().ToString(),
                Password = "test"
            };
            User user = GlobalHelper.GenerateExampleUser();
            Room room = GlobalHelper.GenerateRoom();
            _roomRepository.Setup(repo => repo.GetRoomByNameAsync(It.IsAny<string>())).ReturnsAsync(room);
            var service = new RoomServiceHelper(_roomRepository.Object, _userToRoomService.Object);

            // Act
            bool isCatch = false;
            try
            {
                await service.VerificationDataBeforeUpdate(updateForm, user);
            }
            catch (RoomWithThisNameExist e)
            {
                // assert
                isCatch = true;
                e.Message.Should().BeSameAs("Room with this name exist");
            }
            isCatch.Should().BeTrue("method doesn't throw RoomDoesntExistExceptionException");
            
            
        }

        [Fact]
        public async Task VerificationDataBeforeUpdate_InvalidRole_ThrowInvalidRole()
        {
            // Arrange
            Room room = GlobalHelper.GenerateRoom();
            User user = GlobalHelper.GenerateExampleUser();
            var utr= GlobalHelper.AssignUserToRoom(user, room, UserToRoom.SimpleUser);
            
            UpdateRoomForm updateForm = new()
            {
                Name = room.Name,
                Password = room.Password
            };
            
            _roomRepository.Setup(repo => repo.GetRoomAsync(It.IsAny<string>())).ReturnsAsync((Room) room);
            _userToRoomService.Setup(repo => repo.GetUserToRoomAsync(user, room)).ReturnsAsync(utr);
            var service = new RoomServiceHelper(_roomRepository.Object, _userToRoomService.Object);
            
            // Act
            bool isCatch = false;
            try
            {
                await service.VerificationDataBeforeUpdate(updateForm, user);
            }
            catch (InvalidRoleException e)
            {
                // assert
                isCatch = true;
                e.Message.Should().Equals("Invalid role");
            }
            isCatch.Should().BeTrue("method doesn't throw InvalidRoleException");

        }

        [Fact]
        public async Task VerificationDataBeforeUpdate_UserIsNotInTheROom_ThrowInvalidRole()
        {
            // Arrange
            Room room = GlobalHelper.GenerateRoom();
            User user = GlobalHelper.GenerateExampleUser();
            GlobalHelper.AssignUserToRoom(user, room, UserToRoom.SimpleUser);
            UpdateRoomForm updateForm = new()
            {
                Name = room.Name,
                Password = room.Password
            };
            
            _roomRepository.Setup(repo => repo.GetRoomAsync(It.IsAny<string>())).ReturnsAsync((Room) room);
            _userToRoomService.Setup(repo => repo.GetUserToRoomAsync(user, room)).ReturnsAsync((UserToRoom) null);
            var service = new RoomServiceHelper(_roomRepository.Object, _userToRoomService.Object);
            
            // Act
            bool isCatch = false;
            try
            {
                await service.VerificationDataBeforeUpdate(updateForm, user);
            }
            catch (InvalidRoleException e)
            {
                // assert
                isCatch = true;
                e.Message.Should().Equals("Invalid role");
            }
            isCatch.Should().BeTrue("method doesn't throw InvalidRoleException");
        }


    }
}