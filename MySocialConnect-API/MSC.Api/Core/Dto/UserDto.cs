using System;
using System.Collections.Generic;

namespace MSC.Api.Core.Dto;

public class UserDto
{
    public int Id { get; set; }
    public string UserName { get; set; }
    public string PhotoUrl { get; set; } //custom where Photo isMain
    public int Age { get; set; }
    public string DisplayName { get; set; }
    public string Gender { get; set; }
    public string Introduction { get; set; }
    public string LookingFor { get; set; }
    public string Interests { get; set; }
    public string City { get; set; }
    public string Country { get; set; }
    public ICollection<PhotoDto> Photos { get; set; }
    public DateTime LastActive { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime UpdatedOn { get; set; }
}