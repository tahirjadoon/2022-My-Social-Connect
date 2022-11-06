using System.Collections.Generic;
using MSC.Api.Core.Entities;

namespace MSC.Api.Core.Constants;
public class SiteIdentityConstants
{
    public const string Role_Member = "Member";
    public const string Role_Admin = "Admin";
    public const string Role_Moderator = "Moderator";

    public static List<AppRole> SiteRoles = new List<AppRole>
    {
        new AppRole{ Name = SiteIdentityConstants.Role_Member },
        new AppRole { Name = SiteIdentityConstants.Role_Admin },
        new AppRole { Name = SiteIdentityConstants.Role_Moderator }
    };

    public const string AuthPolicy_RequireAdminRole = $"RequireAdminRole";
    public const string AuthPolicy_ModeratePhotoRole = $"RequireModerateRole";
}
