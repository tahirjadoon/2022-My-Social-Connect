using Microsoft.AspNetCore.Http;
using MSC.Api.Core.Constants;
using MSC.Api.Core.Dto.Helpers;

namespace MSC.Api.Core.Extensions;
public static class HttpExtensions
{
    /// <summary>
    /// add pagination header onto the response
    /// </summary>
    /// <param name="response"></param>
    /// <param name="currentPage"></param>
    /// <param name="itemsPerPage"></param>
    /// <param name="totalItems"></param>
    /// <param name="totalPages"></param>
    public static void AddPaginationHeader(this HttpResponse response, int currentPage, int itemsPerPage, int totalItems, int totalPages)
    {
        var paginationHeader = new PaginationHeader(currentPage, itemsPerPage, totalItems, totalPages);
        var paginationHeaderSerialize = paginationHeader.ToJson();
        //write custom header. No more adding X- to it. Give a sensible name
        response.Headers.Add(HeaderNameConstants.Pagination, paginationHeaderSerialize);
        //need to add the CORS header as well since a custom header is being used to make it available
        //cors header must be specific name
        response.Headers.Add(HeaderNameConstants.AccessControlExposeHeaders, HeaderNameConstants.Pagination);
    }
}
