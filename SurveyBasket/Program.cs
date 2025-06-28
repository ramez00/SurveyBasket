using Hangfire;
using HangfireBasicAuthenticationFilter;
using Microsoft.EntityFrameworkCore;
using Serilog;
using SurveyBasket;
using SurveyBasket.Persistence;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Identity.Client;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAllDependencies(builder.Configuration);


builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration)
);

// Different between response cache and output cache and Distributed cache
// => output cache is used to cache on server ,
// while response cache is used to cache the response of a requester's memory.
// In Memory cache => used to cache the data in memory of server 
// if u have more than one server, the data will not be shared between them
// =====> distributed cache is used to cache the data in a shared location OR multiple servers.

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI( options =>
    {
        var description = app.DescribeApiVersions();
        foreach (var item in description)
            options.SwaggerEndpoint($"/swagger/{item.GroupName}/swagger.json", item.GroupName.ToUpperInvariant());
    });
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseCors("AllowMyDomain"); // allow all origins && allow Specific Origin app.UseCors("Name of Core I defined")

app.UseAuthorization();

app.UseHangfireDashboard("/jobs", new DashboardOptions
{
    Authorization =
    [
       new HangfireCustomBasicAuthenticationFilter
       {
           User = app.Configuration.GetValue<string>("HangFireSettings:userName"),
           Pass = app.Configuration.GetValue<string>("HangFireSettings:Password")
       }
    ],
    DashboardTitle = "Survey Basket DashBoard Jobs" // to change title of screen 
});

app.MapControllers();

app.UseExceptionHandler();

app.UseRateLimiter();

app.MapHealthChecks("/health",new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse // to change the response of health check
});

app.MapHealthChecks("/health-check-api", new HealthCheckOptions  // if i need to sperate health check 
{
    Predicate = x => x.Tags.Contains("API"), // to generate new URL to check only APIs service health check
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse // to change the response of health check
});

app.Run();
