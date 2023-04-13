using Common.DataTransferObjects._Base;
using System.ComponentModel.DataAnnotations;

namespace Common.DataTransferObjects.AppUserDetails
{
    public class AuthenticateRequest : SaveDTOExtension
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
