using Akka.Actor;
using Akka.Cluster;
using Akka.Event;

namespace NetCoreCluster.Services
{
    public class WorkActor : ReceiveActor
    {
        private readonly ILoggingAdapter logger = Context.GetLogger();

        protected Cluster Cluster = Akka.Cluster.Cluster.Get(Context.System);

        public WorkActor()
        {
            ReceiveAsync<string>(async msg =>
            {
                logger.Info($"{msg} 를 전송받았습니다.");
            });
        }

        protected override void PreStart()
        {
            Cluster.Subscribe(Self, ClusterEvent.InitialStateAsEvents, new[] { typeof(ClusterEvent.IMemberEvent), typeof(ClusterEvent.UnreachableMember) });
        }

        protected override void PostStop()
        {
            Cluster.Unsubscribe(Self);
        }
    }
}
