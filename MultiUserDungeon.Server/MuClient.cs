using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MultiUserDungeon.Common;
using MultiUserDungeon.Common.NetMsgs;

namespace MultiUserDungeon.Server
{
    public class MsgEventArgs : EventArgs
    {
        public NetworkMsg Msg { get; }
        public MsgEventArgs(NetworkMsg msg) => Msg = msg;
    }

    public class DisconnectedEventArgs : EventArgs
    {
        public bool ForInnactivity { get; }
        public DisconnectedEventArgs(bool forInnactivity) => ForInnactivity = forInnactivity;
    }

    /// <summary>
    /// Defines the interactive portions of a MuClient
    /// </summary>
    public interface IMuClient : IDisposable
    {
        event EventHandler<DisconnectedEventArgs> Disconnected;
        event EventHandler<MsgEventArgs> MsgReceived;

        int UserIndex { get; }

        void SendMessage(NetworkMsg msg);
    }

    /// <summary>
    /// Represents a client in the Server's structure.
    /// Can receive and send messages to a REAL client, once said client is implemented.
    /// Constructed with a stream, usually a NetworkStream from a socket.
    /// </summary>
    public class MuClient : IMuClient
    {
        public event EventHandler<DisconnectedEventArgs> Disconnected;
        public event EventHandler<MsgEventArgs> MsgReceived;

        /// <summary>
        /// The current user message
        /// </summary>
        public string CurUserLine { get; private set; }

        /// <summary>
        /// The user's index for identification purposes
        /// </summary>
        public int UserIndex { get; set; } = -1;
        
        private Stream _stream { get; }
        private StreamWriter _streamWriter { get; }
        private bool _disposed = false;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="stream">A stream to receive/send information on. Usually a NetworkStream</param>
        public MuClient(Stream stream)
        {
            if (!stream.CanRead || !stream.CanWrite)
            {
                throw new ArgumentException("Unsuitable stream. The provided stream must be capable of both reading and writing.", nameof(stream));
            }

            _stream = stream;
            _streamWriter = new StreamWriter(_stream);

            Task.Factory.StartNew(ReceiveMsgs);
        }

        /// <summary>
        /// Runs continuously to receive and process messages from the network stream
        /// </summary>
        private void ReceiveMsgs()
        {
            const int MAX_READ = 1024;
            StringBuilder netMsgWIP = new StringBuilder();
            byte[] buffer = new byte[MAX_READ];
            while (!_disposed)
            {
                // Try to read data from the stream
                int numRead = _stream.Read(buffer, 0, MAX_READ);
                if (numRead > 0)
                {
                    var str = Encoding.UTF8.GetString(buffer, 0, numRead).ToCharArray();
                    netMsgWIP.Append(str);
                    ProcessMessages(netMsgWIP);
                }

                // If we weren't able to read a full buffer then we'll
                // wait for a bit before reading some more
                if (numRead < MAX_READ)
                {
                    Thread.Sleep(20);
                }

                //TODO: implement a timeout counter of some sort to detect a crashed or otherwise
                //      AFK user
            }
        }

        /// <summary>
        /// Looks for messages delimeted with a record separator and LineFeed character
        /// </summary>
        /// <param name="curMsg"></param>
        private void ProcessMessages(StringBuilder curMsg)
        {
            while (true)
            {
                // Find a line feed character indicating the end of a record
                var rsIndex = curMsg.ToString().IndexOf(NetworkMsg.LINE_FEED);
                if (curMsg.Length > 0 && rsIndex >= 0)
                {
                    var msg = curMsg.ToString().Substring(0, rsIndex);
                    ProcessMessage(msg);
                    curMsg.Remove(0, rsIndex + 1);
                }
                else
                {
                    // No messages found, exit the search loop
                    break;
                }
            }
        }

        /// <summary>
        /// Deserializes a string version of an arbitrary NetworkMessage and send a MsgReceived event
        /// </summary>
        /// <param name="message"></param>
        private void ProcessMessage(string message)
        {
            NetworkMsg msg = null;
            try
            {
                msg = NetworkMsg.Deserialize(message);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to deserialize message: {ex.Message}");
            }

            if (msg != null)
            {
                OnMsgReceived(msg);
            }
        }
        

        /// <summary>
        /// Sends the given network message to the client on the other end of the network
        /// </summary>
        /// <param name="msg"></param>
        public void SendMessage(NetworkMsg msg)
        {
            _streamWriter.WriteAndRewind(msg.Serialize());
        }

        /// <summary>
        /// Sends the given network message to the client on the other end of the network asynchronously
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public async Task SendMessageAsync(NetworkMsg msg)
        {
            await _streamWriter.WriteAndRewindAsync(msg.Serialize());
        }

        /// <summary>
        /// Sends a "Disconnected" event
        /// </summary>
        /// <param name="forInnactivity">true to indicate that the user timed out</param>
        protected void OnDisconnected(bool forInnactivity)
        {
            Disconnected?.Invoke(this, new DisconnectedEventArgs(forInnactivity));
        }

        /// <summary>
        /// Sends a MsgReceived event
        /// </summary>
        /// <param name="msg"></param>
        protected void OnMsgReceived(NetworkMsg msg)
        {
            if (msg is ClientDisconnectMsg)
            {
                OnDisconnected(false);
            }
            else
            {
                MsgReceived?.Invoke(this, new MsgEventArgs(msg));
            }
        }

        /// <summary>
        /// Disposes of the associated streamwriter and stream and causes the message
        /// loop to close.
        /// </summary>
        public void Dispose()
        {
            _streamWriter.Dispose();
            _stream.Dispose();
            _disposed = true;
        }
    }
}
