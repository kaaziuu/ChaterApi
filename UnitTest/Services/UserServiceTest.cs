using System.Collections.Generic;
using System.Threading.Tasks;
using Chater.Models;
using Chater.Repository.Abstract;
using Chater.Service.Concrete;
using FluentAssertions;
using Moq;
using Xunit;

namespace UnitTest
{
    public class UserServiceTest
    {
        private readonly Mock<IUserToRoomRepository> _userToRoomRepo = new();

        [Fact]
         public async Task GetUserRoom_WithExistingRoom_ReturnsICollectionWithUserRoom()
        {
            // Arrange
            Room[] exampleRoom =
                {GlobalHelper.GenerateRoom(), GlobalHelper.GenerateRoom(), GlobalHelper .GenerateRoom()};
            
            User exampleUser = GlobalHelper.GenerateExampleUser();
            ICollection<UserToRoom> userToRooms = new List<UserToRoom>();
            userToRooms.Add(GlobalHelper.AssignUserToRoom(exampleUser, exampleRoom[0]));
            userToRooms.Add(GlobalHelper.AssignUserToRoom(exampleUser, exampleRoom[1]));
            _userToRoomRepo.Setup(repo => repo.GetUserRoomAsync(exampleUser)).ReturnsAsync(userToRooms);
            var service = new UserService(_userToRoomRepo.Object);
            
            // Act
            ICollection<Room> rooms = await service.GetUserRoomsAsync(exampleUser);
            // Assert

            rooms.Should().OnlyContain(r => r.Id == exampleRoom[0].Id || r.Id == exampleRoom[2].Id);
        }

         [Fact]
         public async Task GeyUseRoom_WhenUserDontHaveAnyRoom_ReturnsEmptyICollection()
         {
            // Arrange
             Room[] exampleRoom =
                 {GlobalHelper.GenerateRoom(), GlobalHelper.GenerateRoom(), GlobalHelper .GenerateRoom()};
            
             User exampleUser = GlobalHelper.GenerateExampleUser();
             ICollection<UserToRoom> userToRooms = new List<UserToRoom>();
             _userToRoomRepo.Setup(repo => repo.GetUserRoomAsync(exampleUser)).ReturnsAsync(userToRooms);

             var service = new UserService(_userToRoomRepo.Object);
            
             // Act
             ICollection<Room> rooms = await service.GetUserRoomsAsync(exampleUser);
             // Assert

             rooms.Count.Should().Equals(0);


         }
    }
}