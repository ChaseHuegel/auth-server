using System;
using System.Text;
using System.Threading;

namespace mmorpg_server
{
    class Program
    {
        public const int MAX_PLAYERS = 300;
        public const int PORT = 11774;
        public const int TICK_RATE = 15;
        public const int TIMEOUT = 15;

        private static bool stop = false;

        private static Commands commandHandler = new Commands();
        private static Heartbeat heartbeat = new Heartbeat();

        static void Main(string[] args)
        {
            Server.Start(MAX_PLAYERS, PORT);
            heartbeat.Initialize();

            while (!stop)
            {
                commandHandler.Read();
            }

            heartbeat.Stop();
        }

        public static void Exit()
        {
            stop = true;
        }
    }
}
