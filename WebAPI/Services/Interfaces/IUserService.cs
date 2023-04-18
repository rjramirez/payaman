using Common.DataTransferObjects.AppUserDetails;
using Common.Entities;
using DataAccess.DBContexts.RITSDB.Models;

namespace WebAPI.Services.Interfaces
{
    public interface IUserService
    {
        Task<AuthenticateResponse> Authenticate(AuthenticateRequest model);
        Task<IEnumerable<AppUser>> GetAll();
        Task<AppUser> GetById(int id);
        void Register(RegisterRequest model);
        void Update(int id, UpdateRequest model);
        void Delete(int id);
    }
}
