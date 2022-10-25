using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MSC.Api.Core.Entities;
using MSC.Api.Core.Extensions;

namespace MSC.Api.Core.Services;

public class TokenService : ITokenService
{
    //one key is used to encrypt and decrypt electronic information
    //JWT uses SymmetricSecurityKey, it never leaves the server
    private readonly SymmetricSecurityKey _key;
    private readonly UserManager<AppUser> _usermanager;

    public TokenService(IConfiguration config, UserManager<AppUser> usermanager)
    {
        _usermanager = usermanager;

        //get the tokenKey from config
        var tokenKey = config.GetTokenKey();
        if (string.IsNullOrWhiteSpace(tokenKey))
            throw new Exception("TokenKey missing");

        //convert to bytes array
        var tokenKeyBytes = Encoding.UTF8.GetBytes(tokenKey);

        //create key
        _key = new SymmetricSecurityKey(tokenKeyBytes);
    }

    public async Task<string> CreateToken(AppUser user)
    {
        if (user == null)
            throw new Exception("User info missing");

        //claims
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
            new Claim("Guid", user.GuId.ToString()),
            new Claim("DisplayName", user.DisplayName),
        };

        //get roles and add to the claims above with a custom name 
        var roles = await _usermanager.GetRolesAsync(user);
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        //credentials with key
        var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

        //describe token
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddDays(7),
            SigningCredentials = creds
        };

        //token handler
        var tokenHandler = new JwtSecurityTokenHandler();

        //token
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var writeToken = tokenHandler.WriteToken(token);
        return writeToken;
    }
}