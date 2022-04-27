using System.Text;
using System;
using Swordfish.Library.Extensions;
using Swordfish.Library.Integrations.SQL;
using Swordfish.Library.Networking;
using Swordfish.Library.Networking.Attributes;
using Swordfish.Library.Networking.Interfaces;
using Swordfish.Library.Util;
using Swordfish.MMORPG.Enums;

using NetServer = Swordfish.Library.Networking.NetServer;
using System.Linq;
using System.Data.SqlClient;
using System.Data;
using Swordfish.MMORPG.Server;

namespace Swordfish.MMORPG.Packets
{
    [Packet(RequiresSession = false)]
    public struct LoginPacket : ISerializedPacket
    {
        public string Username;

        public string Password;

        //  TODO this should be AccountFlags when packets support enums
        public int Flags;

        [ServerPacketHandler]
        public static void OnLoginServer(NetServer server, LoginPacket packet, NetEventArgs e)
        {
            AccountFlags flags = AccountFlags.None;

            if (!Accounts.VerifyUsername(packet.Username))
                flags |= AccountFlags.UsernameIncorrect;

            if (!Accounts.VerifyPassword(packet.Username, packet.Password))
                flags |= AccountFlags.PasswordIncorrect;

            packet.Flags = (int)flags;
            server.Send(packet, e.EndPoint);

            if (flags == AccountFlags.None)
                Console.WriteLine($"[{e.EndPoint}] logged into account [{packet.Username}]");
            else
                Console.WriteLine($"[{e.EndPoint}] tried to login to account [{packet.Username}]: {flags}");
        }

        [ClientPacketHandler]
        public static void OnLoginClient(NetClient client, LoginPacket packet, NetEventArgs e)
        {
            AccountFlags flags = (AccountFlags)packet.Flags;

            if (flags == AccountFlags.None)
                Console.WriteLine($"Logged into account [{packet.Username}]");
            else
                Console.WriteLine($"Failed to login to account [{packet.Username}]: {flags}");
        }
    }
}
