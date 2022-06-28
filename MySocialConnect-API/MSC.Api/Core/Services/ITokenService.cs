using MSC.Api.Core.Entities;

namespace MSC.Api.Core.Services;

public interface ITokenService
{
    string CreateToken(AppUser user);
}