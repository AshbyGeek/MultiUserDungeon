using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MultiUserDungeon.Common.NetMsgs
{
    /// <summary>
    /// A message sent from the server to the clients containing
    /// information to be displayed to the user in the Server section
    /// </summary>
    public class ServerMsg : NetworkMsg
    {
        /// <summary>
        /// The actual text content to be displayed
        /// </summary>
        public string Payload { get; set; } = String.Empty;
        

        public ServerMsg(string payload)
        {
            Payload = payload;
        }

        protected override object[] ContentMembers => new[] { Payload };
    }
}
