using System;

using Swordfish.Library.Networking;
using Swordfish.Library.Networking.Attributes;
using Swordfish.Library.Networking.Interfaces;
using Swordfish.Library.Types;
using Swordfish.MMORPG.Enums;
using Swordfish.MMORPG.Server;

namespace Swordfish.MMORPG.Packets
{
    [Packet(RequiresSession = false)]
    public struct JoinWorldPacket : ISerializedPacket
    {
        public int Slot;

        //  TODO should be an enum when packets support them
        public int Flags;

        [ServerPacketHandler]
        public static void OnJoinWorldPacketServer(NetServer server, JoinWorldPacket packet, NetEventArgs e)
        {
            JoinWorldFlags flags = JoinWorldFlags.None;

            //  Verify the endpoint is logged into an account and capture that account
            if (!GameServer.Instance.Logins.TryGetValue(e.EndPoint, out string username))
                flags |= JoinWorldFlags.NotLoggedIn;

            string characterName = Characters.GetCharacterList(username)[packet.Slot];

            packet.Flags = (int)flags;
            server.Send(packet, e.EndPoint);

            if (flags == JoinWorldFlags.None)
            {
                Console.WriteLine($"[{e.EndPoint}] is entering the world as [{characterName}]");
            }
            else
            {
                Console.WriteLine($"[{e.EndPoint}] tried to enter the world as [{characterName}]: {flags}");
            }
        }

        [ClientPacketHandler]
        public static void OnJoinWorldPacketClient(NetClient client, JoinWorldPacket packet, NetEventArgs e)
        {
            JoinWorldFlags flags = (JoinWorldFlags)packet.Flags;

            if (flags == JoinWorldFlags.None)
            {
                Console.WriteLine($"Entering the world with character [{packet.Slot}]");
                client.Handshake();
            }
            else
            {
                Console.WriteLine($"Failed to enter the world with character [{packet.Slot}]: {flags}");
            }
        }
    }
}
