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
using Common.DataTransferObjects.ReferenceData;
using System.Security.Principal;
using Common.Constants;

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
        AuthenticateResponse response = _mapper.Map<AuthenticateResponse>(user);
        response.Token = _jwtUtils.GenerateToken(user);

        var role = await _RITSDBUnitOfWork.AppUserRoleRepository.SingleOrDefaultAsync(x => x.AppUserRoleId == user.AppUserRoleId);

        // Create a new ClaimsIdentity with the desired claims
        var claims = new[]
        {
            new Claim(ClaimConstant.AppUserId, user.AppUserId.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimConstant.ClientId, user.AppUserId.ToString()),
            new Claim(ClaimTypes.Role, user.AppUserRole.Name)
        };
        var identity = new ClaimsIdentity(claims, "User");

        // Create a new ClaimsPrincipal with the custom identity
        var principal = new ClaimsPrincipal(identity);
        response.Role = (Role)role.AppUserRoleId;

        // Set the HttpContext.User property to the custom principal
        _httpContextAccessor.HttpContext.User = principal;
        _httpContextAccessor.HttpContext.Items["User"] = user.AppUserId;
        _httpContextAccessor.HttpContext.Request.Headers["Authorization"] = String.Format("Bearer {0}", response.Token);


        return response;
    }

    public async Task<RegisterResponse> Register(RegisterRequest model)
    {
        // validate
        var checkUser = await _RITSDBUnitOfWork.AppUserRepository.SingleOrDefaultAsync(x => x.Username == model.Username);
        if (checkUser != null)
            throw new Exception("Username '" + model.Username + "' is already taken");

        // map model to new user object
        var user = _mapper.Map<AppUser>(model);

        // hash password
        user.Password = BCrypt.HashPassword(model.Password);

        user.CreatedDate = DateTime.UtcNow;
        user.CreatedBy = model.TransactionBy;
        user.Active = true;

        user.AppUserRoleId = 2; //Cashier by Default

        // save user
        await _RITSDBUnitOfWork.AppUserRepository.AddAsync(user);
        await _RITSDBUnitOfWork.SaveChangesAsync(model.Username);

        // map model to new user object
        RegisterResponse registeredUser = _mapper.Map<RegisterResponse>(user);

        return registeredUser;
    }

    public async Task<IEnumerable<AppUser>> GetAll()
    {

        return await _RITSDBUnitOfWork.AppUserRepository.FindAsync(
            selector: au => new AppUser() { 
                AppUserId = au.AppUserId,
                FirstName = au.FirstName,
                LastName = au.LastName,
                Username = au.Username,
                AppUserRole = au.AppUserRole,
            }, 
            predicate: a => a.Active == true);
    }

    public async Task<AppUser> GetById(int id)
    {
        return await getUser(id);
    }
    public async Task<ReferenceDataDetail> GetUserRoleByName(string name)
    {
        var appUserRole = await getUserRoleByName(name);
        return appUserRole;
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

        await _RITSDBUnitOfWork.SaveChangesAsync(model.TransactionBy);
    }

    public async void Delete(int id)
    {
        var user = await getUser(id);

        user.Active = false;
        string name = _httpContextAccessor.HttpContext.User.Identity.Name;

        await _RITSDBUnitOfWork.SaveChangesAsync(name);
    }

    //// helper methods

    private async Task<ReferenceDataDetail> getUserRoleByName(string name)
    {
        var user = await _RITSDBUnitOfWork.AppUserRepository.SingleOrDefaultAsync(x => x.Username == name);
        var userRole = await _RITSDBUnitOfWork.AppUserRoleRepository.SingleOrDefaultAsync(x => x.AppUserRoleId == user.AppUserRoleId);

        if (user == null) throw new KeyNotFoundException("User not found");

        ReferenceDataDetail referenceData = new()
        {
            Value = user.AppUserRoleId,
            Name = userRole.Name,
            Active = true,
        };

        return referenceData;
    }

    private async Task<AppUser> getUser(int id)
    {
        var user = await _RITSDBUnitOfWork.AppUserRepository.SingleOrDefaultAsync(x=> x.AppUserId == id);
        if (user == null) throw new KeyNotFoundException("User not found");
        return user;
    }
}