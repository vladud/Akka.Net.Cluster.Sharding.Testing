using Common;
using System;
using Topshelf;

namespace ShardingNode
{
    class Program
    {
        static void Main(string[] args)
        {
            var nodeId = NodeId.One;

            HostFactory.Run(x =>
            {
                x.AddCommandLineDefinition("nodeId", (p) => nodeId = GetNodeId(p));
                x.ApplyCommandLine();

                x.UseLog4Net();
                x.Service<MyNode>(s =>
                {
                    s.ConstructUsing(name => new MyNode(nodeId));
                    s.WhenStarted(n => n.Start());
                    s.WhenStopped(n => n.Stop());
                });
                //x.RunAsLocalSystem();
                x.RunAsPrompt();

                var nodeInfo = $"Node {nodeId}";

                x.SetDescription(nodeInfo);
                x.SetDisplayName(nodeInfo);
                x.SetServiceName(nodeInfo);
            });
        }

        private static NodeId GetNodeId(string nodeIdArg)
        {
            int nodeIdNumber;
            if (int.TryParse(nodeIdArg, out nodeIdNumber) && Enum.IsDefined(typeof(NodeId), nodeIdNumber))
            {
                return (NodeId)nodeIdNumber;
            }

            return NodeId.One;
        }
    }
}
