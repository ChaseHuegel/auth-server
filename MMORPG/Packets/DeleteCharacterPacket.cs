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
    public struct DeleteCharacterPacket : ISerializedPacket
    {
        public int Slot;

        //  TODO should be an enum when packets support them
        public int Flags;

        [ServerPacketHandler]
        public static void OnDeleteCharacterServer(NetServer server, DeleteCharacterPacket packet, NetEventArgs e)
        {
            DeleteCharacterFlags flags = DeleteCharacterFlags.None;

            //  Verify the endpoint is logged into an account and capture that account
            if (!GameServer.Instance.Logins.TryGetValue(e.EndPoint, out string username))
                flags |= DeleteCharacterFlags.NotLoggedIn;

            if (flags == DeleteCharacterFlags.None)
                Characters.DeleteCharacter(username, packet.Slot);

            packet.Flags = (int)flags;
            server.Send(packet, e.EndPoint);

            if (flags == DeleteCharacterFlags.None)
            {
                Console.WriteLine($"[{e.EndPoint}] deleted character slot [{packet.Slot}]");
            }
            else
            {
                Console.WriteLine($"[{e.EndPoint}] tried to delete character slot [{packet.Slot}]: {flags}");
            }
        }

        [ClientPacketHandler]
        public static void OnDeleteCharacterClient(NetClient client, DeleteCharacterPacket packet, NetEventArgs e)
        {
            DeleteCharacterFlags flags = (DeleteCharacterFlags)packet.Flags;

            if (flags == DeleteCharacterFlags.None)
            {
                Console.WriteLine($"Deleted character slot [{packet.Slot}]");
            }
            else
            {
                Console.WriteLine($"Failed to delete character [{packet.Slot}]: {flags}");
            }
        }
    }
}
