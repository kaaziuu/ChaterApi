using System.Threading.Tasks;
using Chater.Dtos.Message.Form;
using Chater.Models;

namespace Chater.Repository.Abstract
{
    public interface IMessageService
    {
        Task NewMessage(NewMessageForm form, User user);
    }
}