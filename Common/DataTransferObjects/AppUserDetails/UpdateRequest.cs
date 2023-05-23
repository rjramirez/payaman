namespace Common.DataTransferObjects.AppUserDetails
{
    public class UpdateRequest
    {
        public string AppUserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string AppUserRoleId { get; set; }
        public string Password { get; set; }
        public string TransactionBy { get; set; }
    }
}
