using DataAccess.Repositories.PayamanDB.Interfaces;
using DataAccess.UnitOfWork.Base;

namespace DataAccess.UnitOfWorks.PayamanDB
{
    public interface IPayamanDBUnitOfWork : IBaseUnitOfWork
    {
        public IErrorLogRepository ErrorLogRepository { get; }
        public IAuditTrailRepository AuditTrailRepository { get; }
    }
}
