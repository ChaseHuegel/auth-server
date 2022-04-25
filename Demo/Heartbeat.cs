using Swordfish.Threading;

namespace Demo
{
    public class Heartbeat
    {
        private ThreadWorker thread;

        private Demo Demo;

        public void Initialize()
        {
            Demo = new Demo();
            thread = new ThreadWorker(Demo.Tick, false, "MainHeartbeat");
            thread.TargetTickRate = Demo.TICK_RATE;
            thread.Start();

            Demo.Start();
        }

        public void Stop()
        {
            thread.Stop();
        }
    }
}
