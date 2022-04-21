using System;
using Swordfish.Networking.Attributes;
using Swordfish.Networking.Packets;

namespace Swordfish.Networking.Handlers
{
    public static class HandshakeHandler
    {
        [PacketHandler(typeof(HandshakePacket))]
        public static void OnHandshakeReceived(HandshakePacket packet, ClientConnection client)
        {
            Console.WriteLine($"Handshake '{packet.Message}' from client [{client.ID}] @{client.EndPoint}");
        }
    }
}
