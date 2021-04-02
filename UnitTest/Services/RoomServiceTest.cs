using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Chater.Dtos.Room;
using Chater.Dtos.Room.Form;
using Chater.Dtos.Room.Response;
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
            AddUserFromRoom form = new()
            {
                UserId = user.Id,
                RoomPassword = "123"
            };
            
            // Act
            
            Func<Task> action = async () => await service.GetRoomAndAddUserAsync(form,  It.IsAny<string>());

            // Assert
            action.Should().ThrowAsync<InvalidPasswordException>();

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
            AddUserFromRoom form = new()
            {
                UserId = user.Id,
                RoomPassword = "test",
                Role = 5
            };
            RoomService service = new RoomService(_roomRepository.Object, _userToRoomRepository.Object, helper, _userRepository.Object);
         
            // Act 
            Func<Task> action = async () => await service.GetRoomAndAddUserAsync(form,  It.IsAny<string>());
            // Assert
            action.Should().ThrowAsync<InvalidRoleException>();

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
            
            AddUserFromRoom form = new()
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
            
            AddUserFromRoom form = new()
            {
                UserId = user.Id,
                RoomPassword = "test"
            };
            // Act

            Func<Task> action = async () => await service.GetRoomAndAddUserAsync(form, It.IsAny<string>());
            // Assert 
        }

        [Fact]
        public async Task RemoveUserFromRoom_UserIsNotInRoom_ThrowExceptions()
        {
            // Arrange
            var form = new RemoveUserForm()
            {
                Password = "test",
                UserId = It.IsAny<string>()
            };
            var owner = GlobalHelper.GenerateExampleUser();
            var user = GlobalHelper.GenerateExampleUser();
            var room = GlobalHelper.GenerateRoom();

            room.Password = BCrypt.Net.BCrypt.HashPassword("test");
            var utr = GlobalHelper.AssignUserToRoom(user, room, UserToRoom.Administration);
            
            _userToRoomRepository.Setup(repo => repo.UserIsOnRoomAsync(It.IsAny<User>(), It.IsAny<Room>()))
                .ReturnsAsync(false);
            _roomRepository.Setup(repo => repo.GetRoomAsync(It.IsAny<string>())).ReturnsAsync(room);
            _userToRoomRepository.Setup(repo => repo.GetUserToRoomAsync(It.IsAny<User>(), It.IsAny<Room>()))
                .ReturnsAsync(utr);
            _userRepository.Setup(repo => repo.GetUserAsync(It.IsAny<string>())).ReturnsAsync(user);
            
            RoomServiceHelper helper = new RoomServiceHelper(_roomRepository.Object, _userToRoomRepository.Object);
            RoomService roomService = new RoomService(_roomRepository.Object, _userToRoomRepository.Object, helper,
                _userRepository.Object);
            // act

            Func<Task> action =  async () => await roomService.RemoveUserFromRoomAsync(owner, form, It.IsAny<string>());
            // assert

            action.Should().Throw<Exception>();
        }

        [Fact]
        public async Task RemoveUserFromRoom_InvalidPassword_ThrowExceptions()
        {
            ICollection<int> tmp = new List<int>(){1, 2, 3, 4};
            // Arrange
            var form = new RemoveUserForm()
            {
                Password = "test",
                UserId = It.IsAny<string>()
            };
            var owner = GlobalHelper.GenerateExampleUser();
            var user = GlobalHelper.GenerateExampleUser();
            var room = GlobalHelper.GenerateRoom();
            var utr = GlobalHelper.AssignUserToRoom(user, room, UserToRoom.Administration);
            room.Password = BCrypt.Net.BCrypt.HashPassword("test12");

            _userToRoomRepository.Setup(repo => repo.UserIsOnRoomAsync(It.IsAny<User>(), It.IsAny<Room>()))
                .ReturnsAsync(true);
            _roomRepository.Setup(repo => repo.GetRoomAsync(It.IsAny<string>())).ReturnsAsync(room);
            _userToRoomRepository.Setup(repo => repo.GetUserToRoomAsync(It.IsAny<User>(), It.IsAny<Room>()))
                .ReturnsAsync(utr);
            _userRepository.Setup(repo => repo.GetUserAsync(It.IsAny<string>())).ReturnsAsync(user);

            
            
            RoomServiceHelper helper = new RoomServiceHelper(_roomRepository.Object, _userToRoomRepository.Object);
            RoomService roomService = new RoomService(_roomRepository.Object, _userToRoomRepository.Object, helper,
                _userRepository.Object);
            // act
            Func<Task> action = async () => await roomService.RemoveUserFromRoomAsync(owner, form, It.IsAny<string>());
            
            // Assert
            await action.Should().ThrowAsync<InvalidPasswordException>().WithMessage("Invalid Password");


        }
        [Fact]
        public async Task RemoveUserFromRoom_InvalidRole_ThrowExceptions()
        {
            // Arrange
            var form = new RemoveUserForm()
            {
                Password = "test",
                UserId = It.IsAny<string>()
            };
            var owner = GlobalHelper.GenerateExampleUser();
            var user = GlobalHelper.GenerateExampleUser();
            var room = GlobalHelper.GenerateRoom();
            var utr = GlobalHelper.AssignUserToRoom(user, room, UserToRoom.SimpleUser);
            room.Password = BCrypt.Net.BCrypt.HashPassword("test");

            _userToRoomRepository.Setup(repo => repo.UserIsOnRoomAsync(It.IsAny<User>(), It.IsAny<Room>()))
                .ReturnsAsync(true);
            _roomRepository.Setup(repo => repo.GetRoomAsync(It.IsAny<string>())).ReturnsAsync(room);
            _userToRoomRepository.Setup(repo => repo.GetUserToRoomAsync(It.IsAny<User>(), It.IsAny<Room>()))
                .ReturnsAsync(utr);
            _userRepository.Setup(repo => repo.GetUserAsync(It.IsAny<string>())).ReturnsAsync(user);

            
            RoomServiceHelper helper = new RoomServiceHelper(_roomRepository.Object, _userToRoomRepository.Object);
            RoomService roomService = new RoomService(_roomRepository.Object, _userToRoomRepository.Object, helper,
                _userRepository.Object);
            // act
            Func<Task> action =  async () => await roomService.RemoveUserFromRoomAsync(owner, form, It.IsAny<string>());
            // Assert
            action.Should().Throw<InvalidRoleException>().WithMessage("Invalid Role");


        }

        [Fact]
        public async Task RemoveUserFromRoom_RomeDoesnExist_ThrowExceptions()
        {
            // Arrange
            var form = new RemoveUserForm()
            {
                Password = "test",
                UserId = It.IsAny<string>()
            };
            var owner = GlobalHelper.GenerateExampleUser();
            var user = GlobalHelper.GenerateExampleUser();
            var room = GlobalHelper.GenerateRoom();
            var utr = GlobalHelper.AssignUserToRoom(user, room, UserToRoom.Administration);
            room.Password = BCrypt.Net.BCrypt.HashPassword("test");

            _userToRoomRepository.Setup(repo => repo.UserIsOnRoomAsync(It.IsAny<User>(), It.IsAny<Room>()))
                .ReturnsAsync(false);
            _roomRepository.Setup(repo => repo.GetRoomAsync(It.IsAny<string>())).ReturnsAsync((Room)null);
            _userToRoomRepository.Setup(repo => repo.GetUserToRoomAsync(It.IsAny<User>(), It.IsAny<Room>()))
                .ReturnsAsync(utr);
            _userRepository.Setup(repo => repo.GetUserAsync(It.IsAny<string>())).ReturnsAsync(user);

            
            RoomServiceHelper helper = new RoomServiceHelper(_roomRepository.Object, _userToRoomRepository.Object);
            RoomService roomService = new RoomService(_roomRepository.Object, _userToRoomRepository.Object, helper,
                _userRepository.Object);
            // act
            Func<Task> action =  async () => await roomService.RemoveUserFromRoomAsync(owner, form, It.IsAny<string>());
            // Assert
            action.Should().Throw<RoomDoesntExistExceptionException>().WithMessage("Room doesnt exist");

        }
        
        [Fact]
        public async Task RemoveUserFromRoom_ValidData_EndWithoutError()
        {
            // Arrange
            var form = new RemoveUserForm()
            {
                Password = "test",
                UserId = It.IsAny<string>()
            };
            var owner = GlobalHelper.GenerateExampleUser();
            var user = GlobalHelper.GenerateExampleUser();
            var room = GlobalHelper.GenerateRoom();
            var utr = GlobalHelper.AssignUserToRoom(user, room, UserToRoom.Administration);
            room.Password = BCrypt.Net.BCrypt.HashPassword("test");

            _userToRoomRepository.Setup(repo => repo.UserIsOnRoomAsync(It.IsAny<User>(), It.IsAny<Room>()))
                .ReturnsAsync(true);
            _roomRepository.Setup(repo => repo.GetRoomAsync(It.IsAny<string>())).ReturnsAsync(room);
            _userToRoomRepository.Setup(repo => repo.GetUserToRoomAsync(It.IsAny<User>(), It.IsAny<Room>()))
                .ReturnsAsync(utr);
            _userRepository.Setup(repo => repo.GetUserAsync(It.IsAny<string>())).ReturnsAsync(user);

            
            RoomServiceHelper helper = new RoomServiceHelper(_roomRepository.Object, _userToRoomRepository.Object);
            RoomService roomService = new RoomService(_roomRepository.Object, _userToRoomRepository.Object, helper,
                _userRepository.Object);
            // act
            Func<Task> action=  async () => await roomService.RemoveUserFromRoomAsync(owner, form, It.IsAny<string>());
            // Assert
            action.Should().NotThrow();
        }
    }
}