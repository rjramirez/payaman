using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Common.DataTransferObjects;
using DataAccess.DbContexts.RITSDB.Models;
using DataAccess.DBContexts.RITSDB;
using DataAccess.DBContexts.RITSDB.Models;
using DataAccess.UnitOfWorks.RITSDB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace DataAccess.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;
    private readonly IRITSDBUnitOfWork _RITSDBUnitOfWork;
    private readonly RITSDBContext _context;
    private readonly IHttpContextAccessor _contextAccessor;

    public AuthenticationService(IHttpContextAccessor contextAccessor, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, IRITSDBUnitOfWork RITSDBUnitOfWork, RITSDBContext context)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
        _RITSDBUnitOfWork = RITSDBUnitOfWork;
        _context = context;
        _contextAccessor = contextAccessor;

    }

    public async Task<string> Register(RegisterRequest request)
    {
        //if (await _RITSDBUnitOfWork.AspNetUserRepository.IsExistAsync(r => r.UserName != "Admin"))
        //{
        //    AspNetRole aspRoleAdmin = new();
        //    aspRoleAdmin.Name = "Admin";
        //    aspRoleAdmin.NormalizedName = "Admin".ToUpper();
        //    aspRoleAdmin.ConcurrencyStamp = Guid.NewGuid().ToString();

        //    AspNetRole aspRoleCashier = new();
        //    aspRoleCashier.Name = "Cashier";
        //    aspRoleCashier.NormalizedName = "Cashier".ToUpper();
        //    aspRoleCashier.ConcurrencyStamp = Guid.NewGuid().ToString();

        //    await _context.AddAsync(aspRoleAdmin);
        //    await _context.AddAsync(aspRoleCashier);
        //}

        //var adminUser = new AspNetUser
        //{
        //    UserName = "adm_rjramirez",
        //    NormalizedUserName = "adm_rjramirez",
        //    Email = "",
        //    NormalizedEmail = "",
        //    EmailConfirmed = true,
        //    LockoutEnabled = false,
        //    SecurityStamp = Guid.NewGuid().ToString()
        //};


        ////Admin
        //if (!_context.AspNetUsers.Any(u => u.UserName == adminUser.UserName))
        //{
        //    var password = new PasswordHasher<AspNetUser>();
        //    var hashed = password.HashPassword(adminUser, "R1TST3st3r07!?");
        //    adminUser.PasswordHash = hashed;

        //    await _context.AddAsync(adminUser);

        //    //Add Role Admin
        //    var userAdmin = await _RITSDBUnitOfWork.AspNetUserRepository.SingleOrDefaultAsync(predicate: a => a.UserName == "adm_rjramirez");
        //    var aspNetRoleAdmin = await _RITSDBUnitOfWork.AspNetRoleRepository.SingleOrDefaultAsync(predicate: a => a.Name == "Admin");

        //    AspNetUserRole aspNetUserRoleAdmin = new();
        //    aspNetUserRoleAdmin.UserId = userAdmin.Id;
        //    aspNetUserRoleAdmin.RoleId = aspNetRoleAdmin.Id;

        //    await _RITSDBUnitOfWork.AspNetUserRoleRepository.AddAsync(aspNetUserRoleAdmin);
        //}

        //var cashierUser = new AspNetUser
        //{
        //    UserName = "cash_rjramirez",
        //    NormalizedUserName = "cash_rjramirez",
        //    Email = "",
        //    NormalizedEmail = "",
        //    EmailConfirmed = true,
        //    LockoutEnabled = false,
        //    SecurityStamp = Guid.NewGuid().ToString()
        //};


        ////Cashier
        //if (!_context.AspNetUsers.Any(u => u.UserName == adminUser.UserName))
        //{
        //    var password = new PasswordHasher<AspNetUser>();
        //    var hashed = password.HashPassword(adminUser, "R1TST3st3r07!?");
        //    adminUser.PasswordHash = hashed;

        //    await _context.AddAsync(adminUser);

        //    //Add Role Cashier
        //    var userCashier = await _RITSDBUnitOfWork.AspNetUserRepository.FindAsync(predicate: a => a.UserName == "adm_rjramirez");

        //}

        //await _context.SaveChangesAsync();

        
        var userByUsername = await _userManager.FindByNameAsync(request.Username);
        if (userByUsername is not null)
        {
            throw new ArgumentException($"User with username {request.Username} already exists.");
        }

        var passhash = _userManager.PasswordHasher;
        
        ApplicationUser user = new()
        {
            UserName = request.Username,
            SecurityStamp = Guid.NewGuid().ToString()
        };
        user.PasswordHash = passhash.HashPassword(user, request.Password);

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            throw new ArgumentException($"Unable to register user {request.Username} errors: {GetErrorsText(result.Errors)}");
        }

        return await Login(new LoginRequest { Username = "adm_rjramirez", Password = "R1TST3st3r07!?" });
    }

    public async Task<string> Login(LoginRequest request)
    {
        var user = await _userManager.FindByNameAsync(request.Username);

        if (user is null)
        {
            user = await _userManager.FindByEmailAsync(request.Username);
        }

        if (user is null || !await _userManager.CheckPasswordAsync(user, request.Password))
        {
            throw new ArgumentException($"Unable to authenticate user {request.Username}");
        }

        var authClaims = new List<Claim>
        {
            new(ClaimTypes.Name, user.UserName),
            new(ClaimTypes.Email, user.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var token = GetToken(authClaims);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private JwtSecurityToken GetToken(IEnumerable<Claim> authClaims)
    {
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

        var token = new JwtSecurityToken(
            issuer: _configuration["JWT:ValidIssuer"],
            audience: _configuration["JWT:ValidAudience"],
            expires: DateTime.Now.AddHours(3),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256));

        return token;
    }

    private string GetErrorsText(IEnumerable<IdentityError> errors)
    {
        return string.Join(", ", errors.Select(error => error.Description).ToArray());
    }
}

