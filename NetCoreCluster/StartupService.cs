using Akka.Actor;
using Akka.Routing;
using AkkaDotModule.Config;
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
            var roles = Environment.GetEnvironmentVariable("CLUSTER_ROLES");
            var envName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            SystemNameForCluster = Environment.GetEnvironmentVariable("ACTORSYSTEM");            

            var akkaConfig = AkkaLoadEx.Load(envName);
            var actorSystem = ActorSystem.Create(SystemNameForCluster, akkaConfig);
            
            AkkaLoadEx.RegisterActor(
                "ClusterWorkerPoolActor",
                actorSystem.ActorOf(Props.Create<WorkActor>()
                        .WithRouter(FromConfig.Instance),
                        "cluster-workerpool"
            ));



            if (roles.Contains("AdminNode"))
            {
                string singleToneRole = "AdminNode";
                actorSystem.BootstrapSingleton<SingleToneActor>("SingleToneActor", singleToneRole);
                var singleToneActor = actorSystem.BootstrapSingletonProxy("SingleToneActor", singleToneRole, "/user/SingleToneActor", "singleToneActorProxy");
                AkkaLoadEx.RegisterActor("SingleToneActor", singleToneActor);

                actorSystem
                    .Scheduler
                    .ScheduleTellRepeatedly(TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(1),
                    singleToneActor, "이벤트 발생..", ActorRefs.NoSender);
            }

            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
        }
    }
}
