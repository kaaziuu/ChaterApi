using System;
using System.Threading.Tasks;
using Chater.Dtos.Message.Form;
using Chater.Exception;
using Chater.Models;
using Chater.Repository.Abstract;
using Chater.Service.Concrete;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;

namespace UnitTest.Services
{
    public class MessageServiceTest
    {
        private readonly Mock<IRoomRepository> _roomRepository = new();
        private readonly Mock<IUserToRoomRepository> _userToRoom = new();

        private NewMessageForm GenerateNewMessageForm() => new NewMessageForm()
        {
            Text = "blablabla"
        };

        [Fact]
        public async Task NewMessage_UserIsNotInRoom_ThrowExceptions()
        {
            // Arrange
            var room = GlobalHelper.GenerateRoom();
            var user = GlobalHelper.GenerateExampleUser();

            var message = GenerateNewMessageForm();
            
            _roomRepository.Setup(repo => repo.GetRoomAsync(It.IsAny<string>())).ReturnsAsync(room);
            _userToRoom.Setup(
                    repo => repo.GetUserToRoomAsync(It.IsAny<User>(), It.IsAny<Room>()))
                            .ReturnsAsync((UserToRoom) null);

            var helper = new Chater.Service.Concrete.HelperService.HelperService(_roomRepository.Object, _userToRoom.Object);
            
            var service = new MessageService(_roomRepository.Object, helper);
            
            // Act
            Func<Task> action = async () => await service.NewMessage(message, user, It.IsAny<string>());
            // Assert
            await action.Should().ThrowAsync<UserIsNotInRoomException>();
            
        }

        [Fact]
        public async Task NewMessage_RoomDoesntExist_ThrowExceptions()
        {
            Room room = null;
            var user = GlobalHelper.GenerateExampleUser();
            var message = GenerateNewMessageForm();
            _roomRepository.Setup(repo => repo.GetRoomAsync(It.IsAny<string>())).ReturnsAsync((Room) null);
            _userToRoom.Setup(
                    repo => repo.GetUserToRoomAsync(It.IsAny<User>(), It.IsAny<Room>()))
                .ReturnsAsync((UserToRoom)null);

            var helper = new Chater.Service.Concrete.HelperService.HelperService(_roomRepository.Object, _userToRoom.Object);
            
            var service = new MessageService(_roomRepository.Object, helper);
            
            // Act
            Func<Task> action = async () => await service.NewMessage(message, user, It.IsAny<string>());
            // Assert
            await action.Should().ThrowAsync<RoomDoesntExistExceptionException>().WithMessage("Room doesnt exist");


        }

        [Fact]
        public async Task NewMessage_ValidData_EndWithoutExceptions()
        {
            Room room = GlobalHelper.GenerateRoom();
            var user = GlobalHelper.GenerateExampleUser();
            var message = GenerateNewMessageForm();
            var utr = GlobalHelper.AssignUserToRoom(user, room);
            
            _roomRepository.Setup(repo => repo.GetRoomAsync(It.IsAny<string>())).ReturnsAsync(room);
            _userToRoom.Setup(
                    repo => repo.UserIsOnRoomAsync(It.IsAny<User>(), It.IsAny<Room>()))
                .ReturnsAsync(true);

            var helper = new Chater.Service.Concrete.HelperService.HelperService(_roomRepository.Object, _userToRoom.Object);
            
            var service = new MessageService(_roomRepository.Object, helper);
            
            // Act
            Func<Task> action = async () => await service.NewMessage(message, user, It.IsAny<string>());
            // Assert
            await action.Should().NotThrowAsync();

        }
    }
}