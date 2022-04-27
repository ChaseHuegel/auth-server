using Swordfish.Library.Networking;

using NetServer = Swordfish.Library.Networking.NetServer;

namespace Swordfish.MMORPG.Server
{
    public class GameServer : NetServer
    {
        public const int PORT = 42420;
        public const int MAX_PLAYERS = 300;

        public GameServer() : base(PORT)
        {
            MaxSessions = MAX_PLAYERS;
        }
    }
}
