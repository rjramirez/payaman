using DataAccess.Repositories.RITSDB.Interfaces;
using DataAccess.UnitOfWork.Base;

namespace DataAccess.UnitOfWorks.RITSDB
{
    public interface IRITSDBUnitOfWork : IBaseUnitOfWork
    {
        public IAppUserRepository AppUserRepository { get; }
        public IAppUserRoleRepository AppUserRoleRepository { get; }
        public IErrorLogRepository ErrorLogRepository { get; }
        public IAuditTrailRepository AuditTrailRepository { get; }
        public IProductRepository ProductRepository { get; }
        public IStoreRepository StoreRepository { get; }
        public IOrderRepository OrderRepository { get; }
        public IOrderItemRepository OrderItemRepository { get; }
    }
}
