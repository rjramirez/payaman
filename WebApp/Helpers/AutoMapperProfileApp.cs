namespace DataAccess.Helpers;

using AutoMapper;
using Common.DataTransferObjects;
using Common.DataTransferObjects.AppUserDetails;

public class AutoMapperProfileApp : Profile
{
    public AutoMapperProfileApp()
    {
        // AppUserDetail -> AuthenticateRequest
        CreateMap<AppUserDetail, LoginRequest>();
    }
}