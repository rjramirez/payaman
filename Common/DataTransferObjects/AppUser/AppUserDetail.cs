namespace Common.DataTransferObjects.AppUser
{
    public class AppUserDetail
    {
        public int AppUserId { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
        public string ImageFileName { get; set; }
    }
}
