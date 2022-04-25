using MMORPG.Server;
using Swordfish.Library.Networking;
using Swordfish.Library.Networking.Packets;

using NetClient = Swordfish.Library.Networking.Client;

namespace MMORPG.Client
{
    public class GameClient : NetClient
    {
        public static Host Host = new Host {
            Hostname = "localhost",
            Port = GameServer.PORT
        };

        public GameClient() : base(Host)
        {
            Send(new HandshakePacket());
        }
    }
}
