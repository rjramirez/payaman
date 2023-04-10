using Common.DataTransferObjects.AuditTrail;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace DataAccess.Services.Interfaces
{
    public interface IDbContextChangeTrackingService
    {
        Task SaveAuditTrail(string transactionBy, List<Tuple<ContextChangeTrackingDetail, EntityEntry>> contextChangeTrackingDetail);
        List<Tuple<ContextChangeTrackingDetail, EntityEntry>> TrackRevisionDetails(DbContext dbContext);
    }
}
