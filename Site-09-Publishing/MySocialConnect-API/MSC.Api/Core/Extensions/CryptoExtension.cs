using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
using MSC.Api.Core.Dto;

namespace MSC.Api.Core.Extensions;
public static class CryptoExtension
{
    /// <summary>
    /// Compute HASh for the passed in value
    /// </summary>
    /// <param name="value"></param>
    /// <returns>HashKeyDto</returns>
    public static HashKeyDto ComputeHashHmacSha512(this string value)
    {
        if(string.IsNullOrWhiteSpace(value))
            return null;
        
        var valueBytes = Encoding.UTF8.GetBytes(value);
        using var hmac = new HMACSHA512();
        var hash = hmac.ComputeHash(valueBytes);
        var dto = new HashKeyDto { Salt = hmac.Key, Hash = hash  };
        return dto;
    } 

    /// <summary>
    /// Compute HASh for the passed in value using the passed in saltKey
    /// </summary>
    /// <param name="value"></param>
    /// <param name="saltKey"></param>
    /// <returns>HashKeyDto</returns>
    public static HashKeyDto ComputeHashHmacSha512(this string value, byte[] saltKey)
    {
        if(string.IsNullOrWhiteSpace(value))
            throw new ValidationException("Item1 is missing");
        if(saltKey == null)
            throw new ValidationException("Item2 is missing");
        
        var valueBytes = Encoding.UTF8.GetBytes(value);
        using var hmac = new HMACSHA512(saltKey);
        var hash = hmac.ComputeHash(valueBytes);
        var dto = new HashKeyDto { Salt = hmac.Key, Hash = hash  };
        return dto;
    }



}