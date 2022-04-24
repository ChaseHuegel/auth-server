using Swordfish.Library.Networking.Attributes;
using Swordfish.Library.Networking.Interfaces;

namespace Swordfish.Library.Networking.Packets
{
    [Packet(RequiresSession = true)]
    public struct HandshakePacket : ISerializedPacket
    {
        public string Message;
    }
}