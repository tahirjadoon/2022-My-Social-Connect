using System.Linq;

namespace MSC.Api.Core.Extensions;

public static class ByteExtensions
{
    public static bool AreEqual(this byte[] a, byte[] b)
    {
        var areEqual = a.SequenceEqual(b);
        return areEqual;
    }
}