using Common;
using Topshelf;

namespace ShardingClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var clientId = ClientId.One;

            HostFactory.Run(x =>
            {
                x.UseLog4Net();
                x.Service<MyClient>(s =>
                {
                    s.ConstructUsing(name => new MyClient(clientId));
                    s.WhenStarted(n => n.Start());
                    s.WhenStopped(n => n.Stop());
                });
                //x.RunAsLocalSystem();
                x.RunAsPrompt();

                var cleintInfo = $"Client {clientId}";
                x.SetDescription(cleintInfo);
                x.SetDisplayName(cleintInfo);
                x.SetServiceName(cleintInfo);

                x.StartAutomaticallyDelayed();
            });
        }
    }
}
