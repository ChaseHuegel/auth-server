using System;
using System.Linq;

namespace mmorpg_server
{
    public class Commands
    {
        public void Read()
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
                    Console.WriteLine(string.Join(", ", Demo.Instance.Server.GetSessions()));
                    break;
            }
        }
    }
}
