using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SchedulerJob.Data;
using SchedulerJob.Helper;
using SchedulerJob.Jobs;
using SchedulerJob.Model.Configuration;
using SchedulerJob.Services;
using SchedulerJob.Services.Interfaces;
using Serilog;

namespace SchedulerJob
{
    public class Program
    {
        public static IConfiguration? Configuration { get; set; }

        public static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console(new Serilog.Formatting.Json.JsonFormatter())
                .CreateLogger();

            try
            {
                Log.Information("Executing scheduler Jobs");
                var host = CreateHostBuilder(args).Build();
                var jobRunner = host.Services.GetRequiredService<ISchedulerServices>();
                await jobRunner.ProcessCronJobsAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed executing scheduler jobs");
            }
            finally
            {
                Log.Information("Closing");
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureAppConfiguration((context, config) =>
                {
                    ConfigurationHelper.Initialize(context.HostingEnvironment);
                    //if (!ConfigurationHelper.IsLocalEnvironment())
                    //    config.AddVault(config.Build().GetSection(ApplicationConstants.VaultConfig).Get<VaultConfig>());
                    Configuration = config.Build();
                })
                .ConfigureServices((context, services) =>
                {
                    var connectionStrings = Configuration != null
                        ? ConfigurationHelper.GetSection<ConnectionStrings>(Configuration, "ConnectionStrings")
                        : throw new InvalidOperationException("Configuration has not been initialized.");

                    services.AddDbContext<EswDbContext>(options => options.UseSqlServer(connectionStrings.ESWDb));
                    services.AddTransient<Services.Interfaces.IApplicationLifetime, ApplicationLifetime>();

                    // Register the job dictionary for extensibility
                    services.AddSingleton<IDictionary<string, IJobs>>(provider =>
                    {
                        var config = provider.GetRequiredService<IConfiguration>();
                        var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
                        return SchedulerJob.Services.JobRegistry.CreateJobDictionary(config, loggerFactory);
                    });

                    services.AddTransient<ISchedulerServices, SchedulerServices>();
                });
    }
}
