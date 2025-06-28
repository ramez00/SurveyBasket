using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SurveyBasket.Swagger;

public class ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider) : IConfigureOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider _provider = provider;


    // Scan for all controllers and Endpoint to check if they have the ApiVersion attribute

    public void Configure(SwaggerGenOptions options)
    {
        foreach (var item in _provider.ApiVersionDescriptions)
            options.SwaggerDoc(item.GroupName, createInfoForApiVersion(item));

        // Add option to send token with request in Swagger UI
        options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Description = "Please Add your token",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = JwtBearerDefaults.AuthenticationScheme,
            BearerFormat = "JWT",
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = JwtBearerDefaults.AuthenticationScheme
                    }
                },
                Array.Empty<string>()
            }
        });
    }

    private static OpenApiInfo createInfoForApiVersion(ApiVersionDescription item) =>
        new()
        {
            Title = "Survey Basket API",
            Version = item.ApiVersion.ToString(),
            Description = "Survey Basket API is a RESTful API for managing surveys and polls.",
        };
}
