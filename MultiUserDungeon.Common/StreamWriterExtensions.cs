using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MultiUserDungeon.Common
{
    public static class StreamWriterExtensions
    {
        /// <summary>
        /// Causes the writer to write to the end of its stream and return the Position pointer to
        /// its original position.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        public static void WriteAndRewind(this StreamWriter writer, string value)
        {
            var position = writer.BaseStream.Position;
            if (writer.BaseStream.CanSeek && !(writer.BaseStream is MemoryStreamThreadSafeSeek))
            {
                writer.BaseStream.Position = writer.BaseStream.Length;
            }

            writer.Write(value);
            writer.Flush();
            if (writer.BaseStream.CanSeek && !(writer.BaseStream is MemoryStreamThreadSafeSeek))
            {
                writer.BaseStream.Position = position;
            }
        }

        /// <summary>
        /// Causes the writer to write to the end of its stream asynchronously and return the Position pointer to
        /// its original position.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static async Task WriteAndRewindAsync(this StreamWriter writer, string value)
        {
            var position = writer.BaseStream.Position;
            if (writer.BaseStream.CanSeek && !(writer.BaseStream is MemoryStreamThreadSafeSeek))
            {
                writer.BaseStream.Position = writer.BaseStream.Length;
            }

            await writer.WriteAsync(value);
            await writer.FlushAsync();
            if (writer.BaseStream.CanSeek && !(writer.BaseStream is MemoryStreamThreadSafeSeek))
            {
                writer.BaseStream.Position = position;
            }
        }
    }
}
