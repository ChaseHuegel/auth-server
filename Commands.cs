using System;
using System.Linq;
using System.Threading.Tasks;

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
                
                case "send":
                    Demo.Send(string.Join(" ", arguments.Skip(1)));
                    break;
            }
        }
    }
}
