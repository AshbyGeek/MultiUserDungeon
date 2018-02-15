using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MultiUserDungeon.Common
{
    /// <summary>
    /// Interacts with the user through a specific usage of <see cref="System.Console"/>
    /// Splits the Console area into a large server area dn multiple small individual user areas.
    /// </summary>
    public class MuConsole : IMuConsoleOutput, IMuConsoleInput
    {
        /// <summary>
        /// Used to assure that only one thread/method can write to the console at a time
        /// </summary>
        AsyncLock consoleLock = new AsyncLock();

        /// <summary>
        /// The total number of users currently on the server
        /// </summary>
        public int NumUsers
        {
            get => _NumUsers;
            set
            {
                if (_NumUsers != value && value > 0)
                {
                    _NumUsers = value;

                    // Update the User Strings list to be the appropriate size
                    if (_NumUsers > UserStrings.Count)
                    {
                        for (int i = UserStrings.Count; i < _NumUsers; i++)
                        {
                            UserStrings.Add(new StringBuilder());
                        }
                    }
                    else
                    {
                        UserStrings.RemoveRange(UserStrings.Count - 1, _NumUsers - UserStrings.Count);
                    }
                }
            }
        }
        private int _NumUsers;

        public int UserTop => Console.WindowHeight - 1;
        public int UserWidth => Console.WindowWidth / NumUsers;

        public List<StringBuilder> UserStrings { get; } = new List<StringBuilder>();

        /// <summary>
        /// The width that the console window will be set to (the user can still resize it, unfortunately)
        /// </summary>
        public const int CONSOLE_WIDTH = 120;

        /// <summary>
        /// The height that the console window will be set to (the user can still resize it, unfortunately)
        /// </summary>
        public const int CONSOLE_HEIGHT = 50;
        public MuConsole()
        {
            Console.SetBufferSize(CONSOLE_WIDTH, CONSOLE_HEIGHT);
            Console.SetWindowSize(CONSOLE_WIDTH, CONSOLE_HEIGHT);
            NumUsers = 1;
        }

        /// <summary>
        /// Returns the leftmost column of the specified user's display area
        /// </summary>
        /// <param name="userNum">the index of the user</param>
        /// <returns></returns>
        public int GetUserLeft(int userNum)
        {
            return UserWidth * (userNum);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="numUsers"></param>
        public MuConsole(int numUsers = 1)
        {
            NumUsers = numUsers;
        }

        #region IConsoleOutput

        public async Task UserTyped(char c, int user)
        {
            var str = UserStrings[user];
            if (str.Length < UserWidth)
            {
                str.Append(c);
                
            }
            await Write(str.ToString(), GetUserLeft(user), UserTop, UserWidth);
        }

        public async Task ClearUserLine(int user)
        {
            UserStrings[user].Clear();
            await Write("", GetUserLeft(user), UserTop, UserWidth);
        }

        public async Task ServerSays(string lines)
        {
            await Write(lines, 0, UserTop-1);
        }

        public async Task ServerSays(string text, int row, int column)
        {
            await Write(text, row, column);
        }

        public async Task ClearServer()
        {
            using (await consoleLock.LockAsync())
            {
                Console.Clear();
                Console.SetCursorPosition(GetUserLeft(0), UserTop);
                Console.Out.Flush();
            }

            for (int i = 0; i < NumUsers; i++)
            {
                var str = UserStrings[i].ToString();
                await Write(str, GetUserLeft(i), UserTop, UserWidth);
            }
        }

        private async Task Write(string str, int left, int top, int width = int.MaxValue)
        {
            if (str.Length > width)
            {
                str = str.Substring(str.Length - width, width);
            }
            else if (width < int.MaxValue)
            {
                str = str.PadRight(width);
            }

            using (await consoleLock.LockAsync())
            {
                Console.SetCursorPosition(left, top);
                Console.Write(str);
                Console.SetCursorPosition(GetUserLeft(0), UserTop);
                Console.Out.Flush();
            }
        }

        #endregion

        #region IConsoleInput

        private string charBuffer = "";

        public int ReadChar(bool clearBuffer = false)
        {
            int c = (char)Console.Read();

            if (c != -1)
            {
                if (!clearBuffer)
                {
                    charBuffer += Convert.ToChar(c);
                }
                else
                {
                    charBuffer = "";
                }
            }
            return c;
        }

        public string ReadLine()
        {
            var str = charBuffer + Console.ReadLine();
            charBuffer = "";
            return str;
        }
        
        #endregion
    }
}
