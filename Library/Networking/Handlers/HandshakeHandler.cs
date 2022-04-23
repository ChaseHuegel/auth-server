using System;
using Swordfish.Library.Networking.Attributes;
using Swordfish.Library.Networking.Packets;

namespace Swordfish.Library.Networking.Handlers
{
    public static class HandshakeHandler
    {
        [PacketHandler(typeof(HandshakePacket))]
        public static void OnHandshakeReceived(HandshakePacket packet, NetSession session)
        {
            Console.WriteLine($"Handshake '{packet.Message}' from client [{session.ID}] @{session.EndPoint}");
        }
    }
}
