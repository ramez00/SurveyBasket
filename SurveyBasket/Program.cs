using Microsoft.EntityFrameworkCore;
using SurveyBasket;
using SurveyBasket.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAllDependacies(builder.Configuration);

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
