using Common.DataTransferObjects.AuditTrail;
using DataAccess.DBContexts.ProjectTemplateDB;
using DataAccess.Repositories.ProjectTemplateDB;
using DataAccess.Repositories.ProjectTemplateDB.Interfaces;
using DataAccess.Services.Interfaces;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace DataAccess.UnitOfWorks.ProjectTemplateDB
{
    public sealed class ProjectTemplateDBUnitOfWork : IProjectTemplateDBUnitOfWork
    {
        private readonly ProjectTemplateDBContext _context;
        private readonly IDbContextChangeTrackingService _dbContextChangeTrackingService;
        public ProjectTemplateDBUnitOfWork(ProjectTemplateDBContext context, IDbContextChangeTrackingService dbContextChangeTrackingService)
        {
            _context = context;
            _dbContextChangeTrackingService = dbContextChangeTrackingService;
            ErrorLogRepository = new ErrorLogRepository(_context);
            AuditTrailRepository = new AuditTrailRepository(_context);
        }

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
