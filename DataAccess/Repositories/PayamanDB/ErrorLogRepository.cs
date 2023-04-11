using DataAccess.DBContexts.PayamanDB;
using DataAccess.DBContexts.PayamanDB.Models;
using DataAccess.Repositories.Base;
using DataAccess.Repositories.PayamanDB.Interfaces;

namespace DataAccess.Repositories.PayamanDB
{
    public class ErrorLogRepository : BaseRepository<ErrorLog>, IErrorLogRepository
    {
        public ErrorLogRepository(PayamanDBContext context) : base(context)
        {

        }
    }
}