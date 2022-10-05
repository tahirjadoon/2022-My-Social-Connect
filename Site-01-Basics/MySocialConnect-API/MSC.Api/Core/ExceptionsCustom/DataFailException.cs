using System;

namespace MSC.Api.Core.ExceptionsCustom;
public class DataFailException : Exception
{
    public DataFailException()
    {
    }

    public DataFailException(string message) : base(message)
    {
    }

    public DataFailException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
