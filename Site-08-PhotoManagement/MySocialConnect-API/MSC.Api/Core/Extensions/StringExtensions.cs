using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace MSC.Api.Core.Extensions;
public static class StringExtensions
{
    public static string ToTitleCase(this string s)
    {
        if (string.IsNullOrWhiteSpace(s)) return s;

        var newS = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(s.ToLowerInvariant());
        return newS;
    }

    public static IEnumerable<T> StringSplitToType<T>(this string value, string delimiter = ",")
    {
        var defaultVal = default(IEnumerable<T>);
        if (string.IsNullOrWhiteSpace(value)) return defaultVal;
        var splitResult = value.Split(new[] { delimiter }, StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim()).ToArray();
        if (splitResult.Length <= 0 || (splitResult.Length == 1 && string.IsNullOrWhiteSpace(splitResult[0])))
            return defaultVal;

        var newO = Activator.CreateInstance<List<T>>();
        foreach (var item in splitResult)
        {
            try
            {
                newO.Add((T)Convert.ChangeType(item, typeof(T)));
            }
            catch { }

        }
        if (!newO.Any()) return defaultVal;
        return newO;
    }
}