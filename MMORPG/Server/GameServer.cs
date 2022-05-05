using System.Net;
using System.Collections.Generic;
using System.Collections.Concurrent;

using Swordfish.Library.Networking;
using Swordfish.MMORPG.Data;
using Swordfish.Library.Networking.Packets;
using System;

namespace Swordfish.MMORPG.Server
{
    public class GameServer : NetServer
    {
        private static ServerConfig s_Config;
        public static ServerConfig Config => s_Config ?? (s_Config = Library.Util.Config.Load<ServerConfig>("config/server.toml"));

        public static GameServer Instance;

        public ConcurrentDictionary<int, LivingEntity> Characters = new ConcurrentDictionary<int, LivingEntity>();

        public Dictionary<EndPoint, string> Logins = new Dictionary<EndPoint, string>();

        public GameServer() : base(Config.Connection.Port)
        {
            Instance = this;
            MaxSessions = Config.Connection.MaxPlayers;
            
            HandshakePacket.SetValidationCallback(ValidateHandshake);
        }

        public bool ValidateHandshake(EndPoint endPoint)
        {
            return Logins.ContainsKey(endPoint);
        }

        public override void OnSessionStarted(object sender, NetEventArgs e)
        {
            LivingEntity character = new LivingEntity {
                ID = e.Session.ID
            };

            Characters.TryAdd(e.Session.ID, character);
            Console.WriteLine($"[{e.Session}] joined the game world.");
        }
    }
}
