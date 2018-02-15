using System;
using System.Collections.Generic;
using System.Text;

namespace MultiUserDungeon.Common.NetMsgs
{
    /// <summary>
    /// A message sent first to the server, informing it that a user typed something
    /// The server then adds the user's index number and passes the message on
    /// to the other clients so they can update that user's current line.
    /// </summary>
    public class UserTypedMsg : NetworkMsg
    {
        /// <summary>
        /// Information added by the server for everyone elses benefit
        /// </summary>
        public int UserIndex { get; set; }
        
        public string Value { get; set; }
        
        public UserTypedMsg(string value)
        {
            Value = value;
        }
    }
}
