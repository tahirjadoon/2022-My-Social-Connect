using System.Globalization;

namespace MSC.Api.Core.Extensions;
public static class StringExtensions
{
    public static string ToTitleCase(this string s)
    {
        if(string.IsNullOrWhiteSpace(s)) return s;

        var newS = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(s.ToLowerInvariant());
        return newS;
    }
}