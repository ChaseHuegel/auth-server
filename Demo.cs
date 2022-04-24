using System;
using Swordfish.Library.Networking;
using Swordfish.Library.Networking.Packets;

namespace mmorpg_server
{
    public class Demo
    {
        private static Demo Instance;

        public const int MAX_PLAYERS = 300;
        public const int PORT = 42420;
        public const int TICK_RATE = 15;

        private Server Server;

        public void Start()
        {
            Instance = this;
            Server = new Server(PORT);

            UpdateTitle();
        }

        public void Tick(float deltaTime)
        {
            if (Server == null)
                return;
            
            UpdateTitle();
        }

        public static void Send(string message)
        {
            HandshakePacket handshake = new HandshakePacket {
                Message = message
            };

            Instance.Server.Net.Send(handshake, "localhost", PORT);
        }

        private void UpdateTitle()
        {
            Console.Title = $"MMORPG Server | {Server.Net.Session.EndPoint} | {TICK_RATE}/s | {0}/{MAX_PLAYERS}";
        }
    }
}
