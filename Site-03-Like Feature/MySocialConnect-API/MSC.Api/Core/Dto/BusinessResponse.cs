using System.Net;

namespace MSC.Api.Core.Dto;
public class BusinessResponse
{
    public BusinessResponse()
    {
    }

    public BusinessResponse(HttpStatusCode httpStatusCode, string message = "")
    {
        HttpStatusCode = httpStatusCode;
        Message = message;
    }

    public System.Net.HttpStatusCode HttpStatusCode { get; set; }
    public string Message { get; set; }
}
