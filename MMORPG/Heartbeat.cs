using System;

using MMORPG.Client;
using MMORPG.Server;

using Swordfish.Library.Networking;
using Swordfish.Threading;

namespace MMORPG
{
    public class Heartbeat
    {
        public const int TICK_RATE = 15;
        public static GameClient Client;
        public static GameServer Server;

        private ThreadWorker thread;

        public void Initialize()
        {
            thread = new ThreadWorker(Tick, false, "Heartbeat");
            thread.TargetTickRate = TICK_RATE;
            thread.Start();

            Start();
        }

        public void Start()
        {
            PacketManager.Initialize();
            Server = new GameServer();
            Client = new GameClient();
        }

        public void Stop()
        {
            thread.Stop();
        }

        public void Tick(float deltaTime)
        {
            UpdateTitle();
        }

        private void UpdateTitle()
        {
            Console.Title = $"MMORPG Server | {Server?.Session.EndPoint} | {TICK_RATE}/s | {Server?.SessionCount}/{Server?.MaxSessions}";
        }
    }
}
