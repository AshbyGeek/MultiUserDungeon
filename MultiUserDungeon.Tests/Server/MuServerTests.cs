using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

using MultiUserDungeon.Common;
using System.Linq;
using System.Threading;
using MultiUserDungeon.Common.NetMsgs;

namespace MultiUserDungeon.Server.Tests
{
    [TestClass]
    public class MuServerTests
    {
        public MuServer FakeSrv = null;
        public MuServer Srv = null;
        public FakeServerFactory factory = null;

        [TestInitialize]
        public void TestInit()
        {
            factory = new FakeServerFactory();

            FakeSrv = new MuServer(factory);
            Srv = new MuServer(new MuServerFactory(), IPAddress.Loopback, 45678);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            FakeSrv.Dispose();
            Srv.Dispose();
        }
        
        [TestMethod]
        public void ServerConnectTest()
        {
            Assert.IsTrue(FakeSrv.Clients.Count == 0);

            // Tell the fake TcpListener to add a connection
            var listener = factory.FakeListeners.First();
            listener.AddConn();
            Thread.Sleep(110);
            Assert.IsTrue(FakeSrv.Clients.Count == 1);
        }

        [TestMethod]
        public void ServerDisconnectTest()
        {
            // Tell the fake TcpListener to add a connection
            var listener = factory.FakeListeners.First();
            listener.AddConn();
            Thread.Sleep(110);
            Assert.IsTrue(FakeSrv.Clients.Count == 1);

            // Tell the client to disconnect
            var fakeClient = FakeSrv.Clients.First() as FakeMuClient;
            fakeClient.Disconnect(false);
            Assert.IsTrue(FakeSrv.Clients.Count == 0);
        }

        [TestMethod]
        public void ServerDisconnectInnactivityTest()
        {
            // Tell the fake TcpListener to add a connection
            var listener = factory.FakeListeners.First();
            listener.AddConn();
            listener.AddConn();
            Thread.Sleep(110);
            Assert.IsTrue(FakeSrv.Clients.Count == 2);

            // Tell the client to disconnect
            var fakeClient = FakeSrv.Clients.First() as FakeMuClient;
            fakeClient.Disconnect(true);
            Assert.IsTrue(FakeSrv.Clients.Count == 1);

            var fakeClient2 = FakeSrv.Clients.First() as FakeMuClient;
            Assert.IsTrue(fakeClient2.Msg is ServerMsg);
        }

        [TestMethod]
        public void ServerBroadcastTest()
        {
            var msg = new ServerMsg("Testing");
            FakeSrv.Broadcast(msg);

            Assert.IsTrue(FakeSrv.Clients.Cast<FakeMuClient>().All(x => x.Msg == msg));
        }

        [TestMethod]
        public void ServerActualTCPClientTest()
        {
            var client = new TcpClient();
            client.Connect(IPAddress.Loopback, Srv.Port);

            Thread.Sleep(110);
            Assert.IsTrue(Srv.Clients.Count == 1);

            var stream = client.GetStream();
            var writer = new StreamWriter(stream);
            writer.Write(new ClientDisconnectMsg().Serialize());
            writer.Flush();
            Thread.Sleep(110);
            Assert.IsTrue(Srv.Clients.Count == 0);
        }
    }
}
