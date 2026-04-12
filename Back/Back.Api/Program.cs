using Back.Api.Extensions;
using Back.Api.Middleware;
using Back.Dal;
using Back.Domain;
using Back.Infrastructure.Extensions;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Load environment variables
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();
Log.Logger = new LoggerConfiguration().MinimumLevel.Debug().CreateLogger();

try
{
    Log.Information("Back Server is starting...");
    builder.Services.AddLoggerService();
    builder.Services.AddMongoDbServices(builder.Configuration);
    builder.Services.AddSupabaseServices(builder.Configuration);
    builder.Services.AddJwtAuthentication(builder.Configuration);
    builder.Services.AddApiModules();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.DalInjectionModule();
    builder.Services.DomainInjectionModule();

    builder.Services.AddSwaggerGen(c => {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Back API",
            Version = "v1"
        });
    });
    builder.Services.SetCorsPolicy();

    WebApplication app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Back API V1"));
    }

    app.UseHttpsRedirection();
    app.UseRouting();
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseCustomMiddlewares();
    app.EnableCors();
    app.MapRouters();
    app.Run();
}

catch (Exception ex)
{
    Log.Fatal(ex, "Application Terminated");
    throw;
}

finally
{
    Log.CloseAndFlush();
}
