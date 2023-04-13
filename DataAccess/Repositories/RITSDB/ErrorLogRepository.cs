using DataAccess.DBContexts.RITSDB;
using DataAccess.DBContexts.RITSDB.Models;
using DataAccess.Repositories.Base;
using DataAccess.Repositories.RITSDB.Interfaces;

namespace DataAccess.Repositories.RITSDB
{
    public class ErrorLogRepository : BaseRepository<ErrorLog>, IErrorLogRepository
    {
        public ErrorLogRepository(RITSDBContext context) : base(context)
        {

        }
    }
}