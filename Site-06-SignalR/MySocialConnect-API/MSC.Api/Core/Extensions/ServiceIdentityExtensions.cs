using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using MSC.Api.Core.Constants;
using MSC.Api.Core.DB;
using MSC.Api.Core.Entities;

namespace MSC.Api.Core.Extensions;

public static class IdentityServiceExtensions
{
    public static void AddIdentityServices(this IServiceCollection services, IConfiguration config)
    {
        //for mvc use services.AddIdentity. For the api we can't do that
        services.AddIdentityCore<AppUser>(opt =>
        {
            //there are a lot of options that we can configure here
            //pick per the site password scheme  
            opt.Password.RequireNonAlphanumeric = false;
            opt.Password.RequireDigit = false;
            opt.Password.RequireLowercase = false;
            opt.Password.RequireUppercase = false;
        })
        //roles
        .AddRoles<AppRole>()
        //role manager
        .AddRoleManager<RoleManager<AppRole>>()
        //Sign in manager
        .AddSignInManager<SignInManager<AppUser>>()
        //Role validator
        .AddRoleValidator<RoleValidator<AppRole>>()
        //and finally add store to create all the identity related tables 
        .AddEntityFrameworkStores<DataContext>();
        ;

        var tokenKey = Encoding.UTF8.GetBytes(config.GetTokenKey());
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(tokenKey),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });

        //add authorization policies
        services.AddAuthorization(opt =>
        {
            opt.AddPolicy(SiteIdentityConstants.AuthPolicy_RequireAdminRole, policy => policy.RequireRole(SiteIdentityConstants.Role_Admin));
            opt.AddPolicy(SiteIdentityConstants.AuthPolicy_ModeratePhotoRole, policy => policy.RequireRole(SiteIdentityConstants.Role_Admin, SiteIdentityConstants.Role_Moderator));
        });
    }
}