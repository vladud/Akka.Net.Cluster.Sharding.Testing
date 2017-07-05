using Akka.Actor;
using Akka.Cluster.Tools.Client;
using Common;
using System;

namespace ShardingClient
{
    class MySender
    {
        bool shouldStop;

        public void Start()
        {
            using (var system = ActorSystem.Create(Constants.ActorSystemName))
            {
                system.Settings.InjectTopLevelFallback(ClusterClientReceptionist.DefaultConfig());
                var settings = ClusterClientSettings.Create(system);
                var client = system.ActorOf(ClusterClient.Props(settings));

                while(!shouldStop)
                {
                    Console.WriteLine("Press key to send Hi");
                    Console.ReadLine();
                    client.Tell(new ClusterClient.Send("/user/MyActor", new ShardEnvelope("1", "Hi!")));
                }
            }
        }

        public void Stop()
        {

        }
    }
}
