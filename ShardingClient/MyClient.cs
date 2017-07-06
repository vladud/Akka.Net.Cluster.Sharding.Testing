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

        private readonly ClientId clientId;
        private string clientName;
        private string envelopeId;

        private const string SendMessage = "Knock, knock!";

        public MyClient(ClientId clientId)
        {
            this.clientId = clientId;
            clientName = $"Client_{clientId}";
            envelopeId = ((int)clientId).ToString();
        }

        public void Start()
        {
            var system = ActorSystem.Create(Constants.ActorSystemName, ConfigurationFactory.Load().WithFallback(ClusterSingletonManager.DefaultConfig()));
            system.Settings.InjectTopLevelFallback(ClusterClientReceptionist.DefaultConfig());
            var settings = ClusterClientSettings.Create(system);
            var client = system.ActorOf(ClusterClient.Props(settings), clientName);

            while (!shouldStop)
            {
                Console.WriteLine("Press key to send message");
                Console.ReadKey();
                client.Ask<ShardEnvelope>(new ClusterClient.Send("/user/sharding/MyActor", new ShardEnvelope(envelopeId, SendMessage) { FromClientId = clientId }), TimeSpan.FromSeconds(10))
                    .ContinueWith(se =>
                                  {
                                      if (se.Status == TaskStatus.Canceled)
                                      {
                                          Logger.Warn("He ignored me:(");
                                      }
                                      else
                                      {
                                          Logger.Info($"Received response with EntityId: {se.Result.EntityId}, Message: {se.Result.Message}, from NodeId: {se.Result.FromNodeId}");
                                      }
                                  });
            }
        }

        public void Stop()
        {
            shouldStop = true;
        }
    }
}
