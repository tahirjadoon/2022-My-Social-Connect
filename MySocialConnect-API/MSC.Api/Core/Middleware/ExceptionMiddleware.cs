using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
//using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MSC.Api.Core.Constants;
using MSC.Api.Core.Dto;

namespace MSC.Api.Core.Middleware;
public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly IHostEnvironment _env;

    /// <summary>
    /// Receives RequestDelegate which is whats next in the middle ware pipeline
    /// </summary>
    /// <param name="next">What is next in the pipeline</param>
    /// <param name="logger">So to log the exception</param>
    /// <param name="env">The environment development/production</param>
    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    /// <summary>
    /// The required method to invoke the middleware
    /// </summary>
    /// <param name="context">The http context</param>
    /// <returns></returns>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            //pass the context to the next piece of middleware
            await _next(context);
        }
        catch(Exception ex)
        {
            //log the error
            _logger.LogError(ex, ex.Message);
            //set content type
            context.Response.ContentType = ContentTypeConstants.ApplicationJson;
            //set status code
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            //create the response model
            ApiExceptionDto response = null;
            if(_env.IsDevelopment())
            {
                //development put out the exact message and stack trace
                response = new ApiExceptionDto(context.Response.StatusCode, ex.Message, ex.StackTrace?.ToString());
            }
            else
            {
                //production do not put out the exact message and stack trace
                response = new ApiExceptionDto(context.Response.StatusCode, "Internal Server Error");
            }

            //want the json responses to go as camel case 
            var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

            //serialize the response
            var json = JsonSerializer.Serialize(response, jsonOptions);

            //write
            await context.Response.WriteAsync(json);

        }
    }
}