using Microsoft.EntityFrameworkCore;
using Serilog;
using SurveyBasket;
using SurveyBasket.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAllDependencies(builder.Configuration);

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration)
);

builder.Services.AddOutputCache(); // Different between response cache and output cache => output cache is used to cache on server , while response cache is used to cache the response of a requester's memory.

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

app.UseOutputCache();

app.MapIdentityApi<ApplicationUser>();

app.MapControllers();

app.UseExceptionHandler();

app.Run();
