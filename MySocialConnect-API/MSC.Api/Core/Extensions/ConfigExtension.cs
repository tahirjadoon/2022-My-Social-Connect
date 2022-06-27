using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using MSC.Api.Core.Constants;

namespace MSC.Api.Core.Extensions;

public static class ConfigExtension
{
    public static string GetDefaultConnectionString(this ConfigurationManager config)
    {
        var connectionString = config.GetConnectionString(ConfigKeyConstants.DefaultConnection);
        return connectionString;
    }

    public static List<string> GetAllowSpecificOrigins(this ConfigurationManager config)
    {
        var allowSpecificOrigins = config.GetSectionValue<List<string>>(ConfigKeyConstants.AllowSpecificOrigins, null);
        return allowSpecificOrigins;
    }

    public static string LoggingLevelDefault(this ConfigurationManager config)
    {
        var loggingLevelDefault = config.GetSectionValue<string>(ConfigKeyConstants.LoggingLevelDefault, string.Empty);
        return loggingLevelDefault;
    }

    public static string LoggingLevelMsApnetCore(this ConfigurationManager config)
    {
        var loggingLevelDefault = config.GetSectionValue<string>(ConfigKeyConstants.LoggingLevelMsAspNetCore, string.Empty);
        return loggingLevelDefault;
    }

    public static T GetSectionValue<T>(this ConfigurationManager config, string sectionName)
    {
        if(!config.GetSection(sectionName).Exists())
        {
            return default(T);
        }
        var sValue = config.GetSection(sectionName).Get<T>();
        return sValue;
    }

    public static T GetSectionValue<T>(this ConfigurationManager config, string sectionName, T defaultValue)
    {
        if(!config.GetSection(sectionName).Exists())
        {
            return defaultValue;
        }

        var sValue = config.GetSection(sectionName).Get<T>();
        return sValue;
    }
}