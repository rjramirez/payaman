using Common.DataTransferObjects.AppUser;

namespace WebAPI.Services.Interfaces
{
    public interface IAuthenticationService
    {
        Task<string> Register(RegisterRequestDetail request);
        Task<string> Login(AuthenticateRequestDetail request);
    }

}
