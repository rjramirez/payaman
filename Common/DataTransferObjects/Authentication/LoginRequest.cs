using Common.DataTransferObjects._Base;
using System.ComponentModel.DataAnnotations;

namespace Common.DataTransferObjects;

public class LoginRequest : SaveDTOExtension
{
    [Required]
    public string Username { get; set; }
    [Required]
    public string Password { get; set; }
}
