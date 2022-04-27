using Swordfish.Library.Networking;
using Swordfish.Library.Networking.Packets;
using Swordfish.MMORPG.Server;

using NetClient = Swordfish.Library.Networking.NetClient;

namespace Swordfish.MMORPG.Client
{
    public class GameClient : NetClient
    {
        public static Host Host = new Host
        {
            Hostname = "localhost",
            Port = GameServer.PORT
        };

        public GameClient() : base(Host)
        {
            Send(new HandshakePacket());
        }
    }
}
