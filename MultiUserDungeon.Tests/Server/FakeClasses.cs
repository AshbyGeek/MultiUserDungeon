using MultiUserDungeon.Common;
using MultiUserDungeon.Common.NetMsgs;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MultiUserDungeon.Server.Tests
{
    public class FakeMuClient : IMuClient
    {
        public NetworkMsg Msg { get; set; }

        public int UserIndex { get; set; }

        public event EventHandler<DisconnectedEventArgs> Disconnected;
        public event EventHandler<MsgEventArgs> MsgReceived;

        public void SendMessage(NetworkMsg msg)
        {
            Msg = msg;
        }

        public void Disconnect(bool forInnactivity)
        {
            Disconnected?.Invoke(this, new DisconnectedEventArgs(forInnactivity));
        }

        public void Receive(NetworkMsg msg)
        {
            Msg = msg;
            MsgReceived?.Invoke(this, new MsgEventArgs(msg));
        }

        public void Dispose() { }
    }

    public class FakeTcpListener : ITcpListener
    {
        public bool ExclusiveAddressUse
        {
            get => false;
            set { }
        }

        public EndPoint LocalEndpoint => null;

        public Socket Server => DummySocket;

        public Socket AcceptSocket()
        {
            var task = AcceptSocketAsync();
            task.Wait();
            return task.Result;
        }

        public async Task<Socket> AcceptSocketAsync()
        {
            while (IsRunning)
            {
                if (Pending())
                {
                    PendingConns--;
                    return DummySocket;
                }
                else
                {
                    await Task.Delay(100);
                }
            }
            return null;
        }

        public void AllowNatTraversal(bool allowed) { }

        public bool Pending() => PendingConns > 0;

        public void Start()
        {
            IsRunning = true;
        }

        public void Start(int backlog)
        {
            IsRunning = true;
        }

        public void Stop()
        {
            IsRunning = false;
        }
        
        public void AddConn()
        {
            PendingConns++;
        }

        public Socket DummySocket => new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        private bool IsRunning { get; set; }
        private int PendingConns;
    }

    public class FakeServerFactory : IMuServerFactory
    {
        public List<FakeMuClient> FakeClients = new List<FakeMuClient>();
        public List<FakeTcpListener> FakeListeners = new List<FakeTcpListener>();

        public IMuClient CreateMuClient(Socket sock)
        {
            var client = new FakeMuClient();
            FakeClients.Add(client);
            return client;
        }

        public ITcpListener CreateTcpListener(IPAddress address, int port)
        {
            var listener = new FakeTcpListener();
            FakeListeners.Add(listener);
            return listener;
        }
    }
}
