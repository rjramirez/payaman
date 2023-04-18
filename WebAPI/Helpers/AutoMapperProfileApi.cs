namespace DataAccess.Helpers;

using AutoMapper;
using Common.DataTransferObjects.AppUserDetails;
using DataAccess.DBContexts.RITSDB.Models;

public class AutoMapperProfileApi : Profile
{
    public AutoMapperProfileApi()
    {
        // AppUser -> AuthenticateResponse
        CreateMap<AppUser, AuthenticateResponse>();

        // RegisterRequest -> AppUser
        CreateMap<RegisterRequest, AppUser>();

        // RegisterRequest -> AppUser
        CreateMap<AppUser, RegisterResponse>();

        // UpdateRequest -> AppUser
        CreateMap<UpdateRequest, AppUser>()
            .ForAllMembers(x => x.Condition(
                (src, dest, prop) =>
                {
                    // ignore null & empty string properties
                    if (prop == null) return false;
                    if (prop.GetType() == typeof(string) && string.IsNullOrEmpty((string)prop)) return false;

                    return true;
                }
            ));
    }
}