using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MultiUserDungeon.Common;
using MultiUserDungeon.Common.NetMsgs;
using System.Threading;

namespace MultiUserDungeon.Server.Tests
{
    [TestClass]
    public class MuClientTests
    {
        private class FakeNetworkMsg : NetworkMsg
        {
            public string Content { get; set; }

            protected override object[] ContentMembers => new[] { Content };
        }

        [TestMethod]
        public void ClientSendMessageTest()
        {
            var stream = new MemoryStreamThreadSafeSeek();
            var reader = new StreamReader(stream);

            var client = new MuClient(stream);

            var msg = new FakeNetworkMsg()
            {
                Content = "SendTest"
            };
            client.SendMessage(msg);

            var recvdMsg = reader.ReadToEnd();
            Assert.AreEqual(msg.Serialize(), recvdMsg);
        }

        [TestMethod]
        public void ClientReceiveMessageTest()
        {
            var stream = new MemoryStreamThreadSafeSeek();
            var writer = new StreamWriter(stream);

            var client = new MuClient(stream);
            NetworkMsg rcvdMsg = null;
            client.MsgReceived += (s, e) => rcvdMsg = e.Msg;

            var msg = new FakeNetworkMsg()
            {
                Content = "ReceiveTest"
            };
            writer.WriteAndRewind(msg.Serialize());
            Thread.Sleep(110);
            Assert.AreEqual(msg, rcvdMsg);
        }

        [TestMethod]
        public void ClientReceiveMultiMessageTest()
        {
            var stream = new MemoryStreamThreadSafeSeek();
            var writer = new StreamWriter(stream);

            var client = new MuClient(stream);
            List<NetworkMsg> rcvdMsg = new List<NetworkMsg>();
            client.MsgReceived += (s, e) => rcvdMsg.Add(e.Msg);

            var msg = new FakeNetworkMsg()
            {
                Content = "ReceiveTest"
            };
            var emptyMsg = new FakeNetworkMsg()
            {
                Content = ""
            };
            writer.WriteAndRewind(msg.Serialize());
            writer.WriteAndRewind(emptyMsg.Serialize());
            writer.WriteAndRewind(msg.Serialize());

            Thread.Sleep(220);
            Assert.AreEqual(msg, rcvdMsg[0]);
            Assert.AreEqual(emptyMsg, rcvdMsg[1]);
            Assert.AreEqual(msg, rcvdMsg[2]);
        }

        [TestMethod]
        public void ClientDisconnectMessageTest()
        {
            bool connected = true;
            var stream = new MemoryStreamThreadSafeSeek();
            var writer = new StreamWriter(stream);

            var client = new MuClient(stream);
            client.Disconnected += (s,e) => connected = false;

            var msg = new ClientDisconnectMsg();
            writer.WriteAndRewind(msg.Serialize());
            Thread.Sleep(120);
            Assert.IsFalse(connected);
        }
    }
}
