namespace WebAPI.Controllers;

using Common.Constants;
using Common.DataTransferObjects.AppUserDetails;
using Common.Entities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;
using WebAPI.Authorization;
using WebAPI.Services.Interfaces;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private IUserService _userService;

    public UserController(
        IUserService userService)
    {
        _userService = userService;
    }

    [AllowAnonymous]
    [HttpPost("Authenticate")]
    [SwaggerOperation(Summary = "Authenticate User")]
    public async Task<IActionResult> Authenticate([FromBody] AuthenticateRequest model)
    {
        AuthenticateResponse authDetails = await _userService.Authenticate(model);

        // Create a new ClaimsIdentity with the desired claims
        var claims = new[]
        {
                    new Claim(ClaimConstant.EmployeeId, authDetails.Id.ToString()),
                    new Claim(ClaimTypes.Name, authDetails.Username),
                    new Claim("UserGivenName", authDetails.FirstName + " " + authDetails.LastName),
                    new Claim(ClaimConstant.ClientId, authDetails.Username),
                    new Claim("Token", authDetails.Token),
                    new Claim(ClaimTypes.Role, authDetails.Role.ToString())
                };
        var claimsIdentity = new ClaimsIdentity(
            claims, CookieAuthenticationDefaults.AuthenticationScheme);

        var authProperties = new AuthenticationProperties { IsPersistent = true };
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);


        return Ok(authDetails);
    }

    [AllowAnonymous]
    [HttpPost("Register")]
    [SwaggerOperation(Summary = "Register User")]
    public async Task<IActionResult> Register(RegisterRequest model)
    {
        RegisterResponse response = await _userService.Register(model);

        return Ok(response);
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
    public IActionResult GetAll()
    {
        var users = _userService.GetAll();
        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var user = await _userService.GetById(id);
        return Ok(user);
    }

    [HttpPut("{id}")]
    [Authorize(Role.Admin)]
    public IActionResult Update(int id, UpdateRequest model)
    {
        _userService.Update(id, model);
        return Ok(new { message = "User updated successfully" });
    }

    [HttpDelete("{id}")]
    [Authorize(Role.Admin)]
    public IActionResult Delete(int id)
    {
        _userService.Delete(id);
        return Ok(new { message = "User deleted successfully" });
    }
}