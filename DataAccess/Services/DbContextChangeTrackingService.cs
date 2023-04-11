using Common.Constants;
using Common.DataTransferObjects.AuditTrail;
using DataAccess.DBContexts.PayamanDB;
using DataAccess.DBContexts.PayamanDB.Models;
using DataAccess.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace DataAccess.Services
{
    public class DbContextChangeTrackingService : IDbContextChangeTrackingService
    {
        private readonly PayamanDBContext _context;
        public DbContextChangeTrackingService(PayamanDBContext context)
        {
            _context = context;
        }

        public List<Tuple<ContextChangeTrackingDetail, EntityEntry>> TrackRevisionDetails(DbContext dbContext)
        {
            //TODO: Exclude your table from audit logs
            IEnumerable<EntityEntry> entityEntries = dbContext.ChangeTracker.Entries()
                .Where(e => e.Entity.GetType() != typeof(AuditTrail)
                && e.Entity.GetType() != typeof(AuditTrailDetail)
                && e.Entity.GetType() != typeof(ErrorLog));

            List<string> excludedFields = DbContextConstant.NoAuditColumns.Split(',').ToList();

            List<Tuple<ContextChangeTrackingDetail, EntityEntry>> contextChangeTrackingDetail = new();

            foreach (EntityEntry entityEntry in entityEntries)
            {
                object[] primaryKeys = entityEntry.Metadata.FindPrimaryKey()
                                    .Properties
                                    .Select(p => entityEntry.Property(p.Name).CurrentValue)
                                    .ToArray();
                foreach (PropertyEntry propertyEntry in entityEntry.Properties
                    .Where(p => !excludedFields.Contains(p.Metadata.Name)))
                {
                    if (propertyEntry.IsModified || entityEntry.State == EntityState.Added || entityEntry.State == EntityState.Deleted)
                    {
                        ContextChangeTrackingDetail changeTrackingDetail = new()
                        {
                            EntityId = string.Join(',', primaryKeys),
                            EntityField = propertyEntry.Metadata.Name,
                            OldValue = propertyEntry.OriginalValue?.ToString(),
                            NewValue = propertyEntry.CurrentValue?.ToString(),
                            TableName = entityEntry.Entity.GetType().Name,
                            Action = entityEntry.State.ToString()
                        };

                        Tuple<ContextChangeTrackingDetail, EntityEntry> changeTrackingTuple = new(changeTrackingDetail, entityEntry);
                        contextChangeTrackingDetail.Add(changeTrackingTuple);
                    }
                }
            }

            return contextChangeTrackingDetail;
        }

        public async Task SaveAuditTrail(
            string transactionBy,
            List<Tuple<ContextChangeTrackingDetail, EntityEntry>> contextChangeTrackingDetail)
        {
            AuditTrail auditTrail = new()
            {
                TransactionBy = transactionBy,
                TransactionDate = DateTime.Now
            };

            foreach (var changeTrackingTuple in contextChangeTrackingDetail)
            {
                ContextChangeTrackingDetail changeTrackingDetail = changeTrackingTuple.Item1;
                EntityEntry entityEntry = changeTrackingTuple.Item2;

                object[] primaryKeys = entityEntry.Metadata.FindPrimaryKey()
                                .Properties
                                .Select(p => entityEntry.Property(p.Name).CurrentValue)
                                .ToArray();

                string concatenatedPrimaryKeys = string.Join(',', primaryKeys);

                AuditTrailDetail auditTrailDetail = new()
                {
                    EntityField = changeTrackingDetail.EntityField,
                    EntityId = changeTrackingDetail.EntityId != concatenatedPrimaryKeys ? concatenatedPrimaryKeys : changeTrackingDetail.EntityId,
                    TableName = changeTrackingDetail.TableName,
                    OldValue = changeTrackingDetail.OldValue,
                    NewValue = changeTrackingDetail.NewValue,
                    Action = changeTrackingDetail.Action
                };
                auditTrail.AuditTrailDetails.Add(auditTrailDetail);
            }

            await _context.AuditTrails.AddAsync(auditTrail);
            await _context.SaveChangesAsync();
        }
    }
}
