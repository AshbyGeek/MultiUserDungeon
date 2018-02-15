using System;
using System.Collections.Generic;
using System.Text;

namespace MultiUserDungeon.Common.NetMsgs
{
    /// <summary>
    /// The message sent to the server (and then on to the other clients) by a client as soon
    /// as it connects to the server
    /// </summary>
    public class ClientConnectMsg : NetworkMsg
    {
        public string UserName { get; } = "";

        protected override object[] ContentMembers => new[] { UserName };
        
        public ClientConnectMsg(string username) => UserName = username;
    }
}
