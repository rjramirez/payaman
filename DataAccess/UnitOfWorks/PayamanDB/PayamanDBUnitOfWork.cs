using Common.DataTransferObjects.AuditTrail;
using DataAccess.DBContexts.PayamanDB;
using DataAccess.Repositories.PayamanDB;
using DataAccess.Repositories.PayamanDB.Interfaces;
using DataAccess.Services.Interfaces;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace DataAccess.UnitOfWorks.PayamanDB
{
    public sealed class PayamanDBUnitOfWork : IPayamanDBUnitOfWork
    {
        private readonly PayamanDBContext _context;
        private readonly IDbContextChangeTrackingService _dbContextChangeTrackingService;
        public PayamanDBUnitOfWork(PayamanDBContext context, IDbContextChangeTrackingService dbContextChangeTrackingService)
        {
            _context = context;
            _dbContextChangeTrackingService = dbContextChangeTrackingService;
            AppUserRepository = new AppUserRepository(_context);
            ErrorLogRepository = new ErrorLogRepository(_context);
            AuditTrailRepository = new AuditTrailRepository(_context);
        }

        public IAppUserRepository AppUserRepository { get; private set; }
        public IErrorLogRepository ErrorLogRepository { get; private set; }
        public IAuditTrailRepository AuditTrailRepository { get; private set; }

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
