using System.Threading.Tasks;
using Chater.Dtos.Message.Form;
using Chater.Models;
using Chater.Repository.Abstract;

namespace Chater.Service.Concrete
{
    public class MessageService : IMessageService
    {
        private readonly IRoomRepository _roomRepository;
        public MessageService(IRoomRepository roomRepository)
        {
            _roomRepository = roomRepository;
        }
        public Task NewMessage(NewMessageForm form, User user)
        {
            throw new System.NotImplementedException();
        }
    }
}