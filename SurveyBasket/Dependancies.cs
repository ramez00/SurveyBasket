using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using SurveyBasket.Persistence;
using System.Diagnostics.Contracts;
using System.Reflection;

namespace SurveyBasket;

public static class Dependancies
{
    public static IServiceCollection AddAllDependacies(this IServiceCollection services,IConfiguration configuration)
    {
        services.AddControllers();

        var connectionString = configuration.GetConnectionString("DefualtConnection") ??
                               throw new InvalidOperationException("Connection String....");
        services
                .AddDataBaseConfig(connectionString)
                .AddSewagerConfig()
                .AddMapsterConfig();

        return services;
    }

    public static IServiceCollection AddSewagerConfig(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        return services;
    }

    public static IServiceCollection AddDataBaseConfig(this IServiceCollection services,string connectionString) 
        => services.AddDbContext<ApplicationDbContext>(option => option.UseSqlServer(connectionString));

    public static IServiceCollection AddMapsterConfig(this IServiceCollection services)
    {
        var MappingConfig = TypeAdapterConfig.GlobalSettings;
        MappingConfig.Scan(Assembly.GetExecutingAssembly());

        services.AddSingleton<IMapper>(new Mapper (MappingConfig));

        services.AddMapster();
        
        return services;
    }
}
