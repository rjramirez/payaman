<<<<<<< HEAD:DataAccess/Authorization/JwtMiddleware.cs
namespace DataAccess.Authorization;

using DataAccess.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
=======
using WebAPI.Authorization;
using WebAPI.Services.Interfaces;
>>>>>>> dev:WebAPI/Authorization/JwtMiddleware.cs

public class JwtMiddleware
{
    private readonly RequestDelegate _next;

    public JwtMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, IUserService userService, IJwtUtils jwtUtils)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        var userId = jwtUtils.ValidateToken(token);
        if (userId != null)
        {
            // attach user to context on successful jwt validation
            context.Items["User"] = userService.GetById(userId.Value);
        }

        await _next(context);
    }
}