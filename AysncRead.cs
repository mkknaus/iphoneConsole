using System;
using System.IO;

namespace iphoneConsole
{
    public class AsyncRead
    {
        public Byte[] Data { get; set; }
        public Stream AStream { get; set; }

        public AsyncRead(Byte[] buf, Stream str)
        {
            Data = buf;
            AStream = str;
        }
    }
}
