using System;
using System.Security.Claims;
using MSC.Api.Core.Dto;

namespace MSC.Api.Core.Extensions;
/// <summary>
/// Moved the method GetLoggedInCalims from base controller here. Original method is commented there
/// https://www.jerriepelser.com/blog/useful-claimsprincipal-extension-methods/
/// </summary>
public static class ClaimsPrincipalExtensions
{
    public static string GetUserEmail(this ClaimsPrincipal principal)
    {
        if (principal == null) return string.Empty;
        //return principal.FindFirstValue(ClaimTypes.Email);
        var email = principal.FindFirst(ClaimTypes.Email)?.Value;
        return email;
    }

    public static string GetUserName(this ClaimsPrincipal principal)
    {
        if (principal == null) return string.Empty;
        //return principal.FindFirstValue(ClaimTypes.NameIdentifier);
        var userName = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return userName;
    }

    public static string GetDisplayName(this ClaimsPrincipal principal)
    {
        if (principal == null) return string.Empty;
        var displayName = principal.FindFirst("DisplayName")?.Value;
        return displayName;
    }

    public static Guid GetUserGuid(this ClaimsPrincipal principal)
    {
        var getGuid = Guid.Empty;
        if (principal == null) return getGuid;
        var guid = principal.FindFirst("Guid")?.Value;
        if (string.IsNullOrWhiteSpace(guid)) return getGuid;
        try
        {
            getGuid = new Guid(guid);
        }
        catch { }
        return getGuid;
    }

    public static UserClaimGetDto GetUserClaims(this ClaimsPrincipal principal)
    {
        if (principal == null) return null;
        var claimsDto = new UserClaimGetDto()
        {
            UserName = principal.GetUserName(),
            Guid = principal.GetUserGuid(),
            DisplayName = principal.GetDisplayName()
        };
        return claimsDto;
    }

}
