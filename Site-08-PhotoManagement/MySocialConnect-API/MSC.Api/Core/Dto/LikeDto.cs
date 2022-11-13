using System;

namespace MSC.Api.Core.Dto;
public class LikeDto
{
    public int Id { get; set; }
    public Guid GuId { get; set; }
    public string UserName { get; set; }
    public string PhotoUrl { get; set; } //custom where Photo isMain
    public int Age { get; set; }
    public string DisplayName { get; set; }
    public string City { get; set; }
}
