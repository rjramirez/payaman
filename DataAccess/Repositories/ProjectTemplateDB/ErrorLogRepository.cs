using DataAccess.DBContexts.ProjectTemplateDB;
using DataAccess.DBContexts.ProjectTemplateDB.Models;
using DataAccess.Repositories.Base;
using DataAccess.Repositories.ProjectTemplateDB.Interfaces;

namespace DataAccess.Repositories.ProjectTemplateDB
{
    public class ErrorLogRepository : BaseRepository<ErrorLog>, IErrorLogRepository
    {
        public ErrorLogRepository(ProjectTemplateDBContext context) : base(context)
        {

        }
    }
}