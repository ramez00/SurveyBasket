using Hangfire;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SurveyBasket.Authentication;
using SurveyBasket.Errors;
using SurveyBasket.Health;
using SurveyBasket.Persistence;
using SurveyBasket.Settings;
using System.Diagnostics.Contracts;
using System.Reflection;
using System.Text;
using System.Threading.RateLimiting;

namespace SurveyBasket;

public static class Dependencies
{
    public static IServiceCollection AddAllDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
        services.AddHybridCache();
        services.AddHttpContextAccessor();

        services.Configure<MailSettings>(configuration.GetSection("MailSettings"));

        var allowedOrigins = configuration.GetSection("AllowedOrigins").Get<string[]>();

        services.AddCors(options =>
            options.AddPolicy("AllowMyDomain", builder =>
               builder
                    .AllowAnyHeader()
                    .AllowAnyMethod()                        // Specific Method ("POST","PUT","GET")
                    .AllowAnyOrigin()                       // allow all cors 
                                                            // .WithOrigins(allowedOrigins!)        // allow Specific Core 
            )
        );

        var connectionString = configuration.GetConnectionString("DefaultConnection") ?? 
                                    throw new InvalidOperationException("SQL DataBase Conncetion String..");

        var hangFireConnection = configuration.GetConnectionString("HangFire") ??
                                    throw new InvalidOperationException("HangFire Connection Not Exist...");

        services.AddHealthChecks()
            .AddSqlServer("DataBase",connectionString!)
            .AddHangfire(Options => { Options.MinimumAvailableServers = 1; })
            .AddUrlGroup( new Uri("https://www.google.com"),"GoogleAPI",tags : ["API"]) // add Health Check for external API
            .AddUrlGroup( new Uri("https://www.faceBook.com"),"MetaApi",tags: ["API"]) // if I have another external API 
            .AddCheck<MailServiceHealthCheck>("Mail Service", tags: ["mail"]); // add Health Check for Mail Service

        services.AddRateLimiter(option =>
        {
            option.RejectionStatusCode = StatusCodes.Status429TooManyRequests; // when u reached to maximun request

            option.AddSlidingWindowLimiter("SlidingLimiter", options =>
            {
                options.PermitLimit = 10; // max request per user
                options.Window = TimeSpan.FromMinutes(1); // time to reset the request
                options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst; // oldest request first
                options.QueueLimit = 2; // max request in queue
                options.SegmentsPerWindow = 2; // number of segments in the window
            });
        });

        services
                .AddBackgroundJobsConfig(hangFireConnection)
                .AddSewagerConfig()
                .AddMapsterConfig()
                .AddDataBaseConfig(connectionString!)
                .AddIdentityConfig()
                .AddSingleton<IJwtProvider, JwtProvider>()
                .AddAuthConfig(configuration)
                .AddTransient<IRoleService, RoleService>()
                .AddScoped<IPollServices, PollServices>()
                .AddScoped<IAuthService, AuthService>()
                .AddScoped<IQuestionService, QuestionService>()
                .AddScoped<IVoteServices, VoteServices>()
                .AddScoped<IEmailSender, EmailService>()
                .AddScoped<IUserService, UserService>()
                .AddFluentValidationConfig()
                .AddExceptionHandler<GlobalExceptionHandler>()
                .AddProblemDetails();


        return services;
    }

    private static IServiceCollection AddBackgroundJobsConfig(this IServiceCollection services, string hangFireConnection)
    {
        services
            .AddHangfire(options => options

                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSqlServerStorage(hangFireConnection)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
            )
            .AddHangfireServer();

        return services;
    }

    private static IServiceCollection AddSewagerConfig(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        return services;
    }

    private static IServiceCollection AddDataBaseConfig(this IServiceCollection services, string connectionString)
        => services.AddDbContext<ApplicationDbContext>(option => option.UseSqlServer(connectionString));

    private static IServiceCollection AddMapsterConfig(this IServiceCollection services)
    {
        var MappingConfig = TypeAdapterConfig.GlobalSettings;
        MappingConfig.Scan(Assembly.GetExecutingAssembly());

        services.AddSingleton<IMapper>(new Mapper(MappingConfig));

        services.AddMapster();

        return services;
    }

    private static IServiceCollection AddFluentValidationConfig(this IServiceCollection service)
        => service.AddFluentValidationAutoValidation()
                  .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

    private static IServiceCollection AddIdentityConfig(this IServiceCollection services)
    {
        services.AddIdentity<ApplicationUser,ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

        return services;
    }

    private static IServiceCollection AddAuthConfig(this IServiceCollection services, IConfiguration configuration)
    {

        var jwtSettings = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>();

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
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings?.Key!)),
                ValidIssuer = jwtSettings?.Issuer,
                ValidAudience = jwtSettings?.Audience,
            };
        });

        // services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));

        // to add validation regarding OPtions load at satrt 
        services.AddOptions<JwtOptions>()
           .BindConfiguration(JwtOptions.SectionName)
           .ValidateDataAnnotations()  // to validate all dataAnotaion
           .ValidateOnStart();         // to Validate once Start App

        services.Configure<IdentityOptions>(options =>
        {
            options.Password.RequiredLength = 6;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = false;
            options.User.RequireUniqueEmail = true;
            options.SignIn.RequireConfirmedEmail = true;
        });

        return services;
    }
}
