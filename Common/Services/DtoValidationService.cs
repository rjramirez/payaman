//using Common.DataTransferObjects.Employee;
//using Common.DataTransferObjects.Employee.Interfaces;
//using Common.DataTransferObjects.EmployeeContact;
//using Common.DataTransferObjects.EmployeeJobDetail;
//using Common.DataTransferObjects.EmployeeLoginDetail;
//using Common.DataTransferObjects.EmployeeSupervisor;
using Common.Services.Interfaces;
using System.Reflection;

namespace Common.Services
{
    public class DtoValidationService : IDtoValidationService
    {
        public DtoValidationService(List<string> validationMessageList = null)
        {
            this.validationMessageList = validationMessageList;
        }

        private List<string> validationMessageList { get; set; }
        public void ValidateObject<TSource>(TSource objectValue)
        {

            Type objectType = objectValue.GetType();
            PropertyInfo[] propValues = objectType.GetProperties();

            foreach (PropertyInfo propValue in propValues)
            {

                if (propValue.PropertyType == typeof(string))
                {
                    string value = objectType.GetProperty(propValue.Name).GetValue(objectValue, null)?.ToString();

                    if (string.IsNullOrEmpty(value))
                    {
                        propValue.SetValue(objectValue, "-----", null);
                    }
                    else if (value.ToLower() == "n/a" || value.ToLower() == "null" || value.ToLower() == "unknown")
                    {
                        propValue.SetValue(objectValue, "-----", null);
                    }
                }

            }
        }

        public void ValidateRequiredProperty(string propertyValue, string propertyName)
        {

            if (string.IsNullOrEmpty(propertyValue) || propertyValue.ToString() == "0")
            {
                validationMessageList.Add(propertyName);
            }
        } 
        
        //public void ValidateSicklineTrackerStringLength(string propertyValue, string propertyName)
        //{
            

        //    if (!string.IsNullOrEmpty(propertyValue))
        //    {
        //        int propertyStringLength = PropertyStringLengthAttribute(propertyName);

        //        if (propertyValue.Length > propertyStringLength)
        //        {
        //            string displayName = PropertyDisplayNameAttribute(propertyName);
        //            validationMessageList.Add($"{displayName} max length is {propertyStringLength}");
        //        }
        //    }
            
        //}

        //public void ValidateEmployeeFullDetail(IEmployeeFullDetail employeeFullDetail)
        //{
        //    ValidateObject(employeeFullDetail.JobDetail != null ?
        //        employeeFullDetail.JobDetail :
        //        employeeFullDetail.JobDetail = new JobDetail());

        //    ValidateObject(employeeFullDetail.JobDetail.EmploymentStatusDetail != null ?
        //        employeeFullDetail.JobDetail.EmploymentStatusDetail :
        //        employeeFullDetail.JobDetail.EmploymentStatusDetail = new EmploymentStatusDetail());

        //    ValidateObject(employeeFullDetail.JobDetail.AssignmentDetail != null ? 
        //        employeeFullDetail.JobDetail.AssignmentDetail : 
        //        employeeFullDetail.JobDetail.AssignmentDetail = new AssignmentDetail());
           
        //    ValidateObject(employeeFullDetail.JobDetail.JobTitle != null ?
        //        employeeFullDetail.JobDetail.JobTitle :
        //        employeeFullDetail.JobDetail.JobTitle = new JobTitle());

        //    ValidateObject(employeeFullDetail.JobDetail.EmploymentRank != null ? 
        //        employeeFullDetail.JobDetail.EmploymentRank : 
        //        employeeFullDetail.JobDetail.EmploymentRank = new EmploymentRank());

        //    ValidateObject(employeeFullDetail.JobDetail.WorkSetup != null ? 
        //        employeeFullDetail.JobDetail.WorkSetup : 
        //        employeeFullDetail.JobDetail.WorkSetup = new WorkSetup());

        //    ValidateObject(employeeFullDetail.JobDetail.AssignmentDetail.ProjectDetail != null ? 
        //        employeeFullDetail.JobDetail.AssignmentDetail.ProjectDetail : 
        //        employeeFullDetail.JobDetail.AssignmentDetail.ProjectDetail = new ProjectDetail());

        //    ValidateObject(employeeFullDetail.JobDetail.AssignmentDetail.FacilityDetail != null ?
        //       employeeFullDetail.JobDetail.AssignmentDetail.FacilityDetail :
        //       employeeFullDetail.JobDetail.AssignmentDetail.FacilityDetail = new FacilityDetail());

        //    ValidateObject(employeeFullDetail.JobDetail.AssignmentDetail.DepartmentDetail != null ?
        //       employeeFullDetail.JobDetail.AssignmentDetail.DepartmentDetail :
        //       employeeFullDetail.JobDetail.AssignmentDetail.DepartmentDetail = new DepartmentDetail());

        //    ValidateObject(employeeFullDetail.SupervisorDetail != null ?
        //        employeeFullDetail.SupervisorDetail :
        //        employeeFullDetail.SupervisorDetail = new SupervisorDetail());

        //    ValidateObject(employeeFullDetail.SecondLevelSupervisorDetail != null ?
        //        employeeFullDetail.SecondLevelSupervisorDetail :
        //        employeeFullDetail.SecondLevelSupervisorDetail = new SupervisorDetail());

