using System;

namespace MMORPG
{
    public class Application
    {
        private static bool stop = false;

        private static Heartbeat heartbeat = new Heartbeat();

        static void Main(string[] args)
        {
            try {
                heartbeat.Initialize();

                while (!stop)
                    Commands.Read();

                heartbeat.Stop();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            Console.ReadKey();
        }

        public static void Exit()
        {
            stop = true;
        }
    }
}
