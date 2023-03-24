using System.ComponentModel.DataAnnotations;

namespace OAuthExample.Models.Requests;

public class RegisterUserRequest
{
    [Required]
    public string UserName { get; set; }

    [Required]
    public string Password { get; set; }
}
