using System;
using System.ComponentModel.DataAnnotations;

namespace MSC.Api.Core.Dto;

public class UserRegisterDto
{
    [Required(ErrorMessage = "Gender is empty")]
    public string Gender { get; set; }

    [Required(ErrorMessage = "DisplayName is empty")]
    [MinLength(5, ErrorMessage = "DisplayName length must be atleast 5 chars")]
    public string DisplayName { get; set; }

    [Required(ErrorMessage = "DateOfBirth is empty")]
    public DateTime DateOfBirth { get; set; }

    [Required(ErrorMessage = "City is empty")]
    [RegularExpression("^[a-zA-z]*$", ErrorMessage = "Only characters are allowed for the city")]
    public string City { get; set; }

    [Required(ErrorMessage = "Country is empty")]
    [RegularExpression("^[a-zA-z]*$", ErrorMessage = "Only characters are allowed for the country")]
    public string Country { get; set; }

    [Required(ErrorMessage = "UserName is empty")]
    [MinLength(5, ErrorMessage = "UserName length must be atleast 5 chars")]
    public string UserName { get; set; }

    [Required(ErrorMessage = "Password is empty")]
    [StringLength(8, MinimumLength = 4)]
    [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d).+$", ErrorMessage = "Password must have an upper case, a lower case and a number")]
    public string Password { get; set; }
}