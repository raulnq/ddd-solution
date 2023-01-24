using Application;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Rebus.Config;
using Serilog;
using System.Text.Json;
using System.Text.Json.Serialization;
using TaskManager.Application;
using TaskManager.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

try
{
    var name = $"Task Manager API ({Environment.GetEnvironmentVariable("RELEASE")})";

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

    builder.Services.AddTaskManagerApplication();

    builder.Services.AddApplication();

    builder.Services.AddTaskManagerInfrastructure(builder.Configuration.GetSection("Infrastructure"));

    builder.Services.AddInfrastructure(builder.Configuration.GetSection("Infrastructure"), name, "v1");

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }

    app.UseHealthChecks();

    app.UseSwagger();

    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", name));

    app.UseLocalization();

    app.UseProblemDetails();

    app.UseDefaultFiles();

    app.UseStaticFiles();

    if (app.Environment.IsProduction())
    {
        app.UseHttpsRedirection();
    }

    app.UseRouting();

    app.UseCors(app.Configuration.GetSection("Infrastructure"));

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




