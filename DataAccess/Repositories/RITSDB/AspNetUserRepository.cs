using DataAccess.DBContexts.RITSDB;
using DataAccess.DBContexts.RITSDB.Models;
using DataAccess.Repositories.Base;
using DataAccess.Repositories.RITSDB.Interfaces;

namespace DataAccess.Repositories.RITSDB
{
    public class AspNetUserRepository : BaseRepository<AspNetUser>, IAspNetUserRepository
    {
        public AspNetUserRepository(RITSDBContext context) : base(context)
        {

        }
    }
}
