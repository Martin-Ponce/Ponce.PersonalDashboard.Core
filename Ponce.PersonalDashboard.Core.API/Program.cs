using Microsoft.EntityFrameworkCore;
using Ponce.PersonalDashboard.Core.API.Extensions;
using Ponce.PersonalDashboard.Core.API.Extensions.Configuration;
using Ponce.PersonalDashboard.Core.API.Extensions.Endpoints;
using Ponce.PersonalDashboard.Core.API.Middlewares;
using Ponce.PersonalDashboard.Core.Infrastructure.Contexts;
using Serilog;
using static Ponce.PersonalDashboard.Core.API.Helpers.ApiConstants;

namespace Ponce.PersonalDashboard.Core.API
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            try
            {
                var builder = WebApplication.CreateBuilder(args);
                var generalConfiguration = builder.Configuration.GetConfiguration();
                builder.Services
                    .ConfigureApiVersioning()
                    .ConfigureJson()
                    .ConfigureSwagger()
                    .ConfigureCors(generalConfiguration)
                    .ConfigureAuthentication(generalConfiguration, builder.Environment)
                    .ConfigureLogger(generalConfiguration)
                    .ConfigureServices(generalConfiguration)
                    .ConfigureContexts(generalConfiguration)
                    .ConfigureHealthChecks()
                    .InjectConfigurations(generalConfiguration)
                    .ConfigureRateLimiter(generalConfiguration)
                    .ConfigureTelemetry(generalConfiguration);
                if (generalConfiguration.Cache.CacheEnabled)
                    builder.Services.AddMemoryCache();

                var app = builder.Build();
                app.UseCors(ALLOW_ALL_CORS_POLICY)
                    .UseRateLimiter()
                    .UseMiddleware<ErrorHandlerMiddleware>()
                    .UseAuthentication()
                    .UseAuthorization()
                    .UseMiddleware<UserSynchronizationMiddleware>();

                if (generalConfiguration.Telemetry.Enabled)
                    app.UseOpenTelemetryPrometheusScrapingEndpoint();
                if (app.Environment.IsDevelopment())
                    app.UseVersionedSwagger();

                app.AddHealthChecks(FIXED_RATE_LIMITING_POLICY)
                    .MapHome(FIXED_RATE_LIMITING_POLICY);
                app.MapEndpoints(FIXED_RATE_LIMITING_POLICY);

                await EnsureDatabaseCreated(app.Services);
                await app.RunAsync();
            }
            catch (Exception ex)
            {
                await ManageException(ex);
            }
        }

        private static async Task ManageException(Exception exception)
        {
            Log.Logger = new LoggerConfiguration()
                                .WriteTo.Console()
                                .WriteTo.File("Logs/app.log")
                                .Destructure.ToMaximumDepth(3)
                                .CreateLogger();
            Log.Logger.Fatal(exception, "Error while configuring the application");
            await Task.Delay(TimeSpan.FromSeconds(5));
            Environment.Exit(1);
        }

        private static async Task EnsureDatabaseCreated(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await context.Database.EnsureCreatedAsync();
        }
    }
}
