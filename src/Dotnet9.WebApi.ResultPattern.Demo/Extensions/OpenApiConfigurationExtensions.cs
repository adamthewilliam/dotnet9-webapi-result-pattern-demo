using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;

namespace Dotnet9.WebApi.ResultPattern.Demo.Extensions;

public static class OpenApiConfigurationExtensions
{
    public static void AddOpenApiConfiguration(this IServiceCollection services)
    {
        services.AddOpenApi();

        /* This extension is responsible for crawling the controller action annotations
        to generate the OpenApi specification */
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "v1 Penguins API",
                Version = "v1",
                Description = "An example ASP.NET Core Web API using Swagger with Scalar UI",
            });
        });
    }

    public static void AddScalarUiConfiguration(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment()) return;
        
        // Enable Swagger
        app.UseSwagger();
        
        app.MapOpenApi().CacheOutput();
        
        // Enable Scalar UI
        app.MapScalarApiReference(options =>
        {
            options
                .WithTitle("v1 Penguins API")
                .WithTheme(ScalarTheme.DeepSpace)
                .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient)
                .WithOpenApiRoutePattern("/swagger/v1/swagger.json"); // Setup Scalar UI to route to swagger.json

            options.Servers ??= new List<ScalarServer>();
            options.Servers.Add(new ScalarServer("http://localhost:5199"));
        });
    }
}