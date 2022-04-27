using Swordfish.Library.Networking;
using Swordfish.Library.Networking.Packets;

namespace Swordfish.MMORPG.Client
{
    public class GameClient : NetClient
    {
        private static ClientConfig s_Config;
        public static ClientConfig Config => s_Config ?? (s_Config = Library.Util.Config.Load<ClientConfig>("config/client.toml"));

        public GameClient() : base(Config.Connection.Host) {
            Send(new HandshakePacket());
        }
    }
}
