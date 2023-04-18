namespace WebAPI.Controllers;

using Common.DataTransferObjects.AppUserDetails;
using Common.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Runtime.CompilerServices;
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
        AuthenticateResponse response = await _userService.Authenticate(model);
        return Ok(response);
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
    [HttpGet("{name}")]
    [SwaggerOperation(Summary = "User Role")]
    public IActionResult UserRole(string name)
    {
        var user = _userService.GetByName(name);
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