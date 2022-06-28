using System.ComponentModel.DataAnnotations;

namespace MSC.Api.Core.Dto;

public class UserRegisterDto
{
    [Required(ErrorMessage = "UserName is empty")]
    [MinLength(5, ErrorMessage = "Length must be atleast 5 chars")]
    public string UserName { get; set; }

    [Required(ErrorMessage = "Password is empty")]
    public string Password { get; set; }
}