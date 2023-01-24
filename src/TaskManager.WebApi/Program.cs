using Application;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Text.Json;
using System.Text.Json.Serialization;
using TaskManager.Application;
using TaskManager.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

try
{
    Log.Information("Starting host");

    builder.Services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

    builder.Services.Configure<ApiBehaviorOptions>(options =>
    {
        options.SuppressModelStateInvalidFilter = true;
    });

    var infrastructureConfiguration = builder.Configuration.GetSection("Infrastructure");

    builder.Services.AddTaskManagerApplication();

    builder.Services.AddApplication();

    builder.Services.AddTaskManagerInfrastructure(infrastructureConfiguration);

    builder.Services.AddInfrastructure(infrastructureConfiguration);

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }

    app.UseHealthChecks();

    app.UseSwagger(infrastructureConfiguration);

    app.UseLocalization();

    app.UseProblemDetails();

    app.UseDefaultFiles();

    app.UseStaticFiles();

    if (app.Environment.IsProduction())
    {
        app.UseHttpsRedirection();
    }

    app.UseRouting();

    app.UseCors(infrastructureConfiguration);

    app.UseAuthorization();

    app.UseHttpLogging();

    app.MapControllers();

    app.Run();
}
finally
{
    Log.Information("Ending host");

    Log.CloseAndFlush();
}




