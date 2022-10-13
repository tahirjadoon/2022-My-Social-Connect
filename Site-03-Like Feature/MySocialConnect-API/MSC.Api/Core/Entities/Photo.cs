using System.ComponentModel.DataAnnotations.Schema;

namespace MSC.Api.Core.Entities;

//Database table will be called Photos
[Table("Photos")]
public class Photo
{
    public int Id { get; set; }
    public string Url { get; set; }
    public bool IsMain { get; set; }
    public string PublicId { get; set; }

    //fully defining the relationship between AppUser and Photos
    public AppUser AppUser { get; set; }
    public int AppUserId { get; set; }
}
