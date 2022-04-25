using System;

using Swordfish.Library.Networking;
using Swordfish.Library.Networking.Packets;

namespace Demo
{
    public class Demo
    {
        public static Demo Instance { get; private set; }

        public const string HOSTNAME = "localhost";
        public const int PORT = 42420;
        public const int MAX_PLAYERS = 300;
        public const int TICK_RATE = 15;

        public Server Server { get; private set; }
        public Client Client { get; private set; }

        public void Start()
        {
            Instance = this;

            Host host = new Host {
                Hostname = HOSTNAME,
                Port = PORT
            };

            Server = new Server(PORT);
            Client = new Client(host);

            Client.Send(new HandshakePacket());

            UpdateTitle();
        }

        public void Tick(float deltaTime)
        {            
            UpdateTitle();
        }

        private void UpdateTitle()
        {
            Console.Title = $"MMORPG Server | {Server?.Session.EndPoint} | {TICK_RATE}/s | {Server?.SessionCount}/{MAX_PLAYERS}";
        }
    }
}
