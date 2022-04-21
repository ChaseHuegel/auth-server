using System;
using System.Text;
using System.Threading;
using Swordfish.Networking;

namespace mmorpg_server
{
    class Application
    {
        private static bool stop = false;

        private static Commands commandHandler = new Commands();
        private static Heartbeat heartbeat = new Heartbeat();

        static void Main(string[] args)
        {
            heartbeat.Initialize();

            while (!stop)
                commandHandler.Read();

            heartbeat.Stop();
        }

        public static void Exit()
        {
            stop = true;
        }
    }
}
