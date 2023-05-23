using Common.DataTransferObjects._Base;
using Common.DataTransferObjects.ReferenceData;
using System.Text.Json.Serialization;

namespace Common.DataTransferObjects.AppUserDetails
{
    public class AppUserDetail : SaveDTOExtension
    {
        public int AppUserId { get; set; }
        public int AppUserRoleId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public ReferenceDataDetail AppUserRole { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string TransactionBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }

    }
}
