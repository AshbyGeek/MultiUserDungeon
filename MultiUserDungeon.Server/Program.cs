using System;
using System.Net;
using System.Threading;

namespace MultiUserDungeon.Server
{
    class Program
    {
        /// <summary>
        /// Starts a server and waits
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Console.WriteLine("Starting Server...");

            var srv = MuServer.StartServer();

            //TODO: come up with a resonable ending scenario
            while (true)
            {
                Thread.Sleep(100);
            }
        }
    }
}
