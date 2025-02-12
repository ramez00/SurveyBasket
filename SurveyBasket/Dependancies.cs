using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SurveyBasket.Authentication;
using SurveyBasket.Persistence;
using System.Diagnostics.Contracts;
using System.Reflection;
using System.Text;

namespace SurveyBasket;

public static class Dependencies
{
    public static IServiceCollection AddAllDependencies(this IServiceCollection services,IConfiguration configuration)
    {
        services.AddControllers();

        var connectionString = configuration.GetConnectionString("DefualtConnection") ??
                               throw new InvalidOperationException("Connection String....");

        services
                .AddSewagerConfig()
                .AddMapsterConfig()
                .AddDataBaseConfig(connectionString)
                .AddIdentityConfig()
                .AddSingleton<IJwtProvider, JwtProvider>()
                .AddAuthConfig()
                .AddScoped<IPollServices, PollServices>()
                .AddScoped<IAuthService, AuthService>()
                .AddFluentValidationConfig();

        return services;
    }

    private static IServiceCollection AddSewagerConfig(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        return services;
    }

    private static IServiceCollection AddDataBaseConfig(this IServiceCollection services,string connectionString) 
        => services.AddDbContext<ApplicationDbContext>(option => option.UseSqlServer(connectionString));

    private static IServiceCollection AddMapsterConfig(this IServiceCollection services)
    {
        var MappingConfig = TypeAdapterConfig.GlobalSettings;
        MappingConfig.Scan(Assembly.GetExecutingAssembly());

        services.AddSingleton<IMapper>(new Mapper (MappingConfig));

        services.AddMapster();
        
        return services;
    }

    private static IServiceCollection AddFluentValidationConfig(this IServiceCollection service) 
        => service.AddFluentValidationAutoValidation()
                  .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

    private static IServiceCollection AddIdentityConfig(this IServiceCollection services)
    {
        services.AddIdentityApiEndpoints<ApplicationUser>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

        return services;
    }

    private static IServiceCollection AddAuthConfig(this IServiceCollection services)
    {
            services.AddAuthentication(Options =>
            {
                Options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                Options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o =>
            {
                o.SaveToken = true;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("LQl5vdJD7eFbb3E9ohJRbaCwVOpseRrF")),
                    ValidIssuer = "WebApp",
                    ValidAudience = "WebApp Users"
                };
            });
        
        return services;
    }
}
