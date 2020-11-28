using Discord;
using System;
using System.Configuration;
using System.Threading.Tasks;

namespace NecronomiconBot
{
    class Program
    {

        private System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

        static void Main(string[] args)
            => new Program().MainAsync(args).GetAwaiter().GetResult();

        public Task MainAsync(string[] args)
        {
            if (args.Length == 1)
            {
                config.AppSettings.Settings["Token"].Value = args[0];
                config.Save();
            }
            string token = config.AppSettings.Settings["Token"].Value;

            return Task.CompletedTask;
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

    }
}
