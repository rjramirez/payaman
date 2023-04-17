using Common.DataTransferObjects;

namespace AuthenticationApi.Services;

public interface IAuthenticationService
{
    Task<string> Register(RegisterRequest request);
    Task<string> Login(LoginRequest request);
}

