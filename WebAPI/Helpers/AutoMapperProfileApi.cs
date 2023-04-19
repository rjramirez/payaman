namespace DataAccess.Helpers;

using AutoMapper;
using Common.DataTransferObjects;
using Common.DataTransferObjects.AppUserDetails;
using DataAccess.DBContexts.RITSDB.Models;

public class AutoMapperProfileApi : Profile
{
    public AutoMapperProfileApi()
    {
        // AppUser -> AuthenticateResponse
        //CreateMap<AppUser, AuthenticateResponse>();

        // RegisterRequest -> AppUser
        CreateMap<AppUserDetail, RegisterRequest>();

<<<<<<< HEAD
        //// UpdateRequest -> AppUser
        //CreateMap<UpdateRequest, AppUser>()
        //    .ForAllMembers(x => x.Condition(
        //        (src, dest, prop) =>
        //        {
        //            // ignore null & empty string properties
        //            if (prop == null) return false;
        //            if (prop.GetType() == typeof(string) && string.IsNullOrEmpty((string)prop)) return false;
=======
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
>>>>>>> dev

        //            return true;
        //        }
        //    ));
    }
}