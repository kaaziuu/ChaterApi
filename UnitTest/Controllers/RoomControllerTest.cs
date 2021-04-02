using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Chater.Controllers;
using Chater.Dtos.Room;
using Chater.Dtos.Room.Form;
using Chater.Dtos.Room.Response;
using Chater.Exception;
using Chater.Extensions;
using Chater.Models;
using Chater.Repository.Abstract;
using Chater.Repository.Contrete;
using Chater.Service.Abstract;
using Chater.Service.Concrete;
using Chater.Service.Concrete.HelperService;
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
        private readonly Mock<IUserToRoomRepository> _userToRoomRepository = new();
        private readonly Mock<IRoomRepository> _roomRepository = new();
        private readonly Mock<IUserRepository> _userRepository = new();


        [Fact]
        public async Task CreateRoom_WithInvalidData_ReturnsBadRequest()
        {
            
            // Arrange
            var existRoom = GlobalHelper.GenerateRoom();

            CreateRoomForm createForm = new()
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
                s => s.CreateRoomAsync(It.IsAny<CreateRoomForm>(), It.IsAny<User>())
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
            CreateRoomForm createForm = new()
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

            
            _roomService.Setup(s => s.CreateRoomAsync(It.IsAny<CreateRoomForm>(), It.IsAny<User>()))
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

        [Fact]
        public async Task UpdateRoom_RoomDoesntExist_ReturnsBadResult()
        {
            var room = GlobalHelper.GenerateRoom();
            var user = GlobalHelper.GenerateExampleUser();
            _roomRepository.Setup(repo => repo.GetRoomAsync(room.Name)).ReturnsAsync((Room) null);
            _roomRepository.Setup(repo => repo.GetRoomByNameAsync(It.IsAny<string>())).ReturnsAsync(room);
            UpdateRoomForm form = new()
            {
                Name = "test",
                NewName = "test2",
                Password = "name"
            };
            room.Password = BCrypt.Net.BCrypt.HashPassword("name");
            
            ClaimsIdentity claimsIdentity = new ClaimsIdentity();
            var userAuth = GlobalHelper.FakeAuthenticationUser();
            HelperService helperService = new HelperService(_roomRepository.Object, _userToRoomRepository.Object);
            RoomService roomService =
                new RoomService(_roomRepository.Object, _userToRoomRepository.Object, helperService, _userRepository.Object);
            
            
            var owner = GlobalHelper.GenerateExampleUser();
            _identityService.Setup(repo => repo.GetCurrentUserAsync(claimsIdentity)).ReturnsAsync(user);


            var controller = new RoomController(_identityService.Object, _userService.Object, roomService);
            controller.ControllerContext = new ControllerContext();  
            controller.ControllerContext.HttpContext = new DefaultHttpContext { User = userAuth };
            
            // Act
            var result = await controller.UpdateRoomAsync(It.IsAny<string>() ,form);
            // Arrange
            result.Result.Should().BeOfType<BadRequestObjectResult>();
            RoomAction roomAction = (result.Result as BadRequestObjectResult).Value as RoomAction;
            roomAction.Error.Should().BeSameAs("Room with this name exist");
            roomAction.IsSuccessfully.Should().BeFalse();

        }

        [Fact]
        public async Task UpdateRoom_InvalidRole_ReturnsBadResult()
        {
            var room = GlobalHelper.GenerateRoom();
            var user = GlobalHelper.GenerateExampleUser();
            _roomRepository.Setup(repo => repo.GetRoomAsync(It.IsAny<string>())).ReturnsAsync(room);
            var utr = new UserToRoom()
            {
                Roles = UserToRoom.SimpleUser,
                User = user.Id,
                Room = room.Id
            };

            _userToRoomRepository.Setup(repo => repo.GetUserToRoomAsync(It.IsAny<User>(), It.IsAny<Room>())).ReturnsAsync(utr);
            UpdateRoomForm form = new()
            {
                Name = "test",
                NewName = "test2",
                Password = "name"
            };
            room.Password = BCrypt.Net.BCrypt.HashPassword("name");
            
            ClaimsIdentity claimsIdentity = new ClaimsIdentity();
            var userAuth = GlobalHelper.FakeAuthenticationUser();
            HelperService helperService = new HelperService(_roomRepository.Object, _userToRoomRepository.Object);
            RoomService roomService =
                new RoomService(_roomRepository.Object, _userToRoomRepository.Object, helperService, _userRepository.Object);
            
            
            var owner = GlobalHelper.GenerateExampleUser();
            _identityService.Setup(repo => repo.GetCurrentUserAsync(claimsIdentity)).ReturnsAsync(user);


            var controller = new RoomController(_identityService.Object, _userService.Object, roomService);
            controller.ControllerContext = new ControllerContext();  
            controller.ControllerContext.HttpContext = new DefaultHttpContext { User = userAuth };
            
            // Act
            var result = await controller.UpdateRoomAsync(It.IsAny<string>() ,form);
            // Arrange
            result.Result.Should().BeOfType<BadRequestObjectResult>();
            RoomAction roomAction = (result.Result as BadRequestObjectResult).Value as RoomAction;
            roomAction.Error.Should().BeSameAs("Invalid role");
            roomAction.IsSuccessfully.Should().BeFalse();
        }
        
        [Fact]
        public async Task UpdateRoom_WithValidData_ReturnsOkRequest()
        {
            var room = GlobalHelper.GenerateRoom();
            var user = GlobalHelper.GenerateExampleUser();
            _roomRepository.Setup(repo => repo.GetRoomAsync(It.IsAny<string>())).ReturnsAsync(room);
            var utr = new UserToRoom()
            {
                Roles = UserToRoom.Administration,
                User = user.Id,
                Room = room.Id
            };

            _userToRoomRepository.Setup(repo => repo.GetUserToRoomAsync(It.IsAny<User>(), It.IsAny<Room>())).ReturnsAsync(utr);
            UpdateRoomForm form = new()
            {
                Name = "test",
                NewName = "test2",
                Password = "name"
            };
            room.Password = BCrypt.Net.BCrypt.HashPassword("name");
            
            ClaimsIdentity claimsIdentity = new ClaimsIdentity();
            var userAuth = GlobalHelper.FakeAuthenticationUser();
            HelperService helperService = new HelperService(_roomRepository.Object, _userToRoomRepository.Object);
            RoomService roomService =
                new RoomService(_roomRepository.Object, _userToRoomRepository.Object, helperService, _userRepository.Object);
            
            
            var owner = GlobalHelper.GenerateExampleUser();
            _identityService.Setup(repo => repo.GetCurrentUserAsync(claimsIdentity)).ReturnsAsync(user);


            var controller = new RoomController(_identityService.Object, _userService.Object, roomService);
            controller.ControllerContext = new ControllerContext();  
            controller.ControllerContext.HttpContext = new DefaultHttpContext { User = userAuth };
            
            // Act
            var result = await controller.UpdateRoomAsync(It.IsAny<string>() ,form);
            // Arrange
            result.Result.Should().BeOfType<OkObjectResult>();

            RoomAction roomAction = (result.Result as OkObjectResult)?.Value as RoomAction;
            roomAction.Error.Should().BeNullOrEmpty();
            roomAction.IsSuccessfully.Should().BeTrue();

            
        }

        [Fact]
        public async Task AddUserToRoom_InvalidPassword_ReturnsBadRequest()
        {
            // Arrange
            var user = GlobalHelper.GenerateExampleUser();
            var owner = GlobalHelper.GenerateExampleUser();
            var room = GlobalHelper.GenerateRoom();
            room.Password = BCrypt.Net.BCrypt.HashPassword("test");
            HelperService helper = new HelperService(_roomRepository.Object, _userToRoomRepository.Object);

            _userRepository.Setup(repo => repo.GetUserAsync(It.IsAny<string>())).ReturnsAsync(user);
            _roomRepository.Setup(repo => repo.GetRoomAsync(It.IsAny<string>())).ReturnsAsync(room);
            
            
            RoomService roomService = new RoomService(_roomRepository.Object, _userToRoomRepository.Object, helper, _userRepository.Object);
            bool isCatch = false;
            AddUserFromRoom form = new()
            {
                UserId = user.Id,
                RoomPassword = "123",
                Role = UserToRoom.Administration
            };
            var controller = new RoomController(_identityService.Object, _userService.Object, roomService);
            
            // Act
            var result = await controller.AddUserToRoomAsync(It.IsAny<string>(), form);
            
            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();


        }

        [Fact]
        public async Task AddUserToRoom_InvalidRoles_ReturnsBadRequest()
        {
            var user = GlobalHelper.GenerateExampleUser();
            var owner = GlobalHelper.GenerateExampleUser();
            var room = GlobalHelper.GenerateRoom();
            room.Password = BCrypt.Net.BCrypt.HashPassword("test");
            
            _userRepository.Setup(repo => repo.GetUserAsync(It.IsAny<string>())).ReturnsAsync(user);
            _roomRepository.Setup(repo => repo.GetRoomAsync(It.IsAny<string>())).ReturnsAsync(room);
            
            HelperService helper = new HelperService(_roomRepository.Object, _userToRoomRepository.Object);
            AddUserFromRoom form = new()
            {
                UserId = user.Id,
                RoomPassword = "test",
                Role = 5
            };
            RoomService roomService = new RoomService(_roomRepository.Object, _userToRoomRepository.Object, helper, _userRepository.Object);
            var controller = new RoomController(_identityService.Object, _userService.Object, roomService);
            
            // Act
            var result = await controller.AddUserToRoomAsync(It.IsAny<string>(), form);
            
            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task AddUserToRoom_UserIsInRoom_ReturnsBadRequest()
        {
            // Arrange
            var user = GlobalHelper.GenerateExampleUser();
            var room = GlobalHelper.GenerateRoom();
            var owner = GlobalHelper.GenerateExampleUser();
            room.Password = BCrypt.Net.BCrypt.HashPassword("test");
            HelperService helper = new HelperService(_roomRepository.Object, _userToRoomRepository.Object);

            _userToRoomRepository.Setup(repo => repo.UserIsOnRoomAsync(user, room)).ReturnsAsync(true);
            _userRepository.Setup(repo => repo.GetUserAsync(It.IsAny<string>())).ReturnsAsync(user);
            _roomRepository.Setup(repo => repo.GetRoomAsync(It.IsAny<string>())).ReturnsAsync(room);

            AddUserFromRoom form = new()
            {
                UserId = user.Id,
                RoomPassword = "test"
            };

            RoomService roomService = new RoomService(_roomRepository.Object, _userToRoomRepository.Object, helper,
                _userRepository.Object);
            var controller = new RoomController(_identityService.Object, _userService.Object, roomService);

            // Act
            var result = await controller.AddUserToRoomAsync(It.IsAny<string>(), form);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task AddUserToRoom_ValidData_Ok()
        {
            // Arrange
            var user = GlobalHelper.GenerateExampleUser();
            var room = GlobalHelper.GenerateRoom();
            var owner = GlobalHelper.GenerateExampleUser();
            room.Password = BCrypt.Net.BCrypt.HashPassword("test");
            HelperService helper = new HelperService(_roomRepository.Object, _userToRoomRepository.Object);

            _userRepository.Setup(repo => repo.GetUserAsync(It.IsAny<string>())).ReturnsAsync(user);
            _roomRepository.Setup(repo => repo.GetRoomAsync(It.IsAny<string>())).ReturnsAsync(room);

            RoomService roomService = new RoomService(_roomRepository.Object, _userToRoomRepository.Object, helper,
                _userRepository.Object);

            AddUserFromRoom form = new()
            {
                UserId = user.Id,
                RoomPassword = "test"
            };
            var controller = new RoomController(_identityService.Object, _userService.Object, roomService);

            // Act
            var result = await controller.AddUserToRoomAsync(It.IsAny<string>(), form);

            // Assert
            result.Should().BeOfType<OkResult>();
        }


    }
}