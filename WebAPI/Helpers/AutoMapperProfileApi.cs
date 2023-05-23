namespace DataAccess.Helpers;

using AutoMapper;
using Common.DataTransferObjects.AppUserDetails;
using Common.DataTransferObjects.Order;
using Common.DataTransferObjects.Product;
using Common.DataTransferObjects.Store;
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

        // ProductDetail -> Product
        CreateMap<ProductDetail, Product>()
            .ForMember(dest => dest.CreatedBy, act => act.MapFrom(src => src.TransactionBy))
            .ForAllMembers(x => x.Condition(
                (src, dest, prop) =>
                {
                    // ignore null & empty string properties
                    if (prop == null) return false;
                    if (prop.GetType() == typeof(string) && string.IsNullOrEmpty((string)prop)) return false;

                    return true;
                }
            ));

        // OrderDetail -> Order
        CreateMap<OrderDetail, Order>()
            .ForMember(dest => dest.CreatedBy, act => act.MapFrom(src => src.TransactionBy))
            .ForAllMembers(x => x.Condition(
                (src, dest, prop) =>
                {
                    // ignore null & empty string properties
                    if (prop == null) return false;
                    if (prop.GetType() == typeof(string) && string.IsNullOrEmpty((string)prop)) return false;

                    return true;
                }
            ));

        // OrderItemDetail -> OrderItem
        CreateMap<OrderItemDetail, OrderItem>()
            .ForMember(dest => dest.CreatedBy, act => act.MapFrom(src => src.TransactionBy))
            .ForAllMembers(x => x.Condition(
                (src, dest, prop) =>
                {
                    // ignore null & empty string properties
                    if (prop == null) return false;
                    if (prop.GetType() == typeof(string) && string.IsNullOrEmpty((string)prop)) return false;

                    return true;
                }
            ));

        // StoreDetail -> Store
        CreateMap<StoreDetail, Store>()
            .ForMember(dest => dest.CreatedBy, act => act.MapFrom(src => src.TransactionBy))
            .ForAllMembers(x => x.Condition(
                (src, dest, prop) =>
                {
                    // ignore null & empty string properties
                    if (prop == null) return false;
                    if (prop.GetType() == typeof(string) && string.IsNullOrEmpty((string)prop)) return false;

                    return true;
                }
            ));


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


        // UPDATE AppUserDetail -> AppUser
        CreateMap<AppUserDetail, AppUser>()
            .ForAllMembers(x => x.Condition(
                (src, dest, prop) =>
                {
                    // ignore null & empty string properties
                    if (prop == null) return false;
                    if (prop.GetType() == typeof(string) && string.IsNullOrEmpty((string)prop)) return false;

                    dest.ModifiedDate = DateTime.UtcNow;
                    dest.ModifiedBy = src.TransactionBy;
                    
                    return true;
                }
            ));
    }
}