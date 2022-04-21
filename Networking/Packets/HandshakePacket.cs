using Swordfish.Networking.Attributes;
using Swordfish.Networking.Interfaces;

namespace Swordfish.Networking.Packets
{
    [Packet]
    public struct HandshakePacket : IPacket
    {
        public string Message;
    }
}
