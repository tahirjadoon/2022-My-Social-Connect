
using Microsoft.Extensions.DependencyInjection;
using MSC.Api.Core.BusinessLogic;
using MSC.Api.Core.Repositories;

namespace MSC.Api.Core.Extensions;
public static class ServiceExtensions
{
    public static void RegisterRepos(this IServiceCollection collection)
    {
        //repositories
        collection.AddTransient<IUsersRepository, UsersRepository>();

        //business logic
        collection.AddTransient<IUsersBusinessLogic, UsersBusinessLogic>();
    }
}