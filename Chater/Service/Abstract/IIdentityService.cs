using System.Security.Claims;
using System.Threading.Tasks;
using Chater.Models;

namespace Chater.Service.Abstract
{
    public interface IIdentityService
    {
        public Task<string> AuthenticateAsync(string username, string password);

        Task<User> GetCurrentUserAsync(ClaimsIdentity claimsIdentity);
    }
}