using DataAccess.Repositories.RITSDB.Interfaces;
using DataAccess.UnitOfWork.Base;

namespace DataAccess.UnitOfWorks.RITSDB
{
    public interface IRITSDBUnitOfWork : IBaseUnitOfWork
    {
        public IAppUserRepository AppUserRepository { get; }
        public IRoleRepository RoleRepository { get; }
        public IErrorLogRepository ErrorLogRepository { get; }
        public IAuditTrailRepository AuditTrailRepository { get; }
    }
}
