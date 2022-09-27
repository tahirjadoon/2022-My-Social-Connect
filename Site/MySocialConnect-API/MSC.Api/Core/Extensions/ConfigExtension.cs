using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using MSC.Api.Core.Constants;
using MSC.Api.Core.Dto.Helpers;

namespace MSC.Api.Core.Extensions;

public static class ConfigExtension
{
    public static string GetDefaultConnectionString(this ConfigurationManager config)
    {
        var connectionString = config.GetConnectionString(ConfigKeyConstants.DefaultConnection);
        return connectionString;
    }

    public static string GetDefaultConnectionString(this IConfiguration config)
    {
        var connectionString = config.GetConnectionString(ConfigKeyConstants.DefaultConnection);
        return connectionString;
    }

    public static List<string> GetAllowSpecificOrigins(this ConfigurationManager config)
    {
        var allowSpecificOrigins = config.GetSectionValue<List<string>>(ConfigKeyConstants.AllowSpecificOrigins, null);
        return allowSpecificOrigins;
    }

    public static List<string> GetAllowSpecificOrigins(this IConfiguration config)
    {
        var allowSpecificOrigins = config.GetSectionValue<List<string>>(ConfigKeyConstants.AllowSpecificOrigins, null);
        return allowSpecificOrigins;
    }

    public static string GetTokenKey(this ConfigurationManager config)
    {
        var tokenKey = config.GetSectionValue<string>(ConfigKeyConstants.TokenKey, string.Empty);
        return tokenKey;
    }

    public static string GetTokenKey(this IConfiguration config)
    {
        var tokenKey = config.GetSectionValue<string>(ConfigKeyConstants.TokenKey, string.Empty);
        return tokenKey;
    }

    public static string GetLoggingLevelDefault(this ConfigurationManager config)
    {
        var loggingLevelDefault = config.GetSectionValue<string>(ConfigKeyConstants.LoggingLevelDefault, string.Empty);
        return loggingLevelDefault;
    }

    public static string GetLoggingLevelDefault(this IConfiguration config)
    {
        var loggingLevelDefault = config.GetSectionValue<string>(ConfigKeyConstants.LoggingLevelDefault, string.Empty);
        return loggingLevelDefault;
    }

    public static string GetLoggingLevelMsApnetCore(this ConfigurationManager config)
    {
        var loggingLevelDefault = config.GetSectionValue<string>(ConfigKeyConstants.LoggingLevelMsAspNetCore, string.Empty);
        return loggingLevelDefault;
    }

    public static string GetLoggingLevelMsApnetCore(this IConfiguration config)
    {
        var loggingLevelDefault = config.GetSectionValue<string>(ConfigKeyConstants.LoggingLevelMsAspNetCore, string.Empty);
        return loggingLevelDefault;
    }

    public static CloudinaryConfig GetCloudinaryConfig(this IConfiguration config)
    {
        var cloudinary = config.GetSectionValue<CloudinaryConfig>(ConfigKeyConstants.CloudinarySettingsKey, null);
        return cloudinary;
    }

    public static T GetSectionValue<T>(this ConfigurationManager config, string sectionName)
    {
        if (!config.GetSection(sectionName).Exists())
        {
            return default(T);
        }
        var sValue = config.GetSection(sectionName).Get<T>();
        return sValue;
    }

    public static T GetSectionValue<T>(this ConfigurationManager config, string sectionName, T defaultValue)
    {
        if (!config.GetSection(sectionName).Exists())
        {
            return defaultValue;
        }

        var sValue = config.GetSection(sectionName).Get<T>();
        return sValue;
    }

    public static T GetSectionValue<T>(this IConfiguration config, string sectionName)
    {
        if (!config.GetSection(sectionName).Exists())
        {
            return default(T);
        }
        var sValue = config.GetSection(sectionName).Get<T>();
        return sValue;
    }

    public static T GetSectionValue<T>(this IConfiguration config, string sectionName, T defaultValue)
    {
        if (!config.GetSection(sectionName).Exists())
        {
            return defaultValue;
        }

        var sValue = config.GetSection(sectionName).Get<T>();
        return sValue;
    }
}