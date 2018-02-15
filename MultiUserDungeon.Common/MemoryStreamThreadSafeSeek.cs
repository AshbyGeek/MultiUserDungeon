using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MultiUserDungeon.Common
{
    /// <summary>
    /// A wrapper for a standard MemoryStream that protects the Position property
    /// against multi-threaded calls and always writes to the end of the stream
    /// without moving the Position pointer.
    /// This allows the stream to closely mimic the behaviour of a NetworkStream
    /// </summary>
    public class MemoryStreamThreadSafeSeek : MemoryStream
    {
        private object _positionLock = new object();

        public MemoryStreamThreadSafeSeek()
        {
        }

        public MemoryStreamThreadSafeSeek(byte[] buffer) : base(buffer)
        {
        }

        public MemoryStreamThreadSafeSeek(int capacity) : base(capacity)
        {
        }

        public MemoryStreamThreadSafeSeek(byte[] buffer, bool writable) : base(buffer, writable)
        {
        }

        public MemoryStreamThreadSafeSeek(byte[] buffer, int index, int count) : base(buffer, index, count)
        {
        }

        public MemoryStreamThreadSafeSeek(byte[] buffer, int index, int count, bool writable) : base(buffer, index, count, writable)
        {
        }

        public MemoryStreamThreadSafeSeek(byte[] buffer, int index, int count, bool writable, bool publiclyVisible) : base(buffer, index, count, writable, publiclyVisible)
        {
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            lock (_positionLock)
            {
                var curPos = base.Position;
                base.Position = base.Length;
                base.Write(buffer, offset, count);
                base.Position = curPos;
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            lock (_positionLock)
            {
                return base.Read(buffer, offset, count);
            }
        }

        public override long Position
        {
            get
            {
                lock (_positionLock)
                {
                    return base.Position;
                }
            }
            set
            {
                lock (_positionLock)
                {
                    base.Position = value;
                }
            }
        }
    }
}
