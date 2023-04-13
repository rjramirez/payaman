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
