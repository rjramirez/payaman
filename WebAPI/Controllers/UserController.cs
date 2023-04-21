namespace WebAPI.Controllers;

using Common.DataTransferObjects.AppUserDetails;
using Common.Entities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using WebAPI.Authorization;
using WebAPI.Services.Interfaces;
using Common.Constants;

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