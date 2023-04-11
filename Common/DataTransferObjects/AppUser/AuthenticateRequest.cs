using System.ComponentModel.DataAnnotations;

namespace Common.DataTransferObjects.AppUser
{
    public class AuthenticateRequest
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