        //    ValidateObject(employeeFullDetail.LoginDetail != null ?
        //        employeeFullDetail.LoginDetail :
        //        employeeFullDetail.LoginDetail = new List<LoginDetail>());

        //    ValidateObject(employeeFullDetail.ContactDetail != null ? 
        //        employeeFullDetail.ContactDetail : 
        //        employeeFullDetail.ContactDetail = new ContactDetail());

        //    ValidateObject(employeeFullDetail.ContactDetail.CityDetail != null ?
        //        employeeFullDetail.ContactDetail.CityDetail :
        //        employeeFullDetail.ContactDetail.CityDetail = new CityDetail());

        //}

        //private string PropertyDisplayNameAttribute(string propertyName)
        //{
        //    MemberInfo property = typeof(ISicklineTracker).GetProperty(propertyName);

        //    var attribute = property.GetCustomAttributes(typeof(DisplayNameAttribute), true)
        //   .Cast<DisplayNameAttribute>().Single();
        //    string displayName = attribute.DisplayName;

        //    return displayName;
        //}

        //private int PropertyStringLengthAttribute(string propertyName)
        //{
        //    MemberInfo property = typeof(ISicklineTracker).GetProperty(propertyName);

        //    var attribute = property.GetCustomAttributes(typeof(StringLengthAttribute), true)
        //   .Cast<StringLengthAttribute>().Single();
        //    int stringLength = attribute.MaximumLength;

        //    return stringLength;
        //}

        //public void ValidateSicklineTrackerRequiredProperty(ISicklineTracker sicklineTracker) 
        //{
        //    ValidateRequiredProperty(sicklineTracker.EmployeeId.ToString(),
        //        PropertyDisplayNameAttribute(nameof(sicklineTracker.EmployeeId)));

        //    ValidateRequiredProperty(sicklineTracker.TagCode,
        //        PropertyDisplayNameAttribute(nameof(sicklineTracker.TagCode)));

        //    ValidateRequiredProperty(sicklineTracker.ReasonId.ToString(), 
        //        PropertyDisplayNameAttribute(nameof(sicklineTracker.ReasonId)));

        //    ValidateRequiredProperty(sicklineTracker.ReasonNotes, 
        //        PropertyDisplayNameAttribute(nameof(sicklineTracker.ReasonNotes)));

        //    ValidateSicklineTrackerStringLength(sicklineTracker.ReasonNotes,
        //        nameof(sicklineTracker.ReasonNotes));
            
        //    ValidateSicklineTrackerStringLength(sicklineTracker.OMDNotes,
        //        nameof(sicklineTracker.OMDNotes));
            
        //    ValidateSicklineTrackerStringLength(sicklineTracker.CallbackNo,
        //        nameof(sicklineTracker.CallbackNo));
        //}

        //public void RemoveExtraWhiteSpaces(ISicklineTracker sicklineTracker)
        //{
        //    if (!string.IsNullOrEmpty(sicklineTracker.ReferenceNo))
        //        sicklineTracker.ReferenceNo = Regex.Replace(sicklineTracker.ReferenceNo.Trim(), @"\s+", " ");

        //    if (!string.IsNullOrEmpty(sicklineTracker.CallbackNo))
        //        sicklineTracker.CallbackNo = Regex.Replace(sicklineTracker.CallbackNo.Trim(), @"\s+", " "); 
            
        //    if (!string.IsNullOrEmpty(sicklineTracker.TagCode))
        //        sicklineTracker.TagCode = Regex.Replace(sicklineTracker.TagCode.Trim(), @"\s+", " ");

        //    if (!string.IsNullOrEmpty(sicklineTracker.ReasonNotes))
        //        sicklineTracker.ReasonNotes = Regex.Replace(sicklineTracker.ReasonNotes.Trim(), @"\s+", " ");

        //    if (!string.IsNullOrEmpty(sicklineTracker.OMDNotes))
        //        sicklineTracker.OMDNotes = Regex.Replace(sicklineTracker.OMDNotes.Trim(), @"\s+", " ");
        //}


        //public void ValidateSicklineTrackerRequiredProperty(ILeaveEmployee leaveEmployee)
        //{
        //    ValidateRequiredProperty(leaveEmployee.EmployeeId.ToString(),
        //        PropertyDisplayNameAttribute(nameof(leaveEmployee.EmployeeId)));

        //    ValidateRequiredProperty(leaveEmployee.TagCode,
        //        PropertyDisplayNameAttribute(nameof(leaveEmployee.TagCode)));

        //    ValidateRequiredProperty(leaveEmployee.ReasonId.ToString(),
        //        PropertyDisplayNameAttribute(nameof(leaveEmployee.ReasonId)));

        //    ValidateRequiredProperty(leaveEmployee.ReasonNotes,
        //        PropertyDisplayNameAttribute(nameof(leaveEmployee.ReasonNotes)));

        //    ValidateSicklineTrackerStringLength(leaveEmployee.ReasonNotes,
        //        nameof(leaveEmployee.ReasonNotes));

        //}

        //public void RemoveExtraWhiteSpaces(ILeaveEmployee leaveEmployee)
        //{
        //    if (!string.IsNullOrEmpty(leaveEmployee.ReasonNotes))
        //        leaveEmployee.ReasonNotes = Regex.Replace(leaveEmployee.ReasonNotes.Trim(), @"\s+", " ");

        //}
    }
}
