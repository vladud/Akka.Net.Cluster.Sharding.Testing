using Akka.Actor;
using log4net;
using Common;

namespace ShardingActor
{
    public class MyActor : ReceiveActor, ILogReceive
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public MyActor()
        {
            Receive<ShardEnvelope>(se =>
                                   {
                                       Logger.Info($"Received message with EntityId: {se.EntityId}, Payload: {se.Payload}");
                                       Sender.Tell(se);
                                   });
        }
    }
}
