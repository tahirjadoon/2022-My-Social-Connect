
using AutoMapper;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MSC.Api.Core.BusinessLogic;
using MSC.Api.Core.DB;
using MSC.Api.Core.Repositories;
using MSC.Api.Core.Services;
using MSC.Api.Core.Dto.AutoMapper;
using MSC.Api.Core.Constants;
using MSC.Api.Core.Dto.Helpers;
using MSC.Api.Core.ActionFilters;
using MSC.Api.Core.SignalR;
using MSC.Api.Core.DB.UnitOfWork;

namespace MSC.Api.Core.Extensions;
public static class ServiceExtensions
{
    public static void RegisterRepos(this IServiceCollection services, IConfiguration config)
    {
        /*
        check 
        https://stackoverflow.com/questions/69722872/asp-net-core-6-how-to-access-configuration-during-startup
        https://stackoverflow.com/questions/70865207/net-6-stable-iconfiguration-setup-in-program-cs
        https://stackoverflow.com/questions/69722872/asp-net-core-6-how-to-access-configuration-during-startup/70161492?noredirect=1#comment128539331_70161492
        */

        //AddScoped: is for the life time of the request. Use this for the http requests
        //AddTransient: a new instance is provided to every request
        //AddSingleton: objects are the same for every object and every request

        //add singleton items 
        //SignalR add presenceTracker as a singleton
        services.AddSingleton<PresenceTracker>();

        //adding the Cloudinary to read data from
        services.Configure<CloudinaryConfig>(config.GetSection(ConfigKeyConstants.CloudinarySettingsKey));

        //AutoMapper Profile
        services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);

        //repositories
        //using unit of work so individual repos commented
        /*
        services.AddScoped<IUsersRepository, UsersRepository>();
        services.AddScoped<ILikesRepository, LikesRepository>();
        services.AddScoped<IMessageRepository, MessageRepository>();
        services.AddScoped<ISignalRRepository, SignalRRepository>();
        */
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        //business logic
        services.AddScoped<IUsersBusinessLogic, UsersBusinessLogic>();
        services.AddScoped<ILikesBusinessLogic, LikesBusinessLogic>();
        services.AddScoped<IMessageBusinessLogic, MessageBusinessLogic>();
        services.AddScoped<ISignalRBusinessLogic, SignalRBusinessLogic>();

        //action filters
        services.AddScoped<LogUserActivityFilter>();

        //services
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IPhotoService, PhotoService>();
    }

    public static string RegisterCors(this IServiceCollection services, IConfiguration config)
    {
        var myAllowSpecificOrigins = "_myAllowSpecificOrigins";
        //https://stackoverflow.com/questions/42858335/how-to-hardcode-and-read-a-string-array-in-appsettings-json
        var allowedSpecificOrigins = config.GetAllowSpecificOrigins();
        if (allowedSpecificOrigins != null && allowedSpecificOrigins.Any())
        {
            services.AddCors(options =>
            {
                options.AddPolicy(name: myAllowSpecificOrigins,
                                policy =>
                                {
                                    policy.WithOrigins(allowedSpecificOrigins.ToArray())
                                    .AllowAnyHeader()
                                    .AllowAnyMethod()
                                    //signalR
                                    .AllowCredentials()
                                    ;
                                });
            });
        }
        return myAllowSpecificOrigins;
    }

    public static void RegisterDBContext(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<DataContext>(options =>
        {
            options.UseSqlite(config.GetDefaultConnectionString());
        });
    }
}