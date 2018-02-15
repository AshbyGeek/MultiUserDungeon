using MultiUserDungeon.Common;
using MultiUserDungeon.Server;
using System;
using System.Diagnostics;

namespace MultiUserDungeon.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            // Show the launch pages
            var console = new MuConsole();
            var launcher = new Launcher(console, console);
            var task = launcher.ShowAll();
            task.Wait();

            // Start the server if desired
            MuServer srv = null;
            if (launcher.CreateServer)
            {
                srv = MuServer.StartServer();
            }
            
            // TODO: connect to an existing server and start playing
        }
    }
}
