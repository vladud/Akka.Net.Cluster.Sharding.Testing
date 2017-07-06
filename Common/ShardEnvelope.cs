namespace Common
{
    public sealed class ShardEnvelope
    {
        public readonly string EntityId;
        public readonly string Message;
        public NodeId FromNodeId { get; set; } = NodeId.Unknown;
        public ClientId FromClientId { get; set; } = ClientId.Unknown;

        public ShardEnvelope(string entityId, string message)
        {
            EntityId = entityId;
            Message = message;
        }
    }
}
