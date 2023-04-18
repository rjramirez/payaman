using Common.DataTransferObjects.AuditTrail;
using DataAccess.DBContexts.RITSDB;
using DataAccess.Repositories.RITSDB;
using DataAccess.Repositories.RITSDB.Interfaces;
using DataAccess.Services.Interfaces;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace DataAccess.UnitOfWorks.RITSDB
{
    public sealed class RITSDBUnitOfWork : IRITSDBUnitOfWork
    {
        private readonly RITSDBContext _context;
        private readonly IDbContextChangeTrackingService _dbContextChangeTrackingService;
        public RITSDBUnitOfWork(RITSDBContext context, IDbContextChangeTrackingService dbContextChangeTrackingService)
        {
            _context = context;
            _dbContextChangeTrackingService = dbContextChangeTrackingService;
            ErrorLogRepository = new ErrorLogRepository(_context);
            AuditTrailRepository = new AuditTrailRepository(_context);
            ProductRepository = new ProductRepository(_context);
            OrderRepository = new OrderRepository(_context);
            StoreRepository = new StoreRepository(_context);
            AspNetUserRepository = new AspNetUserRepository(_context);
            AspNetUserRoleRepository = new AspNetUserRoleRepository(_context);
            AspNetRoleRepository = new AspNetRoleRepository(_context);
        }

        public IErrorLogRepository ErrorLogRepository { get; private set; }
        public IAuditTrailRepository AuditTrailRepository { get; private set; }
        public IProductRepository ProductRepository { get; private set; }
        public IOrderRepository OrderRepository { get; private set; }
        public IStoreRepository StoreRepository { get; private set; }
        public IAspNetUserRepository AspNetUserRepository { get; private set; }
        public IAspNetUserRoleRepository AspNetUserRoleRepository { get; private set; }
        public IAspNetRoleRepository AspNetRoleRepository { get; private set; }
        public void Dispose()
        {
            _context.Dispose();
        }

        public async Task<int> SaveChangesAsync(string transactionBy)
        {
            List<Tuple<ContextChangeTrackingDetail, EntityEntry>> contextChangeTrackingDetail = _dbContextChangeTrackingService.TrackRevisionDetails(_context);
            int result = await _context.SaveChangesAsync();

            if (contextChangeTrackingDetail.Any())
            {
                await _dbContextChangeTrackingService.SaveAuditTrail(transactionBy, contextChangeTrackingDetail);
            }

            return result;
        }
    }
}
