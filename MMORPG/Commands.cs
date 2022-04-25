using System;

namespace MMORPG
{
    public static class Commands
    {
        public static void Read()
        {
            string input = Console.ReadLine();
            string[] arguments = input.Split(' ');

            switch (arguments[0].ToLower())
            {
                case "stop":
                    Application.Exit();
                    break;
                case "list":
                case "players":
                case "sessions":
                case "connected":
                    Console.WriteLine("Server: " + string.Join(", ", Heartbeat.Server.GetSessions()));
                    Console.WriteLine("Client: " + string.Join(", ", Heartbeat.Client.GetSessions()));
                    break;
            }
        }
    }
}
