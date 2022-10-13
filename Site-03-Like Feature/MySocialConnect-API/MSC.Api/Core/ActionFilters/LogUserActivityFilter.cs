using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using MSC.Api.Core.BusinessLogic;
using MSC.Api.Core.Extensions;

namespace MSC.Api.Core.ActionFilters;
public class LogUserActivityFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        //update after the activity so use next 
        var resultContext = await next();

        //user must be logged in 
        if (!resultContext.HttpContext.User.Identity.IsAuthenticated)
            return;

        //we can get the individual properties or the full claims object that has every thing 
        var userName = resultContext.HttpContext.User.GetUserName();
        var guid = resultContext.HttpContext.User.GetUserGuid();
        var id = resultContext.HttpContext.User.GetUserId();
        var claims = resultContext.HttpContext.User.GetUserClaims();
        if (claims == null)
            return;

        //get the reference to the user business logic
        var userBl = resultContext.HttpContext.RequestServices.GetService<IUsersBusinessLogic>();

        //call method to update the last active date 
        await userBl.LogUserActivityAsync(id);
    }
}