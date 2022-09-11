using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MSC.Api.Core.DB;
using MSC.Api.Core.Extensions;
using MSC.Api.Core.Middleware;

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
builder.Services.RegisterRepos();
builder.Services.RegisterDBContext(configuration);
var myAllowSpecificOrigins = builder.Services.RegisterCors(configuration);
builder.Services.AddIdentityServices(configuration);
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
    //asynchronously applies an pending migrations for the context to the database. Will create the database if it doesn't exist already
    //just restarting the application will apply our migrations
    await context.Database.MigrateAsync();
    //now seed the users
    await Seed.SeedUsers(context);
}
catch(Exception ex)
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
//CUSTOM: End

app.UseAuthorization();

app.MapControllers();

app.Run();
