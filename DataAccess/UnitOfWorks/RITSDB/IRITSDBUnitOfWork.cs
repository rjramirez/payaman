using DataAccess.Repositories.RITSDB.Interfaces;
using DataAccess.UnitOfWork.Base;

namespace DataAccess.UnitOfWorks.RITSDB
{
    public interface IRITSDBUnitOfWork : IBaseUnitOfWork
    {
        public IErrorLogRepository ErrorLogRepository { get; }
        public IAuditTrailRepository AuditTrailRepository { get; }
    }
}
