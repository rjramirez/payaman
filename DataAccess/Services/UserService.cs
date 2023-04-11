namespace DataAccess.Services;

using AutoMapper;
using BCrypt.Net;
using DataAccess.Authorization;
using DataAccess.DBContexts.PayamanDB;
using DataAccess.DBContexts.PayamanDB.Models;
using DataAccess.Services.Interfaces;
using Common.DataTransferObjects.AppUser;

public class UserService : IUserService
{
    private IJwtUtils _jwtUtils;
    private readonly IMapper _mapper;
    private readonly PayamanDBContext _context;

    public UserService(
        IJwtUtils jwtUtils,
        IMapper mapper,
        PayamanDBContext context)
    {
        _jwtUtils = jwtUtils;
        _mapper = mapper;
        _context = context;
    }

    public AuthenticateResponse Authenticate(AuthenticateRequest model)
    {
        var user = _context.AppUsers.SingleOrDefault(x => x.UserName == model.Username);

        // validate
        if (user == null || !BCrypt.Verify(model.Password, user.PasswordHash))
            throw new Exception("Username or password is incorrect");

        // authentication successful
        var response = _mapper.Map<AuthenticateResponse>(user);
        response.Token = _jwtUtils.GenerateToken(user);
        return response;
    }

    public IEnumerable<AppUser> GetAll()
    {
        return _context.AppUsers;
    }

    public AppUser GetById(int id)
    {
        return getUser(id);
    }

    public void Register(RegisterRequest model)
    {
        // validate
        if (_context.AppUsers.Any(x => x.UserName == model.Username))
            throw new Exception("Username '" + model.Username + "' is already taken");

        // map model to new user object
        var user = _mapper.Map<AppUser>(model);

        // hash password
        user.PasswordHash = BCrypt.HashPassword(model.Password);

        // save user
        _context.AppUsers.Add(user);
        _context.SaveChanges();
    }

    public void Update(int id, UpdateRequest model)
    {
        var user = getUser(id);

        // validate
        if (model.Username != user.UserName && _context.AppUsers.Any(x => x.UserName == model.Username))
            throw new Exception("Username '" + model.Username + "' is already taken");

        // hash password if it was entered
        if (!string.IsNullOrEmpty(model.Password))
            user.PasswordHash = BCrypt.HashPassword(model.Password);

        // copy model to user and save
        _mapper.Map(model, user);
        _context.AppUsers.Update(user);
        _context.SaveChanges();
    }

    public void Delete(int id)
    {
        var user = getUser(id);
        _context.AppUsers.Remove(user);
        _context.SaveChanges();
    }

    // helper methods

    private AppUser getUser(int id)
    {
        var user = _context.AppUsers.Find(id);
        if (user == null) throw new KeyNotFoundException("User not found");
        return user;
    }
}