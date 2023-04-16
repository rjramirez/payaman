using DataAccess.DbContexts.RITSDB.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.DBContexts.RITSDB;

public class AppDBContext : IdentityDbContext<ApplicationUser>
{
    public AppDBContext()
    {
    }

    public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
    {
    }
}