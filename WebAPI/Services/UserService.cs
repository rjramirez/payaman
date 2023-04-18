namespace WebAPI.Services;

using AutoMapper;
using BCrypt.Net;
using Common.DataTransferObjects.AppUserDetails;
using DataAccess.DBContexts.RITSDB.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using WebAPI.Services.Interfaces;
using DataAccess.UnitOfWorks.RITSDB;
using WebAPI.Authorization;
using Common.Entities;

public class UserService : IUserService
{
    private IJwtUtils _jwtUtils;
    private readonly IMapper _mapper;
    private readonly IRITSDBUnitOfWork _RITSDBUnitOfWork;
    private IHttpContextAccessor _httpContextAccessor;

    public UserService(
        IJwtUtils jwtUtils,
        IMapper mapper,
        IRITSDBUnitOfWork ritsDBUnitOfWork,
        IHttpContextAccessor httpContextAccessor)
    {
        _jwtUtils = jwtUtils;
        _mapper = mapper;
        _RITSDBUnitOfWork = ritsDBUnitOfWork;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<AuthenticateResponse> Authenticate(AuthenticateRequest model)
    {
        var user = await _RITSDBUnitOfWork.AppUserRepository.SingleOrDefaultAsync(x => x.Username == model.Username);

        // validate
        if (user == null || !BCrypt.Verify(model.Password, user.Password))
            throw new ArgumentException("Username or password is incorrect");

        // authentication successful
        var response = _mapper.Map<AuthenticateResponse>(user);
        response.Token = _jwtUtils.GenerateToken(user);

        var userRole = await _RITSDBUnitOfWork.AppUserRoleRepository.SingleOrDefaultAsync(x => x.AppUserId == user.Id);
        var role = await _RITSDBUnitOfWork.RoleRepository.SingleOrDefaultAsync(x => x.Id == userRole.Id);


        // Create a new ClaimsIdentity with the desired claims
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, role.Name)
        };
        var identity = new ClaimsIdentity(claims, "User");

        // Create a new ClaimsPrincipal with the custom identity
        var principal = new ClaimsPrincipal(identity);

        // Set the HttpContext.User property to the custom principal
        _httpContextAccessor.HttpContext.User = principal;


        return response;
    }

    public async Task<IEnumerable<AppUser>> GetAll()
    {
        return await _RITSDBUnitOfWork.AppUserRepository.GetAllAsync();
    }

    public async Task<AppUser> GetById(int id)
    {
        return await getUser(id);
    }

    public async void Register(RegisterRequest model)
    {
        // validate
        if (await _RITSDBUnitOfWork.AppUserRepository.IsExistAsync(x => x.Username == model.Username))
            throw new Exception("Username '" + model.Username + "' is already taken");

        // map model to new user object
        var user = _mapper.Map<AppUser>(model);

        // hash password
        user.Password = BCrypt.HashPassword(model.Password);

        // save user
        await _RITSDBUnitOfWork.AppUserRepository.AddAsync(user);
        await _RITSDBUnitOfWork.SaveChangesAsync(model.Username);
    }

    public async void Update(int id, UpdateRequest model)
    {
        var user = await getUser(id);

        // validate
        if (model.Username != user.Username && await _RITSDBUnitOfWork.AppUserRepository.IsExistAsync(x => x.Username == model.Username))
            throw new Exception("Username '" + model.Username + "' is already taken");

        // hash password if it was entered
        if (!string.IsNullOrEmpty(model.Password))
            user.Password = BCrypt.HashPassword(model.Password);

        // copy model to user and save
        _mapper.Map(model, user);

        await _RITSDBUnitOfWork.SaveChangesAsync(model.Username);
    }

    public async void Delete(int id)
    {
        var user = await getUser(id);

        user.IsActive = false;
        string name = _httpContextAccessor.HttpContext.User.Identity.Name;

        await _RITSDBUnitOfWork.SaveChangesAsync(name);
    }

    //// helper methods

    private async Task<AppUser> getUser(int id)
    {
        var user = await _RITSDBUnitOfWork.AppUserRepository.SingleOrDefaultAsync(x=> x.Id == id);
        if (user == null) throw new KeyNotFoundException("User not found");
        return user;
    }
}