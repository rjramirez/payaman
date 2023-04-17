using Common.DataTransferObjects._Base;
using System.ComponentModel.DataAnnotations;

namespace Common.DataTransferObjects;

public class RegisterRequest : SaveDTOExtension
{
    [Required]
    public string Username { get; set; }
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }
}
