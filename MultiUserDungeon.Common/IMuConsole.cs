using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MultiUserDungeon.Common
{
    /// <summary>
    /// Describes how a client can output messages to the user
    /// </summary>
    public interface IMuConsoleOutput
    {
        /// <summary>
        /// writes a single character to a specific user's display area
        /// </summary>
        /// <param name="c">the character to write</param>
        /// <param name="user">the index of the user</param>
        /// <returns></returns>
        Task UserTyped(char c, int user);

        /// <summary>
        /// clears the content's of a specific user's display area
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task ClearUserLine(int user);

        /// <summary>
        /// Writes a message to the server's display area
        /// </summary>
        /// <param name="lines">one or more lines of text to display</param>
        /// <returns></returns>
        Task ServerSays(string lines);

        /// <summary>
        /// Writes a message to a specific location in the server's display area
        /// </summary>
        /// <param name="text">The message to write</param>
        /// <param name="row">the row to start at (0 is the top row)</param>
        /// <param name="column">the column to start at (0 is the left column)</param>
        /// <returns></returns>
        Task ServerSays(string text, int row, int column);

        /// <summary>
        /// Clear everything from the server's display area
        /// </summary>
        /// <returns></returns>
        Task ClearServer();
    }

    /// <summary>
    /// Defines how input can be queried from the user
    /// </summary>
    public interface IMuConsoleInput
    {
        /// <summary>
        /// Reads a single character from the user
        /// </summary>
        /// <param name="clearBuffer">If true, the user's line will be cleared after reading
        ///    this character. Otherwise it remains and can be read with ReadLine later.</param>
        /// <returns></returns>
        int ReadChar(bool clearBuffer = false);

        /// <summary>
        /// Reads an entire line from the user, ending when the character types a return character.
        /// </summary>
        /// <returns></returns>
        string ReadLine();
    }
}
