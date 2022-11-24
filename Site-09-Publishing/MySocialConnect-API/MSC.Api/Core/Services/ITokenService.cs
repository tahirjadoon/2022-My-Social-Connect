using System.Threading.Tasks;
using MSC.Api.Core.Entities;

namespace MSC.Api.Core.Services;

public interface ITokenService
{
    Task<string> CreateToken(AppUser user);
}