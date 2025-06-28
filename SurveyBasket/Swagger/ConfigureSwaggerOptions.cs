using Asp.Versioning.ApiExplorer;
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
    }

    private static OpenApiInfo createInfoForApiVersion(ApiVersionDescription item) =>
        new()
        {
            Title = "Survey Basket API",
            Version = item.ApiVersion.ToString(),
            Description = "Survey Basket API is a RESTful API for managing surveys and polls.",
        };
}
