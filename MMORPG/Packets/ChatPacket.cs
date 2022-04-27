using System;

using Swordfish.Library.Networking;
using Swordfish.Library.Networking.Attributes;
using Swordfish.Library.Networking.Interfaces;

using NetServer = Swordfish.Library.Networking.NetServer;

namespace Swordfish.MMORPG.Packets
{
    [Packet]
    public struct ChatPacket : ISerializedPacket
    {
        public int Sender;

        public string Message;

        [PacketHandler]
        public static void OnChatReceived(NetController net, ChatPacket packet, NetEventArgs e)
        {
            if (net is NetServer)
            {
                //  The server has authority over identifying the sender
                packet.Sender = e.Session.ID;
                net.Broadcast(packet);
            }

            Console.WriteLine($"[{net.GetType().Name}] {packet.Sender}: {packet.Message}");
        }
    }
}
