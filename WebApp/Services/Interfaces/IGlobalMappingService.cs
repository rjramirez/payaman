using Common.DataTransferObjects.Employee;
using Common.DataTransferObjects.ReferenceData;

namespace WebApp.Services.Interfaces
{
    public interface IGlobalMappingService
    {
        Task<IEnumerable<ReferenceDataDetail>> FilterReference(string endpoint, bool enableCache);
        Task<IEnumerable<EmployeeDetail>> FilterEmployee(string searchString, bool enableCache);
    }
}

