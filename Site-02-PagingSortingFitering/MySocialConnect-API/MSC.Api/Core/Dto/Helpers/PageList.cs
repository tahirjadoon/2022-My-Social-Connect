using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MSC.Api.Core.Dto.Helpers;
public class PageList<T> : List<T>
{
    public PageList(IEnumerable<T> items, int count, int pageNumber, int pageSize)
    {
        CurrentPage = pageNumber;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        PageSize = pageSize;
        TotalCount = count;

        //to have access to the items in the page list
        AddRange(items);
    }

    /// <summary>
    /// Current page number
    /// </summary>
    public int CurrentPage { get; set; }
    /// <summary>
    /// Total pages
    /// </summary>
    public int TotalPages { get; set; }
    /// <summary>
    /// Page size, total records to pull for the page
    /// </summary>
    public int PageSize { get; set; }
    /// <summary>
    /// Total records available
    /// </summary>
    public int TotalCount { get; set; }

    //static method which will receive the IQueryable pageNumber and pageSize and return the page data
    public static async Task<PageList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
    {
        //get the total count
        var count = await source.CountAsync();
        //skip the pages to go to the intended page pick the records
        var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        //return the data as paged list
        var data = new PageList<T>(items, count, pageNumber, pageSize);
        return data;
    }
}
