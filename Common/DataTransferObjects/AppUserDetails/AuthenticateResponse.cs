using Common.Entities;

namespace Common.DataTransferObjects.AppUserDetails
{
    public class AuthenticateResponse
    {
        public int AppUserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public Role Role { get; set; }
        public string Token { get; set; }
    }
}

