using Akka.Actor;
using Akka.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using NetCoreCluster.Config;
using NetCoreCluster.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace NetCoreCluster
{
    public sealed class StartupService : IHostedService
    {
        public IConfiguration Configuration { get; }

        public string SystemNameForCluster = "NetCoreCluster";

        public StartupService(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {

            var envName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            SystemNameForCluster = Environment.GetEnvironmentVariable("ACTORSYSTEM");            

            var akkaConfig = AkkaLoadEx.Load(envName);
            var actorSystem = ActorSystem.Create(SystemNameForCluster, akkaConfig);
            
            var worker = AkkaLoadEx.RegisterActor(
                "ClusterWorkerPoolActor",
                actorSystem.ActorOf(Props.Create<WorkActor>()
                        .WithRouter(FromConfig.Instance),
                        "cluster-workerpool"
            ));

            for(int i=0;i<10;i++) worker.Tell($"TestMessage{i}");


            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
        }
    }
}
