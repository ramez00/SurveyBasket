﻿using Asp.Versioning;
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
using Microsoft.OpenApi.Models;
using SurveyBasket.Authentication;
using SurveyBasket.Errors;
using SurveyBasket.Health;
using SurveyBasket.Persistence;
using SurveyBasket.Settings;
using SurveyBasket.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
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
        services.AddApiVersioning(options =>
        {
            options.ApiVersionReader = new MediaTypeApiVersionReader("api-version"); // to read version from media type header (e.g. application/json;api-version=1.0)
            options.ReportApiVersions = true; // to report api version in response header
            options.AssumeDefaultVersionWhenUnspecified = true; // if client not specify version we will use default version
            options.DefaultApiVersion = new ApiVersion(1); // default version
            options.ReportApiVersions = true; // to report api version in response header
        }).AddApiExplorer( Options  =>
        {
            Options.GroupNameFormat = "'v'V"; // to format version in the API explorer
            Options.SubstituteApiVersionInUrl = true; // to substitute version in the URL
        });

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
            .AddUrlGroup( new Uri("https://les-dev.net/"),"LesCompany",tags: ["Company Support"]) // if I have another external API 
            .AddCheck<MailServiceHealthCheck>("Mail Service", tags: ["mail"]); // add Health Check for Mail Service

       

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
                .AddProblemDetails()
                .AddRateLimiter();


        return services;
    }


    private static IServiceCollection AddRateLimiter(this IServiceCollection services)
    {
        services.AddRateLimiter(option =>
        {
            option.RejectionStatusCode = StatusCodes.Status429TooManyRequests; // when u reached to maximun request

            option.AddPolicy(RateLimiterConstant.IpRateLimiter, httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 2,                            // number of allowance request per time
                        Window = TimeSpan.FromSeconds(20),   // time to allow request
                    }
                )
            );

            // wanna to add rate limit for specific user
            // if user not login or not authenticated we will use anonymous user
            option.AddPolicy(RateLimiterConstant.UserRateLimiter, httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: httpContext.User.GetUserId() ?? "anonymous",
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 5,                             // number of allowance request per time => when client send 5 request will blocked based on time below
                        Window = TimeSpan.FromSeconds(30),    // time to allow request
                    }
                )
            );

            option.AddConcurrencyLimiter(RateLimiterConstant.ConcurrencyLimiter, options =>
            {
                options.PermitLimit = 100; // only one request at a time
                options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst; // oldest request will be processed first
                options.QueueLimit = 10; // maximum number of requests that can be queued
            });
        });

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
        services.AddSwaggerGen(options =>
        {
            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        });

        services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

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
