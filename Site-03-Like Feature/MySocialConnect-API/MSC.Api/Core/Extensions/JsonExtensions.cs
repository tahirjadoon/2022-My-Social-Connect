using System.Text.Json;

namespace MSC.Api.Core.Extensions;
public static class JsonExtensions
{
    public static string ToJson<T>(this T data, bool isCamelCase = true)
    {
        if (data == null) return string.Empty;
        var jsonString = "";
        if (isCamelCase)
        {
            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            jsonString = JsonSerializer.Serialize<T>(data, options);
        }
        else
        {
            jsonString = JsonSerializer.Serialize<T>(data);
        }
        return jsonString;
    }

    public static string ToJsonIndented<T>(this T data, bool isCamelCase = true)
    {
        if (data == null) return string.Empty;
        var jsonString = "";
        var options = new JsonSerializerOptions { WriteIndented = true };
        if (isCamelCase)
        {
            options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        }
        jsonString = JsonSerializer.Serialize<T>(data, options);
        return jsonString;
    }

    public static T FromJson<T>(this string jsonString)
    {
        if (string.IsNullOrWhiteSpace(jsonString)) return default(T);
        var data = JsonSerializer.Deserialize<T>(jsonString);
        return data;
    }
}
