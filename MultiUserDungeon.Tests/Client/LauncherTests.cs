using Microsoft.VisualStudio.TestTools.UnitTesting;

using MultiUserDungeon.Common;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;

namespace MultiUserDungeon.Client.Tests
{
    public class FakeConsole : IMuConsoleOutput
    {
        public StringBuilder CurrentServerContent { get; set; } = new StringBuilder();
        public List<StringBuilder> UserStrings { get; } = new List<StringBuilder>();

        public async Task ClearServer()
        {
            CurrentServerContent.Clear();
        }

        public async Task ClearUserLine(int user)
        {
            if (UserStrings.Count >= user)
            {
                UserStrings[user].Clear();
            }
        }

        public async Task ServerSays(string lines)
        {
            CurrentServerContent.Append(lines);
        }

        public async Task ServerSays(string text, int row, int column)
        {
            CurrentServerContent.Append(text);
        }

        public async Task UserTyped(char c, int user)
        {
            if (UserStrings.Count < user)
            {
                for (int i = UserStrings.Count - 1; i < user; i++)
                {
                    UserStrings.Add(new StringBuilder());
                }
                UserStrings[user].Append(c);
            }
        }
    }

    public class FakeUser : IMuConsoleInput
    {
        public string UserInput { get; set; }
        public string[] UserInputLines => UserInput.TrimEnd().Split("\n").Select(x => x.Trim()).ToArray();
        public int NumCharsRead;
        public int NumLinesRead;
        
        public int ReadChar(bool clearBuffer = false)
        {
            if (NumCharsRead < UserInput.Length)
            {
                char c = UserInput[NumCharsRead++];
                if (clearBuffer)
                {
                    UserInput = UserInput.Substring(NumCharsRead);
                    NumCharsRead = 0;
                }
                return c;
            }
            else
            {
                return (char)0;
            }
        }

        public string ReadLine()
        {
            if (NumLinesRead < UserInputLines.Length)
            {
                return UserInputLines[NumLinesRead++];
            }
            return "";
        }
    }


    [TestClass]
    public class LauncherTests
    {
        [TestMethod]
        public void LauncherWelcomeTest()
        {
            var console = new FakeConsole();
             var user = new FakeUser()
            {
                UserInput = "\n"
            };

            var launch = new Launcher(console, user);
            launch.Show(Launcher.PageTitle.Welcome);

            Assert.IsTrue(console.CurrentServerContent.Length > 0);
        }

        [TestMethod]
        public void LauncherIPTest()
        {
            const string IP = "192.168.1.12";
            var console = new FakeConsole();
            var user = new FakeUser()
            {
                UserInput = IP + "\n",
            };

            var launch = new Launcher(console, user);
            launch.Show(Launcher.PageTitle.IPAddress);

            Assert.AreEqual(IPAddress.Parse(IP), launch.ServerAddress);
        }

        [TestMethod]
        public void LauncherNameTest()
        {
            const string NAME = "Bob";

            var console = new FakeConsole();
            var user = new FakeUser()
            {
                UserInput = NAME + "\n",
            };

            var launch = new Launcher(console, user);
            launch.Show(Launcher.PageTitle.UserName);

            Assert.AreEqual(NAME, launch.UserName);
        }

        [TestMethod]
        public void LauncherCreateServerTest()
        {
            const string ANSWER = "Yes";

            var console = new FakeConsole();
            var user = new FakeUser()
            {
                UserInput = ANSWER + "\n",
            };

            var launch = new Launcher(console, user);
            launch.Show(Launcher.PageTitle.CreateServer);

            Assert.IsTrue(launch.CreateServer);
        }

        [TestMethod]
        public void LauncherDontCreateServerTest()
        {
            const string ANSWER = "No";

            var console = new FakeConsole();
            var user = new FakeUser()
            {
                UserInput = ANSWER + "\n",
            };

            var launch = new Launcher(console, user);
            launch.CreateServer = true;
            launch.Show(Launcher.PageTitle.CreateServer);

            Assert.IsFalse(launch.CreateServer);
        }

        [TestMethod]
        public void LauncherShowAllTest()
        {
            const string NAME = "Bob";
            const string ANSWER = "No";
            const string IP = "192.168.1.12";

            var console = new FakeConsole();
            var user = new FakeUser()
            {
                UserInput = "a" +
                            NAME + "\n" +
                            ANSWER + "\n" +
                            IP + "\n",
            };

            var launch = new Launcher(console, user);
            launch.ShowAll();

            Assert.AreEqual(NAME, launch.UserName);
            Assert.IsFalse(launch.CreateServer);
            Assert.AreEqual(IP, launch.ServerAddress.ToString());
        }
    }
}
