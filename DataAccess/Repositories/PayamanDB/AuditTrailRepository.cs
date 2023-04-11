using DataAccess.DBContexts.PayamanDB;
using DataAccess.DBContexts.PayamanDB.Models;
using DataAccess.Repositories.Base;
using DataAccess.Repositories.PayamanDB.Interfaces;

namespace DataAccess.Repositories.PayamanDB
{
    public class AuditTrailRepository : BaseRepository<AuditTrail>, IAuditTrailRepository
    {
        public AuditTrailRepository(PayamanDBContext context) : base(context)
        {

        }
    }
}
