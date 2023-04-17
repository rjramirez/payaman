//using Common.DataTransferObjects.Employee.Interfaces;

namespace Common.Services.Interfaces
{
    public interface IDtoValidationService
    {
        //void ValidateEmployeeFullDetail(IEmployeeFullDetail employeeFullDetail);
        void ValidateObject<TSource>(TSource objectValue);
        void ValidateRequiredProperty(string propertyValue, string propertyName);

        //void ValidateSicklineTrackerRequiredProperty(ISicklineTracker sicklineTracker);

        //void ValidateSicklineTrackerRequiredProperty(ILeaveEmployee leaveEmployee);
        //void RemoveExtraWhiteSpaces(ILeaveEmployee leaveEmployee);
        //void RemoveExtraWhiteSpaces(ISicklineTracker sicklineTracker);
    }
}
