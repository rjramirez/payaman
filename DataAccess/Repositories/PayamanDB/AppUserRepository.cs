using DataAccess.DBContexts.PayamanDB;
using DataAccess.DBContexts.PayamanDB.Models;
using DataAccess.Repositories.Base;
using DataAccess.Repositories.PayamanDB.Interfaces;

namespace DataAccess.Repositories.PayamanDB
{
    public class AppUserRepository : BaseRepository<AppUser>, IAppUserRepository
    {
        public AppUserRepository(PayamanDBContext context) : base(context)
        {

        }
    }
}
