using Common.DataTransferObjects.AppUserDetails;
using Common.Entities;
using DataAccess.DBContexts.RITSDB.Models;

namespace DataAccess.Services.Interfaces
{
    public interface IUserService
    {
        AuthenticateResponse Authenticate(AuthenticateRequest model);
        IEnumerable<AppUser> GetAll();
        AppUser GetById(int id);
        void Register(RegisterRequest model);
        void Update(int id, UpdateRequest model);
        void Delete(int id);
    }
}
