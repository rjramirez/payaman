using DataAccess.DBContexts.ProjectTemplateDB;
using DataAccess.DBContexts.ProjectTemplateDB.Models;
using DataAccess.Repositories.Base;
using DataAccess.Repositories.ProjectTemplateDB.Interfaces;

namespace DataAccess.Repositories.ProjectTemplateDB
{
    public class AuditTrailRepository : BaseRepository<AuditTrail>, IAuditTrailRepository
    {
        public AuditTrailRepository(ProjectTemplateDBContext context) : base(context)
        {

        }
    }
}
