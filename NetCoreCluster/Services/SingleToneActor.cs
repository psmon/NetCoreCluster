using Akka.Actor;
using Akka.Event;
using NetCoreCluster.Config;

namespace NetCoreCluster.Services
{
    public class SingleToneActor : ReceiveActor
    {
        private readonly ILoggingAdapter logger = Context.GetLogger();

        public SingleToneActor()
        {
            logger.Debug("Create SingleToneActor");

            ReceiveAsync<string>(async msg =>
            {
                logger.Debug($"SingleToneActor:{msg}");

                AkkaLoadEx.ActorSelect("ClusterWorkerPoolActor").Tell("Task");

            });
        }
    }
}
