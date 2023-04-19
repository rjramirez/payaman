using Common.DataTransferObjects.AppUserDetails;
using Common.DataTransferObjects.ReferenceData;
using Common.Entities;
using DataAccess.DBContexts.RITSDB.Models;

namespace WebAPI.Services.Interfaces
{
    public interface IUserService
    {
        Task<AuthenticateResponse> Authenticate(AuthenticateRequest model);
        Task<IEnumerable<AppUser>> GetAll();
        Task<ReferenceDataDetail> GetUserRoleByName(string name);
        Task<AppUser> GetById(int id);
        Task<RegisterResponse> Register(RegisterRequest model);
        void Update(int id, UpdateRequest model);
        void Delete(int id);
    }
}
