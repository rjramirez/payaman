<<<<<<< HEAD
﻿using DataAccess.Services;
using Common.DataTransferObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
=======
﻿namespace WebAPI.Controllers;

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
>>>>>>> dev

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
//[Authorize(Policy = "SystemLog")]
[ApiVersion("1.0")]
public class UserController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;

    public UserController(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    [AllowAnonymous]
    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
<<<<<<< HEAD
        var response = await _authenticationService.Login(request);

        return Ok(response);
=======
        AuthenticateResponse authDetails = await _userService.Authenticate(model);

        // Create a new ClaimsIdentity with the desired claims
        //var claims = new[]
        //{
        //            new Claim(ClaimTypes.PrimarySid, authDetails.Id.ToString()),
        //            new Claim(ClaimTypes.Name, authDetails.Username),
        //            new Claim(ClaimConstant.ClientId, authDetails.Username),
        //            new Claim(ClaimTypes.Role, authDetails.Role.ToString())
        //        };
        //var claimsIdentity = new ClaimsIdentity(
        //    claims, CookieAuthenticationDefaults.AuthenticationScheme);

        //var authProperties = new AuthenticationProperties { IsPersistent = true };
        //await HttpContext.SignInAsync(
        //    CookieAuthenticationDefaults.AuthenticationScheme,
        //    new ClaimsPrincipal(claimsIdentity),
        //    authProperties);

        return Ok(authDetails);
>>>>>>> dev
    }

    [AllowAnonymous]
    [HttpPost("Register")]
<<<<<<< HEAD
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var response = await _authenticationService.Register(request);

        return Ok(response);
=======
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
>>>>>>> dev
    }
}

<<<<<<< HEAD
=======
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
>>>>>>> dev
