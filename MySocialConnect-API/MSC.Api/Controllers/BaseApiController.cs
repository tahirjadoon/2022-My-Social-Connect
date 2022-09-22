using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using MSC.Api.Core.Dto;

namespace MSC.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BaseApiController : ControllerBase
{
    
    /// <summary>
    /// gets the calims. Check TokenService for more details about the claims created
    /// NameId is the username claim
    /// Sid is the guid claim
    /// </summary>
    /// <returns></returns>
    public UserClaimGetDto GetLoggedInCalims(){
        var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var guid = User.FindFirst("Guid")?.Value;
        var displayName = User.FindFirst("DisplayName")?.Value;
        /*
        can loop through as well
        var claims = User.Claims;
        foreach(var c in claims){
            var x = c.Value;
            var y = c.Type;
            var z = "";
        }
        */
        var claimsDto = new UserClaimGetDto(){
            UserName = username,
            Guid = string.IsNullOrWhiteSpace(guid) ? Guid.Empty : new Guid(guid), 
            DisplayName = displayName
        };

        return claimsDto;
    }
}