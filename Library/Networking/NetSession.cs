using System.Net;
using System;

namespace Swordfish.Library.Networking
{
    public struct NetSession
    {
        public IPEndPoint EndPoint { get; set; }

        public int ID { get; set; }
    }
}
