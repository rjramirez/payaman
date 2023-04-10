using DataAccess.DBContexts.ProjectTemplateDB.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.DBContexts.ProjectTemplateDB
{
    public partial class ProjectTemplateDBContext : DbContext
    {
        public ProjectTemplateDBContext(DbContextOptions<ProjectTemplateDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AuditTrail> AuditTrails { get; set; }
        public virtual DbSet<AuditTrailDetail> AuditTrailDetails { get; set; }
        public virtual DbSet<ErrorLog> ErrorLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AuditTrailDetail>(entity =>
            {
                entity.HasOne(d => d.AuditTrail)
                    .WithMany(p => p.AuditTrailDetails)
                    .HasForeignKey(d => d.AuditTrailId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AuditTrailDetail_AuditTrail");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
