using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Chater.Dtos.Message.Form;
using Chater.Models;
using Chater.Repository.Abstract;
using Chater.Service.Abstract.HelperService;
using MongoDB.Bson;

namespace Chater.Service.Concrete
{
    public class MessageService : IMessageService
    {
        private readonly IRoomRepository _roomRepository;
        private readonly IHelperService _helperService;
        
        public MessageService(IRoomRepository roomRepository, IHelperService helperService)
        {
            _roomRepository = roomRepository;
            _helperService = helperService;
        }
        
        public async Task NewMessage(NewMessageForm form, User user, string roomId)
        {
            Room? room = await _roomRepository.GetRoomAsync(roomId);
            await _helperService.VerificationDataBeforeSendMessage(user, room);
            await SaveMessage(form, room, user);
        }

        private async Task SaveMessage(NewMessageForm form, Room room, User user)
        {
            var newMessage = new Message()
            {
                SendAt = DateTime.Now,
                Text = form.Text,
                UserId = user.Id
            };
            if (room.Messages is null)
                room.Messages = new List<Message>();
            room.Messages.Add(newMessage);
            await _roomRepository.UpdateRoomAsync(room);
        }
    }
}