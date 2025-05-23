using Hangfire;
using HangfireBasicAuthenticationFilter;
using Microsoft.EntityFrameworkCore;
using Serilog;
using SurveyBasket;
using SurveyBasket.Persistence;

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
    app.UseSwaggerUI();
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

app.Run();
