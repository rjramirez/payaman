namespace WebAPI.Authorization;

using Common.Constants;
using Common.Entities;
using DataAccess.DBContexts.RITSDB.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApi.Helpers;

public interface IJwtUtils
{
    public string GenerateToken(AppUser user);
    public int? ValidateToken(string token);
}

public class JwtUtils : IJwtUtils
{
    private readonly JwtSettings _jwtSettings;

    public JwtUtils(IOptions<JwtSettings> jwtSettings)
    {
        _jwtSettings = jwtSettings.Value;
    }

    public string GenerateToken(AppUser user)
    {
        // generate token that is valid for 7 days
        //var tokenHandler = new JwtSecurityTokenHandler();
        //var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);
        //var tokenDescriptor = new SecurityTokenDescriptor
        //{
        //    Subject = new ClaimsIdentity(new[] { 
        //        new Claim(ClaimConstant.AppUserId, user.AppUserId.ToString()),
        //        new Claim(ClaimTypes.Name, user.Username),
        //        new Claim("UserGivenName", user.FirstName + " " + user.LastName),
        //        new Claim(ClaimConstant.ClientId, user.Username),
        //        new Claim(ClaimTypes.Role, ((Role)user.AppUserRoleId).ToString()),
        //    }),
        //    Expires = DateTime.UtcNow.AddDays(7),
        //    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        //};
        //var token = tokenHandler.CreateToken(tokenDescriptor);
        var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);
        var authSigningKey = new SymmetricSecurityKey(key);

        var authClaims = new List<Claim>
        {
            new Claim(ClaimConstant.AppUserId, user.AppUserId.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim("UserGivenName", user.FirstName + " " + user.LastName),
            new Claim(ClaimConstant.ClientId, user.Username),
            new Claim(ClaimTypes.Role, ((Role)user.AppUserRoleId).ToString()),
        };

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            expires: DateTime.Now.AddHours(3),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public int? ValidateToken(string token)
    {
        if (token == null) 
            return null;

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);
        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var userId = int.Parse(jwtToken.Claims.First(x => x.Type == ClaimConstant.AppUserId).Value);

            // return user id from JWT token if validation successful
            return userId;
        }
        catch
        {
            // return null if validation fails
            return null;
        }
    }
}