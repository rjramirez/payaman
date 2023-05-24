namespace WebAPI.Controllers;

using Common.DataTransferObjects.AppUserDetails;
using Common.Entities;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using WebAPI.Services.Interfaces;
using Common.DataTransferObjects.ReferenceData;
using WebAPI.Authorization;
using AutoMapper;
using DataAccess.UnitOfWorks.RITSDB;
using BCrypt.Net;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AppUserController : ControllerBase
{
    private IUserService _userService;
    private readonly IMapper _mapper;
    private readonly IRITSDBUnitOfWork _RITSDBUnitOfWork;

    public AppUserController(IUserService userService, IMapper mapper, IRITSDBUnitOfWork RITSDBUnitOfWork)
    {
        _userService = userService;
        _mapper = mapper;
        _RITSDBUnitOfWork = RITSDBUnitOfWork;
    }

    [AllowAnonymous]
    [HttpPost("Authenticate")]
    [SwaggerOperation(Summary = "Authenticate User")]
    public async Task<IActionResult> Authenticate([FromBody] AuthenticateRequest model)
    {
        AuthenticateResponse authDetails = await _userService.Authenticate(model);

        // Create a new ClaimsIdentity with the desired claims
        //var claims = new[]
        //{
        //    new Claim(ClaimConstant.AppUserId, authDetails.Id.ToString()),
        //    new Claim(ClaimTypes.Name, authDetails.Username),
        //    new Claim("UserGivenName", authDetails.FirstName + " " + authDetails.LastName),
        //    new Claim(ClaimConstant.ClientId, authDetails.Username),
        //    new Claim("Token", authDetails.Token),
        //    new Claim(ClaimTypes.Role, authDetails.Role.ToString())
        //};
        //var claimsIdentity = new ClaimsIdentity(
        //    claims, CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);

        //var authProperties = new AuthenticationProperties { IsPersistent = true };
        //await HttpContext.SignInAsync(
        //    CookieAuthenticationDefaults.AuthenticationScheme,
        //    new ClaimsPrincipal(claimsIdentity),
        //    authProperties);


        return Ok(authDetails);
    }

    [HttpPost("Add")]
    [SwaggerOperation(Summary = "Add User")]
    public async Task<IActionResult> Add(AppUserDetail appUserDetail)
    {
        var user = await _RITSDBUnitOfWork.AppUserRepository.SingleOrDefaultAsync(predicate: a => a.AppUserId == appUserDetail.AppUserId && a.Active);

        // copy model to user and save
        _mapper.Map(appUserDetail, user);

        user.CreatedBy = appUserDetail.TransactionBy;
        user.CreatedDate = Convert.ToDateTime(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff"));
        user.Active = true;


        // validate
        if (appUserDetail.Username != user.Username && await _RITSDBUnitOfWork.AppUserRepository.IsExistAsync(x => x.Username == appUserDetail.Username))
            throw new Exception("Username '" + appUserDetail.Username + "' is already taken");

        // hash password if it was entered
        if (!string.IsNullOrEmpty(appUserDetail.Password))
            user.Password = BCrypt.HashPassword(appUserDetail.Password);

        await _RITSDBUnitOfWork.SaveChangesAsync(appUserDetail.TransactionBy);

        ClientResponse clientResponse = new()
        {
            Message = "User added successfully",
            IsSuccessful = true,
        };


        return Ok(clientResponse);
    }

    [AllowAnonymous]
    [HttpPost("Register")]
    [SwaggerOperation(Summary = "Register User")]
    public async Task<IActionResult> Register(RegisterRequest model)
    {
        RegisterResponse response = await _userService.Register(model);

        ClientResponse clientResponse = new()
        {
            IsSuccessful = true,
            Data = response,
            Message = "User successfully added"
        };

        return Ok(clientResponse);
    }

    [AllowAnonymous]
    [HttpGet("UserRole/{name}")]
    [SwaggerOperation(Summary = "User Role")]
    public async Task<IActionResult> UserRole(string name)
    {
        var user = await _userService.GetUserRoleByName(name);
        return Ok(user);
    }

    [HttpGet]
    [Authorize(Role.Admin)]
    [Route("GetAllAppUsers")]
    public async Task<IActionResult> GetAllAppUsers()
    {
        var users = await _userService.GetAll();

        IEnumerable<AppUserDetail> appUserDetail = users.Select(u => new AppUserDetail()
        {
            AppUserId = u.AppUserId,
            FirstName = u.FirstName,
            LastName = u.LastName,
            Username = u.Username,
            AppUserRole = new ReferenceDataDetail() { Active = true, Name = u.AppUserRole.Name, Value = u.AppUserRole.AppUserRoleId },
            CreatedBy = u.CreatedBy,
            CreatedDate = u.CreatedDate,
            ModifiedBy = u.ModifiedBy,
            ModifiedDate = u.ModifiedDate == null ? DateTime.MinValue : u.ModifiedDate.Value,
        });

        return Ok(appUserDetail);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var user = await _userService.GetById(id);
        return Ok(user);
    }

    [HttpPut]
    [Authorize(Role.Admin)]
    [Route("Update")]
    [SwaggerOperation(Summary = "Update App User")]
    public async Task<IActionResult> Update(AppUserDetail appUserDetail)
    {
        var user = await _RITSDBUnitOfWork.AppUserRepository.SingleOrDefaultAsync(predicate: a => a.AppUserId == appUserDetail.AppUserId && a.Active);

        // copy model to user and save
        _mapper.Map(appUserDetail, user);

        user.CreatedBy = appUserDetail.TransactionBy;
        user.CreatedDate = DateTime.UtcNow;

        // validate
        if (appUserDetail.Username != user.Username && await _RITSDBUnitOfWork.AppUserRepository.IsExistAsync(x => x.Username == appUserDetail.Username))
            throw new Exception("Username '" + appUserDetail.Username + "' is already taken");

        // hash password if it was entered
        if (!string.IsNullOrEmpty(appUserDetail.Password))
            user.Password = BCrypt.HashPassword(appUserDetail.Password);

        await _RITSDBUnitOfWork.SaveChangesAsync(appUserDetail.TransactionBy);

        ClientResponse clientResponse = new()
        {
            Message = "User updated successfully",
            IsSuccessful = true,
        };


        return Ok(clientResponse);
    }

    [HttpDelete("{id}")]
    [Authorize(Role.Admin)]
    public IActionResult Delete(int id)
    {
        _userService.Delete(id);
        return Ok(new { message = "User deleted successfully" });
    }
}