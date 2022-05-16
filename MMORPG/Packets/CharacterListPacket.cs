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
    public struct CharacterListPacket : ISerializedPacket
    {
        public string[] CharacterNames;

        //  TODO should be an enum when packets support them
        public int Flags;

        [ServerPacketHandler]
        public static void OnCharacterListPacketServer(NetServer server, CharacterListPacket packet, NetEventArgs e)
        {
            AccountFlags flags = AccountFlags.None;

            if (!GameServer.Instance.Logins.TryGetValue(e.EndPoint, out string username))
                flags |= AccountFlags.UsernameIncorrect;

            if (flags == AccountFlags.None)
                packet.CharacterNames = Characters.GetCharacterList(username);
            
            packet.Flags = (int)flags;
            server.Send(packet, e.EndPoint);
            
            if (flags == AccountFlags.None)
            {
                Console.WriteLine($"[{e.EndPoint}] requested character list for [{username}]");
            }
            else
            {
                Console.WriteLine($"[{e.EndPoint}] failed to request character list for [{username}]: {flags}");
            }
        }

        [ClientPacketHandler]
        public static void OnCharacterListPacketClient(NetClient client, CharacterListPacket packet, NetEventArgs e)
        {
            AccountFlags flags = (AccountFlags)packet.Flags;

            if (flags == AccountFlags.None)
            {
                Console.WriteLine($"Character List");
                for (int i = 0; i < packet.CharacterNames.Length; i++)
                    Console.WriteLine($"[{i+1}] {packet.CharacterNames[i]}");
            }
            else
            {
                Console.WriteLine($"Failed to retrieve character list: {flags}");
            }
        }
    }
}
