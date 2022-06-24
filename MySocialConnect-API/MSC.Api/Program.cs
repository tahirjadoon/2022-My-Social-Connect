using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MSC.Api.Core.DB;
using MSC.Api.Core.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//CUSTOM:Start
/*
Check: 
https://stackoverflow.com/questions/69722872/asp-net-core-6-how-to-access-configuration-during-startup
https://stackoverflow.com/questions/70865207/net-6-stable-iconfiguration-setup-in-program-cs
*/
ConfigurationManager configuration = builder.Configuration; // allows both to access and to set up the config 
builder.Services.AddDbContext<DataContext>(options => {
    options.UseSqlite(configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.RegisterRepos();
//CUSTOM:End

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
