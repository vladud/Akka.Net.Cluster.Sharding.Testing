using Akka.Actor;
using log4net;
using Common;

namespace ShardingActor
{
    public class MyActor : ReceiveActor, ILogReceive
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private const string ResponseMessage = "Who's there?";

        public MyActor(NodeId nodeId)
        {
            Receive<ShardEnvelope>(se =>
                                   {
                                       Logger.Info($"Received message with EntityId: {se.EntityId}, from ClientId: {se.FromClientId}, with message: {se.Message}");
                                       Sender.Tell(new ShardEnvelope(se.EntityId, ResponseMessage) { FromNodeId = nodeId });
                                   });

            Receive<object>(o => Logger.Warn($"I can't handle this! {o.GetType()}"));
        }
    }
}
