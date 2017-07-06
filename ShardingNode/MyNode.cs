using Akka.Actor;
using Akka.Cluster.Sharding;
using Akka.Cluster.Tools.Singleton;
using Akka.Configuration;
using Common;
using log4net;
using ShardingActor;
using System;
using System.Collections.Generic;
using Akka.Cluster.Tools.Client;

namespace ShardingNode
{
    class MyNode
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly Dictionary<NodeId, string> NodePorts = new Dictionary<NodeId, string>
        {
            {NodeId.One, "5001" },
            {NodeId.Two, "5002" },
            {NodeId.Three, "5003" }
        };

        private readonly Config config;

        private IActorRef shardRegion;

        public MyNode(NodeId nodeId)
        {
            /*
            config = ConfigurationFactory.ParseString($@"
                akka.remote.dot-netty.tcp {{
                    port = {NodePorts[nodeId]}
                    hostname = 127.0.0.1
                    log-transport = false
                    }}
                }}");
            */

            config = ConfigurationFactory.ParseString($@"
                akka.remote.helios.tcp {{
                    hostname = ""127.0.0.1""
                    port = {NodePorts[nodeId]}
                }}");
        }

        public void Start()
        {
            Logger.Info("Start");

            using (var system = ActorSystem.Create(Constants.ActorSystemName, ConfigurationFactory.Load()
                //.WithFallback(config)
                .WithFallback(null)
                .WithFallback(ClusterSingletonManager.DefaultConfig())))
            {
                var sharding = ClusterSharding.Get(system);
                shardRegion = sharding.Start(
                    typeName: nameof(MyActor),
                    entityProps: Props.Create<MyActor>(), // the Props used to create entities
                    settings: ClusterShardingSettings.Create(system),
                    messageExtractor: new MessageExtractor(Constants.MaxNumberOfNodes * 10)
                );
                ClusterClientReceptionist.Get(system).RegisterService(shardRegion);
            }
        }

        public void Stop()
        {
            Logger.Info("Stop");
            shardRegion?.GracefulStop(TimeSpan.FromSeconds(5)).Wait();
        }
    }
}
