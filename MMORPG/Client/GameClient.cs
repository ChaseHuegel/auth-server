using System.Collections.Concurrent;

using Swordfish.Library.Networking;
using Swordfish.MMORPG.Data;

namespace Swordfish.MMORPG.Client
{
    public class GameClient : NetClient
    {
        private static ClientConfig s_Config;
        public static ClientConfig Config => s_Config ?? (s_Config = Library.Util.Config.Load<ClientConfig>("config/client.toml"));

        public static GameClient Instance;

        public ConcurrentDictionary<int, LivingEntity> Characters;

        public GameClient() : base(Config.Connection.Host) {
            Instance = this;
            Characters = new ConcurrentDictionary<int, LivingEntity>();
        }
    }
}
