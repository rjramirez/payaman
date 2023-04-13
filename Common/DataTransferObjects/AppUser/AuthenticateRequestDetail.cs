using System.ComponentModel.DataAnnotations;

namespace Common.DataTransferObjects.AppUser
{
    public class AuthenticateRequestDetail
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
