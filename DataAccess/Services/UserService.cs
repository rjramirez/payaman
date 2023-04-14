namespace DataAccess.Services;

using AutoMapper;
using BCrypt.Net;
using DataAccess.Services.Interfaces;
using Common.DataTransferObjects.AppUserDetails;
using DataAccess.DBContexts.RITSDB;
using DataAccess.DBContexts.RITSDB.Models;
using DataAccess.Authorization;
using Microsoft.EntityFrameworkCore;

public class UserService : IUserService
{
    private IJwtUtils _jwtUtils;
    private readonly IMapper _mapper;
    private readonly RITSDBContext _context;

    public UserService(
        IJwtUtils jwtUtils,
        IMapper mapper,
        RITSDBContext context)
    {
        _jwtUtils = jwtUtils;
        _mapper = mapper;
        _context = context;
    }

    public async Task<AuthenticateResponse> Authenticate(AuthenticateRequest model)
    {
        var user = await _context.AppUsers.SingleOrDefaultAsync(x => x.Username == model.Username);

        // validate
        if (user == null || !BCrypt.Verify(model.Password, user.Password))
            throw new ArgumentException("Username or password is incorrect");

        // authentication successful
        var response = _mapper.Map<AuthenticateResponse>(user);
        response.Token = _jwtUtils.GenerateToken(user);
        return response;
    }

    public IEnumerable<AppUser> GetAll()
    {
        return _context.AppUsers;
    }

    public async Task<AppUser> GetById(int id)
    {
        return await getUser(id);
    }

    public async void Register(RegisterRequest model)
    {
        // validate
        if (await _context.AppUsers.AnyAsync(x => x.Username == model.Username))
            throw new Exception("Username '" + model.Username + "' is already taken");

        // map model to new user object
        var user = _mapper.Map<AppUser>(model);

        // hash password
        user.Password = BCrypt.HashPassword(model.Password);

        // save user
        await _context.AppUsers.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    public async void Update(int id, UpdateRequest model)
    {
        var user = await getUser(id);

        // validate
        if (model.Username != user.Username && _context.AppUsers.Any(x => x.Username == model.Username))
            throw new Exception("Username '" + model.Username + "' is already taken");

        // hash password if it was entered
        if (!string.IsNullOrEmpty(model.Password))
            user.Password = BCrypt.HashPassword(model.Password);

        // copy model to user and save
        _mapper.Map(model, user);
       _context.AppUsers.Update(user);
       await _context.SaveChangesAsync();
    }

    public async void Delete(int id)
    {
        var user = await getUser(id);
        _context.AppUsers.Remove(user);
        await _context.SaveChangesAsync();
    }

    // helper methods

    private async Task<AppUser> getUser(int id)
    {
        var user = await _context.AppUsers.FindAsync(id);
        if (user == null) throw new KeyNotFoundException("User not found");
        return user;
    }
}