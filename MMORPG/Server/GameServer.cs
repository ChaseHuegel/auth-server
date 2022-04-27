using Swordfish.Library.Networking;

namespace Swordfish.MMORPG.Server
{
    public class GameServer : NetServer
    {
        private static ServerConfig s_Config;
        public static ServerConfig Config => s_Config ?? (s_Config = Library.Util.Config.Load<ServerConfig>("config/server.toml"));

        public GameServer() : base(Config.Connection.Port)
        {
            MaxSessions = Config.Connection.MaxPlayers;
        }
    }
}
