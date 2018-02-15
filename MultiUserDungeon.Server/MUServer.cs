using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

using MultiUserDungeon.Common;
using MultiUserDungeon.Common.NetMsgs;

namespace MultiUserDungeon.Server
{
    public interface IMuServerFactory
    {
        IMuClient CreateMuClient(Socket sock);
        ITcpListener CreateTcpListener(IPAddress address, int port);
    }

    /// <summary>
    /// Factory for MuClients and TcpListeners
    /// </summary>
    public class MuServerFactory : IMuServerFactory
    {
        /// <summary>
        /// Creates a NetworkStream from a socket and uses that stream to create
        /// an MuClient
        /// </summary>
        /// <param name="sock"></param>
        /// <returns></returns>
        public IMuClient CreateMuClient(Socket sock)
        {
            var stream = new NetworkStream(sock, true);
            return new MuClient(stream);
        }

        /// <summary>
        /// Creates a TcpListenerWrapper with the given address and port
        /// </summary>
        /// <param name="address"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public ITcpListener CreateTcpListener(IPAddress address, int port)
        {
            return new TcpListenerWrapper(address, port);
        }
    }

    /// <summary>
    /// The main body of the server, listens for new connections and creates and manages
    /// server-side representations of clients (Server.MuClient) for communicating back and forth
    /// </summary>
    public class MuServer : IDisposable
    {
        public IPAddress IPAddress { get; }
        public int Port { get; }

        private ITcpListener _Listener { get; }
        private IMuServerFactory _ClientFactory { get; }

        public IList<IMuClient> Clients { get; } = new List<IMuClient>();
        
        public MuServer(IMuServerFactory factory, IPAddress address = null, int port = 12345)
        {
            if (address == null)
            {
                address = IPAddress.Any;
            }

            IPAddress = address;
            Port = port;
            _ClientFactory = factory;
            _Listener = factory.CreateTcpListener(address, port);
            _Listener.Start();
            Task.Factory.StartNew(Listen);
        }
        
        /// <summary>
        /// The main listening loop. Adds a new MuClient to the Clients list
        /// whenever a new socket is accepted
        /// </summary>
        /// <returns></returns>
        public async Task Listen()
        {
            try
            {
                while (true)
                {
                    var sock = await _Listener.AcceptSocketAsync();
                    if (sock == null)
                    {
                        break;
                    }

                    var client = _ClientFactory.CreateMuClient(sock);
                    client.Disconnected += Conn_Disconnected;
                    Clients.Add(client);
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine($"SocketException: {e}");
            }
            finally
            {
                _Listener?.Stop();
            }
        }

        /// <summary>
        /// Sends a message to all connected clients
        /// </summary>
        /// <param name="msg"></param>
        public void Broadcast(NetworkMsg msg)
        {
            foreach(var client in Clients)
            {
                client.SendMessage(msg);
            }
        }

        /// <summary>
        /// Closes out all current connections and stops listening for connections
        /// </summary>
        public void Dispose()
        {
            _Listener.Stop();
            foreach(var client in Clients)
            {
                client.Dispose();
            }
        }

        /// <summary>
        /// Event handler for MuClient.Disconnected events
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Conn_Disconnected(object sender, EventArgs e)
        {
            var conn = sender as IMuClient;
            if (conn != null)
            {
                conn.Dispose();
                conn.Disconnected -= Conn_Disconnected;
                this.Clients.Remove(conn);
            }
        }

        /// <summary>
        /// Starts the server using default parameters
        /// Eventually will be upgraded to use a config file
        /// </summary>
        /// <returns></returns>
        public static MuServer StartServer()
        {
            Console.WriteLine("Starting server...");

            //TODO: load parameters from some sort of config file
            var srv = new MuServer(new MuServerFactory(), IPAddress.Any);

            return srv;
        }
    }
}
