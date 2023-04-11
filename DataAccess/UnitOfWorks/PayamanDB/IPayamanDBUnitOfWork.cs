using DataAccess.Repositories.PayamanDB.Interfaces;
using DataAccess.UnitOfWork.Base;

namespace DataAccess.UnitOfWorks.PayamanDB
{
    public interface IPayamanDBUnitOfWork : IBaseUnitOfWork
    {
        public IAppUserRepository AppUserRepository { get; }
        public IErrorLogRepository ErrorLogRepository { get; }
        public IAuditTrailRepository AuditTrailRepository { get; }
    }
}
