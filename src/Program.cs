using Dotnet9.WebApi.ResultPattern.Demo.Data;
using Dotnet9.WebApi.ResultPattern.Demo.Extensions;
using Dotnet9.WebApi.ResultPattern.Demo.FluentResults;
using Dotnet9.WebApi.ResultPattern.Demo.Services;
using FluentResults.Extensions.AspNetCore;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();

builder.Services.AddControllers();

builder.Services.AddOpenApiConfiguration();

builder.Services.AddSingleton<FluentResultsEndpointProfile>();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("InMemoryPenguinDb")); 

builder.Services.AddScoped<IPenguinService, PenguinService>();

var app = builder.Build();

var httpContextAccessor = app.Services.GetRequiredService<IHttpContextAccessor>();
var profile = app.Services.GetRequiredService<FluentResultsEndpointProfile>();

profile.SetHttpContextProvider(() => httpContextAccessor.HttpContext!);

AspNetCoreResult.Setup(options =>
{
    options.DefaultProfile = profile;
});

app.AddScalarUiConfiguration();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();