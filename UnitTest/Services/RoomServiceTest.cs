using System;
using System.Threading.Tasks;
using Chater.Dtos.Room;
using Chater.Dtos.Room.Form;
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
        private readonly Mock<IUserRepository> _userRepository = new();
        
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

            var service = new RoomService(_roomRepository.Object, _userToRoomRepository.Object, roomServiceHelper, _userRepository.Object);
            CreateRoomForm newRoom = new()
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
            var service = new RoomService(_roomRepository.Object, _userToRoomRepository.Object, _roomServiceHelper.Object, _userRepository.Object);
            CreateRoomForm newRoom = new()
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
        public async Task UpdateRoom_roomWithNewNameExist_ReturnsRoomActionWithError()
        {
            
            // Arrange
            var room = GlobalHelper.GenerateRoom();
            _roomRepository.Setup(repo => repo.GetRoomByNameAsync(It.IsAny<string>())).ReturnsAsync(room);

            RoomServiceHelper roomServiceHelper = new RoomServiceHelper(_roomRepository.Object, _userToRoomRepository.Object);
            var owner = GlobalHelper.GenerateExampleUser();
            
            var service = new RoomService(_roomRepository.Object, _userToRoomRepository.Object, roomServiceHelper, _userRepository.Object);
            UpdateRoomForm updateRoom = new()
            {
                Name = new Guid().ToString(),
                Password = null,
                NewName = new Guid().ToString()
            };
            // Act
            bool isCatch = false;
            try
            {
                var result = await service.UpdateRoomAsync(updateRoom, owner);
            }
            catch (RoomWithThisNameExist e)
            {
                isCatch = true;
                // Assert
                e.Message.Should().Be("Room with this name exist");
            }

            isCatch.Should().BeTrue("service doesnt throw exceptions");
        }

        [Fact]
        public async Task UpdateRoom_invalidRole_ThrowInvalidRoleException()
        {
            
            // Arrange
            var room = GlobalHelper.GenerateRoom();
            _roomRepository.Setup(repo => repo.GetRoomAsync(It.IsAny<string>())).ReturnsAsync(room);
            var user = GlobalHelper.GenerateExampleUser();
            var utr = GlobalHelper.AssignUserToRoom(user, room, UserToRoom.SimpleUser);
            
            RoomServiceHelper roomServiceHelper = new RoomServiceHelper(_roomRepository.Object, _userToRoomRepository.Object);
            _userToRoomRepository.Setup(repo => repo.GetUserToRoomAsync(user, room)).ReturnsAsync(utr);

            var service = new RoomService(_roomRepository.Object, _userToRoomRepository.Object, roomServiceHelper, _userRepository.Object);
            UpdateRoomForm updateRoom = new()
            {
                Name = new Guid().ToString(),
                Password = null,
                NewName = new Guid().ToString()
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
        public async Task UpdateRoom_withValidData_ReturnsRoomAction()
        {
            
            // Arrange
            var room = GlobalHelper.GenerateRoom();
            room.Password = BCrypt.Net.BCrypt.HashPassword("test");
            var user = GlobalHelper.GenerateExampleUser();
            var utr = GlobalHelper.AssignUserToRoom(user, room, UserToRoom.Administration);


            _roomRepository.Setup(repo => repo.GetRoomAsync(It.IsAny<string>())).ReturnsAsync(room);
            RoomServiceHelper roomServiceHelper = new RoomServiceHelper(_roomRepository.Object, _userToRoomRepository.Object);
            _userToRoomRepository.Setup(repo => repo.GetUserToRoomAsync(It.IsAny<User>(), It.IsAny<Room>())).ReturnsAsync(utr);

            
            var service = new RoomService(_roomRepository.Object, _userToRoomRepository.Object, roomServiceHelper, _userRepository.Object);
            UpdateRoomForm updateRoom = new()
            {
                Name = new Guid().ToString(),
                Password = null,
                NewName = new Guid().ToString()
            };
            // Act
            var result = await service.UpdateRoomAsync(updateRoom, user);
            // Assert
            result.IsSuccessfully.Should().BeTrue();
            result.Error.Should().BeNullOrEmpty();
        }

        [Fact]
        public async Task AddUserToRoom_InvalidPassword_ThrowInvalidPassword()
        {
            // Arrange
            var user = GlobalHelper.GenerateExampleUser();
            var owner = GlobalHelper.GenerateExampleUser();
            var room = GlobalHelper.GenerateRoom();
            room.Password = BCrypt.Net.BCrypt.HashPassword("test");
            RoomServiceHelper helper = new RoomServiceHelper(_roomRepository.Object, _userToRoomRepository.Object);

            _userRepository.Setup(repo => repo.GetUserAsync(It.IsAny<string>())).ReturnsAsync(user);
            _roomRepository.Setup(repo => repo.GetRoomAsync(It.IsAny<string>())).ReturnsAsync(room);
            
            
            RoomService service = new RoomService(_roomRepository.Object, _userToRoomRepository.Object, helper, _userRepository.Object);
            bool isCatch = false;
            AddRemoveUserFromRoom form = new()
            {
                UserId = user.Id,
                RoomPassword = "123"
            };
            
            // Act
            try
            {
                await service.GetRoomAndAddUserAsync(form,  It.IsAny<string>());
            }
            catch (InvalidPasswordException e)
            {
                // Assert
                isCatch = true;
                e.Message.Should().BeSameAs("Invalid password");
            }

            isCatch.Should().BeTrue("service have to throw InvalidPasswordException");

        }

        [Fact]
        public async Task AddUserToRoom_InvalidRoles_ThrowExceptions()
        {
            // Arrange
            var user = GlobalHelper.GenerateExampleUser();
            var owner = GlobalHelper.GenerateExampleUser();
            var room = GlobalHelper.GenerateRoom();
            room.Password = BCrypt.Net.BCrypt.HashPassword("test");
            
            _userRepository.Setup(repo => repo.GetUserAsync(It.IsAny<string>())).ReturnsAsync(user);
            _roomRepository.Setup(repo => repo.GetRoomAsync(It.IsAny<string>())).ReturnsAsync(room);
            
            RoomServiceHelper helper = new RoomServiceHelper(_roomRepository.Object, _userToRoomRepository.Object);
            AddRemoveUserFromRoom form = new()
            {
                UserId = user.Id,
                RoomPassword = "test",
                Role = 5
            };
            RoomService service = new RoomService(_roomRepository.Object, _userToRoomRepository.Object, helper, _userRepository.Object);
            bool isCatch = false;
            // Act
            try
            {
                await service.GetRoomAndAddUserAsync(form,  It.IsAny<string>());
            }
            catch (Exception e)
            {
                // Assert
                isCatch = true;
                e.Message.Should().BeSameAs("Invalid Role");
            }

            isCatch.Should().BeTrue("service have to throw InvalidPasswordException");

        }

        [Fact]
        public async Task AddUserToRoom_UserIsInRoom_ThrowError()
        {
            // Arrange
            var user = GlobalHelper.GenerateExampleUser();
            var room = GlobalHelper.GenerateRoom();
            var owner = GlobalHelper.GenerateExampleUser();
            room.Password = BCrypt.Net.BCrypt.HashPassword("test");
            RoomServiceHelper helper = new RoomServiceHelper(_roomRepository.Object, _userToRoomRepository.Object);
            _userToRoomRepository.Setup(repo => repo.UserIsOnRoomAsync(user, room)).ReturnsAsync(true);
            
            _userRepository.Setup(repo => repo.GetUserAsync(It.IsAny<string>())).ReturnsAsync(user);
            _roomRepository.Setup(repo => repo.GetRoomAsync(It.IsAny<string>())).ReturnsAsync(room);
            
            RoomService service = new RoomService(_roomRepository.Object, _userToRoomRepository.Object, helper, _userRepository.Object);
            bool isCatch = false;
            
            AddRemoveUserFromRoom form = new()
            {
                UserId = user.Id,
                RoomPassword = "test"
            };
            // Act
            try
            {
                await service.GetRoomAndAddUserAsync(form,  It.IsAny<string>());
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
            var owner = GlobalHelper.GenerateExampleUser();
            room.Password = BCrypt.Net.BCrypt.HashPassword("test");
            RoomServiceHelper helper = new RoomServiceHelper(_roomRepository.Object, _userToRoomRepository.Object);

            _userRepository.Setup(repo => repo.GetUserAsync(It.IsAny<string>())).ReturnsAsync(user);
            _roomRepository.Setup(repo => repo.GetRoomAsync(It.IsAny<string>())).ReturnsAsync(room);
            
            
            RoomService service = new RoomService(_roomRepository.Object, _userToRoomRepository.Object, helper, _userRepository.Object);
            
            AddRemoveUserFromRoom form = new()
            {
                UserId = user.Id,
                RoomPassword = "test"
            };
            bool isCatch = false;
            // Act
            try
            {
                await service.GetRoomAndAddUserAsync(form,  It.IsAny<string>());
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