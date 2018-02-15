using Microsoft.VisualStudio.TestTools.UnitTesting;
using MultiUserDungeon.Common.NetMsgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultiUserDungeon.Common.Tests
{
    [TestClass]
    public class NetworkMessageTests
    {
        public NetworkMsg[] TestMsgs = new NetworkMsg[]
        {
            new ClientConnectMsg("Bob"),
            new ClientDisconnectMsg(),
            new ServerMsg("Testing!"),
            new UserTypedMsg("typing"),
        };

        [TestMethod]
        public void MessagesTestMsgsAreRepresentativeTest()
        {
            var netMsgTypes = from assemblyType in typeof(NetworkMsg).Assembly.GetTypes()
                              where assemblyType.IsSubclassOf(typeof(NetworkMsg))
                              select assemblyType;

            foreach(var msgType in netMsgTypes)
            {
                Assert.IsTrue(TestMsgs.Any(x => x.GetType() == msgType));
            }
        }

        [TestMethod]
        public void MessagesDelimetersTest()
        {
            foreach(var msg in TestMsgs)
            {
                ProperlyDelimited(msg.Serialize());
            }
        }

        private void ProperlyDelimited(string serialized)
        {
            Assert.IsTrue(serialized.StartsWith(NetworkMsg.RECORD_SEPARATOR));
            Assert.IsTrue(serialized.EndsWith(NetworkMsg.LINE_FEED));
            Assert.IsTrue(serialized.Length > 2);
        }

        [TestMethod]
        public void MessagesSerializeDeserializeTest()
        {
            foreach(var msg in TestMsgs)
            {
                SerializedEqualsDeserialized(msg);
            }
        }

        private void SerializedEqualsDeserialized(NetworkMsg msg)
        {
            var serialized = msg.Serialize();
            var newMsg = NetworkMsg.Deserialize(serialized);
            Assert.AreEqual(msg, newMsg);
        }

        private class FakeMsg : NetworkMsg
        {
            string Param1 { get; set; }
            string Param2 { get; set; }

            public FakeMsg(string param1, string param2)
            {
                Param1 = param1;
                Param2 = param2;
            }

            protected override object[] ContentMembers => new object[] { Param1, Param2 };
        }

        [TestMethod]
        public void MessagesEqualsTest()
        {
            // Make sure that Equals will return false if the types are different
            Assert.IsFalse(TestMsgs[0].Equals(TestMsgs[1]));
            Assert.IsFalse(TestMsgs[0].Equals(true));

            // Make sure that equals will return false if the content is different
            var msg1 = new FakeMsg("", "");
            var msg2 = new FakeMsg("test", "");
            Assert.IsFalse(msg1.Equals(msg2));
            Assert.IsFalse(msg2.Equals(msg1));

            // Make sure that equals will return true if the types and content are the same
            var msg3 = new FakeMsg("test", "testagain");
            var msg4 = new FakeMsg("test", "testagain");
            Assert.IsTrue(msg3.Equals(msg4));
            Assert.IsTrue(msg4.Equals(msg3));
            Assert.IsTrue(msg3.Equals((object)msg4));
            Assert.IsTrue(msg4.Equals((object)msg3));
        }

        [TestMethod]
        public void MessagesEqualsEqualsTest()
        {
            // Make sure that Equals will return false if the types are different
            Assert.IsFalse(TestMsgs[0] == TestMsgs[1]);
            Assert.IsFalse(TestMsgs[0] == true);

            // Make sure that equals will return false if the content is different
            var msg1 = new FakeMsg("", "");
            var msg2 = new FakeMsg("test", "");
            Assert.IsFalse(msg1 == msg2);
            Assert.IsFalse(msg2 == msg1);

            // Make sure that equals will return true if the types and content are the same
            var msg3 = new FakeMsg("test", "testagain");
            var msg4 = new FakeMsg("test", "testagain");
            Assert.IsTrue(msg3 == msg4);
            Assert.IsTrue(msg4 == msg3);
            Assert.IsTrue(msg3 == (object)msg4);
            Assert.IsTrue(msg4 == (object)msg3);
        }

        [TestMethod]
        public void MessagesNotEqualsEqualsTest()
        {
            // Make sure that Equals will return false if the types are different
            Assert.IsTrue(TestMsgs[0] != TestMsgs[1]);
            Assert.IsTrue(TestMsgs[0] != true);

            // Make sure that equals will return false if the content is different
            var msg1 = new FakeMsg("", "");
            var msg2 = new FakeMsg("test", "");
            Assert.IsTrue(msg1 != msg2);
            Assert.IsTrue(msg2 != msg1);

            // Make sure that equals will return true if the types and content are the same
            var msg3 = new FakeMsg("test", "testagain");
            var msg4 = new FakeMsg("test", "testagain");
            Assert.IsFalse(msg3 != msg4);
            Assert.IsFalse(msg4 != msg3);
            Assert.IsFalse(msg3 != (object)msg4);
            Assert.IsFalse(msg4 != (object)msg3);
        }

        [TestMethod]
        public void MessagesGethashCodeTest()
        {
            var msg1 = new FakeMsg("test", "testagain");
            var msg2 = new FakeMsg("test", "testagain");
            Assert.AreEqual(msg1.GetHashCode(), msg2.GetHashCode());

            var msg3 = new FakeMsg("", "");
            var msg4 = new FakeMsg("", "");
            Assert.AreEqual(msg3.GetHashCode(), msg4.GetHashCode());
        } 
    }
}
