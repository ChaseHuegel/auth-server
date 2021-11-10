using System;
using System.Diagnostics;
using System.Threading;
using Swordfish.Threading;

namespace mmorpg_server
{
    public class Heartbeat
    {
        private ThreadWorker thread;

        public void Initialize()
        {
            UpdateTitle();

            thread = new ThreadWorker(Run, false, "MainHeartbeat");
            thread.TargetTickRate = Program.TICK_RATE;
            thread.Start();
        }

        public void Run(float deltaTime)
        {
            Server.Tick();
            UpdateTitle();
        }

        public void Stop()
        {
            thread.Stop();
        }

        static void UpdateTitle()
        {
            Console.Title = $"MMORPG Server | {Server.GetAddress()} | {Program.TICK_RATE}/s | {Server.GetPlayerCount()}/{Program.MAX_PLAYERS}";
        }
    }
}
