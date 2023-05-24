using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MSC.Api.Core.DB;
using MSC.Api.Core.Entities;
using MSC.Api.Core.Extensions;
using MSC.Api.Core.Middleware;
using MSC.Api.Core.SignalR;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//CUSTOM:Start
/*
Check: 
https://stackoverflow.com/questions/69722872/asp-net-core-6-how-to-access-configuration-during-startup
https://stackoverflow.com/questions/70865207/net-6-stable-iconfiguration-setup-in-program-cs
https://stackoverflow.com/questions/69722872/asp-net-core-6-how-to-access-configuration-during-startup/70161492?noredirect=1#comment128539331_70161492
*/
IConfiguration configuration = builder.Configuration; // allows both to access and to set up the config 
builder.Services.RegisterRepos(configuration);
builder.Services.RegisterDBContext(configuration);
var myAllowSpecificOrigins = builder.Services.RegisterCors(configuration);
builder.Services.AddIdentityServices(configuration);
builder.Services.AddSignalR();
//CUSTOM:End

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

//CUSTOM: Seed Data Start
using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
try
{
    //get the required service
    var context = services.GetRequiredService<DataContext>();
    //due to identity get the userManager and Role Manager then use it to seed users and roles
    var userManager = services.GetRequiredService<UserManager<AppUser>>();
    var roleManager = services.GetRequiredService<RoleManager<AppRole>>();
    //asynchronously applies an pending migrations for the context to the database. Will create the database if it doesn't exist already
    //just restarting the application will apply our migrations
    await context.Database.MigrateAsync();
    //await context.Database.ExecuteSqlRawAsync("DELETE FROM \"Connections\"");
    //now seed the users
    await Seed.SeedUsers(userManager, roleManager);
}
catch (Exception ex)
{
    var logger = services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occured during migration");
}
//CUSTOM: Seed Data End

//CUSTOM: Middleware Start
app.UseMiddleware<ExceptionMiddleware>();
//CUSTOM: Middleware End

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    //display -- this is the default
    //app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

//CUSTOM: Start 
//ordering is important here. UseCors before UseAuthentication and UseAuthentication before UseAuthorization
app.UseCors(myAllowSpecificOrigins);
app.UseAuthentication();
//tell routing about a hub end point and provide a route for accessing PresenceHub
app.MapHub<PresenceHub>("hubs/presence");
app.MapHub<MessageHub>("hubs/message");
//map to fallback controller after angular build and wwwroot folder getting created inside the MVC.Api folder
app.MapFallbackToController("Index", "Fallback");
//CUSTOM: End

app.UseAuthorization();

//CUSTOM: start
//serve static files as well. In this case we are building the angular prod app inside the MySocialConnect-API\MSC.Api folder
app.UseDefaultFiles(); //will fish out the index.html file from wwwwroot folder 
app.UseStaticFiles(); //will be looking for the wwwroot folder
//CUSTOM: END

app.MapControllers();

app.Run();
