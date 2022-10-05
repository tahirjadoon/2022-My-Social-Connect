using System.ComponentModel.DataAnnotations;

namespace MSC.Api.Core.Dto;

public class LoginDto
{
    [Required(ErrorMessage = "UserName is empty")]
    public string UserName { get; set; }

    [Required(ErrorMessage = "Password is empty")]
    public string Password { get; set; }   
}