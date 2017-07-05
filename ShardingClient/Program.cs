using Topshelf;

namespace ShardingClient
{
    class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(x =>
            {
                x.UseLog4Net();
                x.Service<MySender>(s =>
                {
                    s.ConstructUsing(name => new MySender());
                    s.WhenStarted(n => n.Start());
                    s.WhenStopped(n => n.Stop());
                });
                x.RunAsLocalSystem();

                x.SetDescription("Sender");
                x.SetDisplayName("Sender");
                x.SetServiceName("Sender");
            });
        }
    }
}
