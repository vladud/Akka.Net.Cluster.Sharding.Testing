using Akka.Cluster.Sharding;

namespace Common
{
    public sealed class MessageExtractor : HashCodeMessageExtractor
    {
        public MessageExtractor(int maxNumberOfShards) : base(maxNumberOfShards) { }

        public override string EntityId(object message) => (message as ShardEnvelope)?.EntityId;
        public override object EntityMessage(object message) => (message as ShardEnvelope);
    }
}
