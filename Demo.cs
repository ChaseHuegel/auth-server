using System;
using Swordfish.Library.Networking;

namespace mmorpg_server
{
    public class Demo
    {
        public const int MAX_PLAYERS = 300;
        public const int PORT = 42420;
        public const int TICK_RATE = 15;

        private Server Server;

        public void Start()
        {
            Server = new Server(42420);

            UpdateTitle();
        }

        public void Tick(float deltaTime)
        {
            if (Server == null)
                return;
            
            UpdateTitle();
        }

        private void UpdateTitle()
        {
            Console.Title = $"MMORPG Server | {Server.Net.Session.EndPoint} | {TICK_RATE}/s | {0}/{MAX_PLAYERS}";
        }
    }
}
