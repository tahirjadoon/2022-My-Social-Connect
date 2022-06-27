
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MSC.Api.Core.BusinessLogic;
using MSC.Api.Core.DB;
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
    
    public static string RegisterCors(this IServiceCollection collection, ConfigurationManager config)
    {
        var myAllowSpecificOrigins = "_myAllowSpecificOrigins";
        //https://stackoverflow.com/questions/42858335/how-to-hardcode-and-read-a-string-array-in-appsettings-json
        var allowedSpecificOrigins = config.GetAllowSpecificOrigins();
        if(allowedSpecificOrigins != null && allowedSpecificOrigins.Any()){
            collection.AddCors(options => {
                        options.AddPolicy(name: myAllowSpecificOrigins, 
                                        policy => {
                                            policy.WithOrigins(allowedSpecificOrigins.ToArray())
                                            .AllowAnyHeader()
                                            .AllowAnyMethod();
                                        });
                    });
        }
        return myAllowSpecificOrigins;
    }

    public static void RegisterDBContext(this IServiceCollection collection, ConfigurationManager config)
    {
        collection.AddDbContext<DataContext>(options => {
            options.UseSqlite(config.GetDefaultConnectionString());
        });
    }
}