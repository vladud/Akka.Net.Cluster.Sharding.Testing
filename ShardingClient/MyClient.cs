using Akka.Actor;
using Akka.Cluster.Tools.Client;
using Common;
using System;
using System.Threading.Tasks;
using Akka.Cluster.Tools.Singleton;
using Akka.Configuration;
using log4net;

namespace ShardingClient
{
    class MyClient
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        bool shouldStop;

        private string clientName;
        private string envelopeId;

        private const string HiMessage = "Hi!";

        public MyClient(ClientId clientId)
        {
            clientName = $"Client_{clientId}";
            envelopeId = ((int)clientId).ToString();
        }

        public void Start()
        {
            using (var system = ActorSystem.Create(Constants.ActorSystemName, ConfigurationFactory.Load().WithFallback(ClusterSingletonManager.DefaultConfig())))
            {
                system.Settings.InjectTopLevelFallback(ClusterClientReceptionist.DefaultConfig());
                var settings = ClusterClientSettings.Create(system);
                var client = system.ActorOf(ClusterClient.Props(settings), clientName);

                while(!shouldStop)
                {
                    Console.WriteLine("Press key to send Hi");
                    Console.ReadKey();
                    client.Ask<ShardEnvelope>(new ClusterClient.Send("/user/sharding/MyActor", new ShardEnvelope(envelopeId, HiMessage)), TimeSpan.FromSeconds(10))
                        .ContinueWith(se =>
                                      {
                                          if (se.Status == TaskStatus.Canceled)
                                          {
                                              Logger.Warn("Did not receive response");
                                          }
                                          Logger.Info($"Received response with EntityId: {se.Result.EntityId}, Payload: {se.Result.Payload}");
                                      });
                }
            }
        }

        public void Stop()
        {
            shouldStop = true;
        }
    }
}
