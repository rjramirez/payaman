using Common.DataTransferObjects.ReferenceData;

namespace WebApp.Models.AppUser
{
    public class AppUserVM
    {
        public int AppUserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public ReferenceDataDetail Role { get; set; }
        public string Password { get; set; }
    }
}
