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
    public struct RegisterPacket : ISerializedPacket
    {
        public string Username;

        public string Password;

        public string Email;

        //  TODO this should be RegisterFlags when packets support enums
        public int Flags;

        [ServerPacketHandler]
        public static void OnRegisterServer(NetServer server, RegisterPacket packet, NetEventArgs e)
        {
            RegisterFlags flags = Accounts.ValidateUsername(packet.Username) | Accounts.ValidatePassword(packet.Password) | Accounts.ValidateEmail(packet.Email);

            if (flags == RegisterFlags.None)
                Accounts.Register(packet.Username, packet.Password, packet.Email);

            packet.Flags = (int)flags;
            server.Send(packet, e.EndPoint);

            if (flags == RegisterFlags.None)
                Console.WriteLine($"[{e.EndPoint}] registered account [{packet.Username}, {packet.Email}]");
            else
                Console.WriteLine($"[{e.EndPoint}] tried to register account [{packet.Username}, {packet.Email}]: {flags}");
        }

        [ClientPacketHandler]
        public static void OnRegisterClient(NetClient client, RegisterPacket packet, NetEventArgs e)
        {
            RegisterFlags flags = (RegisterFlags)packet.Flags;

            if (flags == RegisterFlags.None)
                Console.WriteLine($"Account [{packet.Username}, {packet.Email}] registered!");
            else
                Console.WriteLine($"Failed to register account [{packet.Username}, {packet.Email}]: {flags}");
        }
    }
}
