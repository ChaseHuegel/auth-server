namespace MMORPG
{
    public class Application
    {
        private static bool stop = false;

        private static Heartbeat heartbeat = new Heartbeat();

        static void Main(string[] args)
        {
            heartbeat.Initialize();

            while (!stop)
                Commands.Read();

            heartbeat.Stop();
        }

        public static void Exit()
        {
            stop = true;
        }
    }
}
