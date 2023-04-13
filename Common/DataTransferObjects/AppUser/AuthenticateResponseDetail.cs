namespace Common.DataTransferObjects.AppUser 
{
    public class AuthenticateResponseDetail
    {
        public int AppUserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Token { get; set; }
    }
}

