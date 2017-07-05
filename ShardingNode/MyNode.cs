using Akka.Actor;
using Akka.Cluster.Sharding;
using Akka.Cluster.Tools.Singleton;
using Akka.Configuration;
using Common;
using log4net;
using ShardingActor;
using System;
using System.Collections.Generic;

namespace ShardingNode
{
    class MyNode
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static Dictionary<NodeId, string> NodePorts = new Dictionary<NodeId, string>
        {
            {NodeId.One, "5001" },
            {NodeId.Two, "5002" },
            {NodeId.Three, "5003" },
        };

        private Config config;

        private IActorRef shardRegion;

        public MyNode(NodeId nodeId)
        {
            config = ConfigurationFactory.ParseString($@"
                akka.remote.helios.tcp {{
                    transport-class = ""Akka.Remote.Transport.Helios.HeliosTcpTransport, Akka.Remote""
                    transport-protocol = tcp
                    hostname = ""127.0.0.1""
                    port = {NodePorts[nodeId]}
                }}");
        }

        public void Start()
        {
            Logger.Info("Start");

            using (var system = ActorSystem.Create(Constants.ActorSystemName, ConfigurationFactory.Load().WithFallback(config).WithFallback(ClusterSingletonManager.DefaultConfig())))
            {
                var sharding = ClusterSharding.Get(system);
                var shardRegion = sharding.Start(
                    typeName: nameof(MyActor),
                    entityProps: Props.Create<MyActor>(), // the Props used to create entities
                    settings: ClusterShardingSettings.Create(system),
                    messageExtractor: new MessageExtractor(Constants.MaxNumberOfNodes * 10)
                );
            }
        }

        public void Stop()
        {
            Logger.Info("Stop");
            if (shardRegion != null) shardRegion.GracefulStop(TimeSpan.FromSeconds(5)).Wait();
        }
    }
}
