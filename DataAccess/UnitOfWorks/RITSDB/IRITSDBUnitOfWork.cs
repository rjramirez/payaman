using DataAccess.Repositories.RITSDB.Interfaces;
using DataAccess.UnitOfWork.Base;

namespace DataAccess.UnitOfWorks.RITSDB
{
    public interface IRITSDBUnitOfWork : IBaseUnitOfWork
    {
<<<<<<< HEAD
=======
        public IAppUserRepository AppUserRepository { get; }
        public IAppUserRoleRepository AppUserRoleRepository { get; }
>>>>>>> dev
        public IErrorLogRepository ErrorLogRepository { get; }
        public IAuditTrailRepository AuditTrailRepository { get; }
        public IAspNetUserRepository AspNetUserRepository { get; }
        public IAspNetUserRoleRepository AspNetUserRoleRepository { get; }
        public IAspNetRoleRepository AspNetRoleRepository { get; }
    }
}
