using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MultiUserDungeon.Common;

namespace MultiUserDungeon.Client
{
    /// <summary>
    /// Represents the launch/configuration screens for the game before connecting
    /// to a server and commencing actual gameplay
    /// </summary>
    public class Launcher
    {
        public enum PageTitle
        {
            Welcome,
            UserName,
            CreateServer,
            IPAddress,
        }

        public IMuConsoleOutput ConsoleOutput { get; }
        public IMuConsoleInput ConsoleInput { get; }

        public string UserName { get; set; }
        public IPAddress ServerAddress { get; set; } = IPAddress.None;
        public bool CreateServer { get; set; }
        
        public Launcher(IMuConsoleOutput consoleOuput, IMuConsoleInput consoleInput)
        {
            ConsoleOutput = consoleOuput;
            ConsoleInput = consoleInput;
        }

        /// <summary>
        /// Shows all launch screen pages in order
        /// </summary>
        /// <returns></returns>
        public async Task ShowAll()
        {
            await Show(PageTitle.Welcome);
            await Show(PageTitle.UserName);
            await Show(PageTitle.CreateServer);
            if (!CreateServer)
            {
                await Show(PageTitle.IPAddress);
            }
        }

        /// <summary>
        /// Shows a specific launch screen page
        /// </summary>
        /// <param name="page">The page to show</param>
        /// <returns></returns>
        public async Task Show(PageTitle page)
        {
            //await ConsoleOutput.ClearServer();
            switch (page)
            {
                case PageTitle.Welcome:
                    await ConsoleOutput.ServerSays("Welcome to the dungeon!\n" +
                                                   "-----------------------\n" +
                                                   "\n" +
                                                   "Press any enter to continue...\n"
                    );
                    var response = ConsoleInput.ReadChar(true);
                    break;
                case PageTitle.UserName:
                    await ConsoleOutput.ServerSays("What is your name?");
                    UserName = ConsoleInput.ReadLine();
                    break;
                case PageTitle.CreateServer:
                    await ConsoleOutput.ServerSays("Would you like to spawn a server? (y/n)");
                    while (true)
                    {
                        string createServerStr = ConsoleInput.ReadLine();
                        if (createServerStr.ToLower().Contains("y"))
                        {
                            CreateServer = true;
                            ServerAddress = IPAddress.Loopback;
                            break;
                        }
                        else if (createServerStr.ToLower().Contains("n"))
                        {
                            CreateServer = false;
                            break;
                        }
                        else
                        {
                            await ConsoleOutput.ServerSays("I'm not sure what you meant. Try 'yes' or 'no'");
                        }
                    }
                    break;
                case PageTitle.IPAddress:
                    await ConsoleOutput.ServerSays("What's the IP Address of the server you want to connect to? [localhost]");
                    var ipStr = ConsoleInput.ReadLine();
                    if (ipStr.Trim() == "")
                    {
                        ServerAddress = IPAddress.Loopback;
                    }
                    else
                    {
                        ServerAddress = IPAddress.Parse(ipStr);
                    }
                    break;
            }
        }
    }
}
