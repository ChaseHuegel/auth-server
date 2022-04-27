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

        public int Flags;

        [PacketHandler]
        public static void OnRegisterReceived(NetController net, RegisterPacket packet, NetEventArgs e)
        {
            RegisterFlags flags = (RegisterFlags) packet.Flags;

            if (net is NetServer)
            {
                flags = Accounts.ValidateUsername(packet.Username) | Accounts.ValidatePassword(packet.Password) | Accounts.ValidateEmail(packet.Email);

                if (flags == RegisterFlags.None)
                    Accounts.Register(packet.Username, packet.Password, packet.Email);

                packet.Flags = (int)flags;
                net.Send(packet, e.EndPoint);
            }

            if (flags == RegisterFlags.None)
                Console.WriteLine($"@{e.EndPoint} Register account [{packet.Username}, {packet.Email}] successful!");
            else
                Console.WriteLine($"@{e.EndPoint} Register account failed [{packet.Username}, {packet.Email}]: {flags}");
        }
    }
}
