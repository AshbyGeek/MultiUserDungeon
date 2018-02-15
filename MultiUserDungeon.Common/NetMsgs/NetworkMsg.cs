using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MultiUserDungeon.Common.NetMsgs
{
    /// <summary>
    /// Base class for all network messages.
    /// Handles serialization and deserialization and equality operator overrides
    /// </summary>
    public abstract class NetworkMsg
    {
        public const char LINE_FEED = '\u000A';
        public const char RECORD_SEPARATOR = '\u001E';

        public static JsonSerializerSettings SerializerSettings => new JsonSerializerSettings()
        {
            TypeNameHandling = TypeNameHandling.All,
        };

        public static NetworkMsg Deserialize(string msg)
        {
            // Remove any delimiting characters
            msg = msg.Trim(RECORD_SEPARATOR, LINE_FEED);
            return JsonConvert.DeserializeObject<NetworkMsg>(msg, SerializerSettings);
        }

        public static string Serialize(NetworkMsg msg)
        {
            var serialized = JsonConvert.SerializeObject(msg, SerializerSettings);

            // add record separators for convenience in handling in streams
            return RECORD_SEPARATOR + serialized + LINE_FEED;
        }

        public string Serialize()
        {
            return Serialize(this);
        }

        /// <summary>
        /// Override this to specify the content members of this object message
        /// Used for Equals and GetHashCode overrides.
        /// Alternately
        /// </summary>
        protected virtual object[] ContentMembers { get; } = null;

        public override bool Equals(object obj)
        {
            // If the two aren't the same type, return false
            if (GetType() != obj?.GetType())
            {
                return false;
            }

            // If we have two messages of the same type and
            // this message type has no content members, they are equal.
            if (ContentMembers == null)
            {
                return true;
            }

            // Loop through both message's content member lists
            // If any don't match, return false
            var other = obj as NetworkMsg;
            for (int i = 0; i < ContentMembers.Length; i++)
            {
                if (!ContentMembers[i].Equals(other.ContentMembers[i]))
                {
                    return false;
                }
            }
            return true;
        }

        public override int GetHashCode()
        {
            if (ContentMembers == null)
            {
                return 0;
            }

            // Start the hash with type info to reduce collisions between types
            // then xor each content member's hash code
            int hash = GetType().GetHashCode();
            foreach (var member in ContentMembers)
            {
                unchecked
                {
                    hash ^= member.GetHashCode();
                }
            }
            return hash;
        }

        public static bool operator ==(NetworkMsg left, object right)
        {
            if (left is null)
            {
                return right is null;
            }
            return left.Equals(right);
        }

        public static bool operator !=(NetworkMsg left, object right)
        {
            return !(left == right);
        }
    }
}
