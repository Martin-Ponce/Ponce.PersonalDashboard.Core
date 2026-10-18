using Asp.Versioning;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Ponce.PersonalDashboard.Core.API.Dtos.Configurations;
using Ponce.PersonalDashboard.Core.API.Helpers;
using Ponce.PersonalDashboard.Core.API.Helpers.HealthChecks;
using Ponce.PersonalDashboard.Core.API.Helpers.Security;
using Ponce.PersonalDashboard.Core.API.Helpers.Swagger;
using Ponce.PersonalDashboard.Core.Application.Contracts;
using Ponce.PersonalDashboard.Core.Application.Services;
using Ponce.PersonalDashboard.Core.Domain.Contracts;
using Ponce.PersonalDashboard.Core.Infrastructure.Configurations;
using Ponce.PersonalDashboard.Core.Infrastructure.Contexts;
using Ponce.PersonalDashboard.Core.Infrastructure.Repositories;
using Serilog;
using System.Globalization;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using static Ponce.PersonalDashboard.Core.API.Helpers.ApiConstants;

namespace Ponce.PersonalDashboard.Core.API.Extensions
{
    public static class ServiceExtensions
    {
        /// <summary>Validates the identity provider's access tokens, this API has no login of its own.</summary>
        public static IServiceCollection ConfigureAuthentication(this IServiceCollection services, GeneralConfiguration configuration, IHostEnvironment environment)
        {
            // Dev only: sign everyone in as a fixed local user, no identity provider needed.
            if (environment.IsDevelopment() && configuration.Authentication.UseDevelopmentUser)
            {
                services.AddAuthentication(DevelopmentAuthenticationHandler.SCHEME)
                    .AddScheme<AuthenticationSchemeOptions, DevelopmentAuthenticationHandler>(DevelopmentAuthenticationHandler.SCHEME, null);
                services.AddAuthorization();
                services.AddHttpContextAccessor();
                services.AddScoped<ICurrentUser, CurrentUser>();
                return services;
            }

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    // The token's "iss" has to match the Authority exactly.
                    options.Authority = configuration.Authentication.Authority;
                    options.RequireHttpsMetadata = configuration.Authentication.RequireHttpsMetadata;
                    // Keep the IdP's short claim names ("sub", "name", "role") so IdentityClaims can read them as-is.
                    options.MapInboundClaims = false;
                    options.TokenValidationParameters.NameClaimType = IdentityClaims.NAME;
                    options.TokenValidationParameters.RoleClaimType = IdentityClaims.ROLE;
                    // The IdP issues tokens without an "aud" claim, so audience validation has to be off.
                    options.TokenValidationParameters.ValidateAudience = false;
                    // In Docker the issuer isn't reachable from the container, so fetch keys from the internal address.
                    if (configuration.Authentication.InternalAuthority is { } internalAuthority)
                        options.BackchannelHttpHandler = new InternalAuthorityHandler(
                            new Uri(configuration.Authentication.Authority), new Uri(internalAuthority));
                });
            services.AddAuthorization();
            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUser, CurrentUser>();
            return services;
        }

        public static IServiceCollection ConfigureJson(this IServiceCollection services)
        {
            services.ConfigureHttpJsonOptions(options => options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));
            return services;
        }

        public static IServiceCollection ConfigureSwagger(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.DocInclusionPredicate((docName, apiDesc) =>
                    apiDesc.ActionDescriptor?.EndpointMetadata?.OfType<SwaggerIgnoreAttribute>().Any() != true);
                options.TagActionsBy(api =>
                {
                    var tags = api.ActionDescriptor?.EndpointMetadata?
                            .OfType<TagsAttribute>()
                            .SelectMany(t => t.Tags)
                            .ToList();

                    if (tags?.Count > 0)
                        return tags;

                    if (api.GroupName != null)
                        return [api.GroupName];

                    return [APP_NAME];
                });
                options.CustomSchemaIds(type =>
                {
                    if (type.IsGenericType)
                    {
                        var genericTypeName = type.GetGenericTypeDefinition().Name;
                        genericTypeName = genericTypeName[..genericTypeName.IndexOf('`')];
                        var genericArgs = string.Join("_", type.GetGenericArguments().Select(t => t.Name));
                        return $"{genericTypeName}{genericArgs}".FromPascal();
                    }
                    var attr = type.GetCustomAttribute<SwaggerSchemaNameAttribute>();
                    if (attr != null)
                        return attr.Name;

                    if (type.Name.EndsWith("Request"))
                        return type.Name.Replace("Request", "").FromPascal();

                    if (type.Name.EndsWith("Response"))
                        return type.Name.FromPascal();

                    return type.Name;
                });
                options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Description = "Access token issued by the identity provider (without the 'Bearer ' prefix)."
                });
                options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference(JwtBearerDefaults.AuthenticationScheme, document)] = []
                });
            });
            services.ConfigureOptions<SwaggerConfig>();
            return services;
        }

        public static IServiceCollection ConfigureCors(this IServiceCollection services, GeneralConfiguration configuration)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(ALLOW_ALL_CORS_POLICY,
                    builder => builder.WithOrigins(configuration.Cors.Origins)
                    .WithExposedHeaders(configuration.Cors.Headers)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });
            return services;
        }

        public static IServiceCollection ConfigureHealthChecks(this IServiceCollection services)
        {
            services.AddHealthChecks()
                .AddCheck("Liveness", () => HealthCheckResult.Healthy($"API started correctly. v{ASSEMBLY_VERSION}"))
                .AddCheck<DatabaseHealthCheck>("Database", tags: ["ready"]);
            return services;
        }

        public static IServiceCollection ConfigureLogger(this IServiceCollection services, GeneralConfiguration configuration)
        {
            Log.Logger = new LoggerConfiguration()
                    .AddConfiguration(configuration)
                    .Destructure.ToMaximumDepth(3)
                    .CreateLogger();
            services.AddSerilog();
            return services;
        }

        public static IServiceCollection ConfigureServices(this IServiceCollection services, GeneralConfiguration configuration)
        {
            services.AddScoped<IRepository, Repository<AppDbContext>>();

            // Repositories
            services.AddScoped<IExampleItemRepository, ExampleItemRepository>();
            services.AddScoped<IFinancesRepository, FinancesRepository>();
            services.AddScoped<IJobApplicationRepository, JobApplicationRepository>();
            services.AddScoped<IUpcomingDateRepository, UpcomingDateRepository>();
            services.AddScoped<IGoalRepository, GoalRepository>();
            services.AddScoped<IUserRepository, UserRepository>();

            // Application services
            services.AddScoped<ExampleItemService>();
            services.AddScoped<FinancesService>();
            services.AddScoped<JobApplicationService>();
            services.AddScoped<UpcomingDateService>();
            services.AddScoped<GoalService>();
            services.AddScoped<UserService>();
            services.AddScoped<OverviewService>();
            return services;
        }

        public static IServiceCollection ConfigureContexts(this IServiceCollection services, GeneralConfiguration configuration)
        {
            services.AddAppDbContext(configuration.Database);
            return services;
        }

        public static IServiceCollection InjectConfigurations(this IServiceCollection services, GeneralConfiguration configuration)
        {
            services.AddSingleton(configuration.Cache);
            return services;
        }

        public static IServiceCollection ConfigureRateLimiter(this IServiceCollection services, GeneralConfiguration configuration)
        {
            services.AddRateLimiter(_ =>
            {
                _.OnRejected = (context, ct) =>
                {
                    if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                    {
                        context.HttpContext.Response.Headers.RetryAfter =
                            ((int)retryAfter.TotalSeconds).ToString(NumberFormatInfo.InvariantInfo);
                    }
                    context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    context.HttpContext.Response.WriteAsync("Too many requests. Try again later.", cancellationToken: ct);
                    return new ValueTask();
                };
                _.AddFixedWindowLimiter(policyName: FIXED_RATE_LIMITING_POLICY, options =>
                {
                    options.PermitLimit = configuration.RateLimiterMaxCalls;
                    options.Window = TimeSpan.FromMinutes(1);
                    options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    options.QueueLimit = 0;
                });
            });
            return services;
        }

        public static IServiceCollection ConfigureApiVersioning(this IServiceCollection services)
        {
            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = VERSION_1;
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
                options.ApiVersionReader = ApiVersionReader.Combine(
                    new HeaderApiVersionReader("X-Version")
                );
            }).AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });
            return services;
        }

        public static IServiceCollection ConfigureTelemetry(this IServiceCollection services, GeneralConfiguration configuration)
        {
            if (configuration.Telemetry.Enabled)
                services.AddOpenTelemetry()
                            .ConfigureResource(resource => resource
                                .AddService(serviceName: APP_NAME, serviceVersion: ASSEMBLY_VERSION))
                            .WithTracing(tracing => tracing
                                .AddAspNetCoreInstrumentation()
                                .AddHttpClientInstrumentation()
                                .AddOtlpExporter(options =>
                                {
                                    options.Endpoint = new Uri(configuration.Telemetry.TracesDestination);
                                    options.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.HttpProtobuf;
                                    options.ExportProcessorType = ExportProcessorType.Simple;
                                }))
                            .WithMetrics(metrics => metrics
                                .AddAspNetCoreInstrumentation()
                                .AddHttpClientInstrumentation()
                                .AddPrometheusExporter());
            return services;
        }
    }
}
