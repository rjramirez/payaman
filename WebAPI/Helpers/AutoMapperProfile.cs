namespace DataAccess.Helpers;

using AutoMapper;
using Common.DataTransferObjects.AppUser;
using Common.Entities;
using DataAccess.DBContexts.RITSDB.Models;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        // User -> AuthenticateResponse
        CreateMap<AppUser, AuthenticateResponse>();

        // RegisterRequest -> User
        CreateMap<RegisterRequest, AppUser>();

        // UpdateRequest -> User
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