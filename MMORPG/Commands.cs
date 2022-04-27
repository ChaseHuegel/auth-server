using System;
using System.Linq;

using Swordfish.MMORPG.Packets;

namespace Swordfish.MMORPG
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
                case "say":
                    ChatPacket chat = new ChatPacket {
                        Message = string.Join(' ', arguments.Skip(1))
                    };

                    Heartbeat.Client.Send(chat);
                    break;
                case "register":
                    RegisterPacket register = new RegisterPacket {
                        Username = arguments[1],
                        Email = arguments[2],
                        Password = arguments[3]
                    };

                    Heartbeat.Client.Send(register);
                    break;
                case "login":
                    LoginPacket login = new LoginPacket {
                        Username = arguments[1],
                        Password = arguments[2]
                    };

                    Heartbeat.Client.Send(login);
                    break;
            }
        }
    }
}
