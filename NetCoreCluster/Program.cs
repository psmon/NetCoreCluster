using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Web;
using System;
using System.IO;
using System.Threading.Tasks;

namespace NetCoreCluster
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            // https://wakeupandcode.com/generic-host-builder-in-asp-net-core-3-1/ 

            var host = new HostBuilder()
                .ConfigureHostConfiguration(configHost =>
                {
                    var envName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                    var envFileName = "appsettings.json";
                    if (envName.ToLower() != "production")
                    {
                        envFileName = $"appsettings.{envName}.json";
                        LogManager.LoadConfiguration("NLog.Development.config");
                    }
                    else
                    {
                        LogManager.LoadConfiguration("NLog.config");

                    }

                    configHost.SetBasePath(Directory.GetCurrentDirectory());
                    configHost.AddJsonFile(envFileName, optional: false);
                    configHost.AddCommandLine(args);

                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddLogging();

                    // register our host service
                    services.AddHostedService<StartupService>();

                })
                .ConfigureLogging((hostContext, configLogging) =>
                {
                    configLogging.ClearProviders();
                    //configLogging.AddConsole();

                })
                .UseNLog()
                .UseConsoleLifetime()
                .Build();

            await host.RunAsync();
        }


    }
}
