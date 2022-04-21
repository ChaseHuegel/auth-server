using System;
using System.Threading.Tasks;

namespace mmorpg_server
{
    public class Commands
    {
        public void Read()
        {
            string input = Console.ReadLine();

            if (input.ToLower().Trim() == "stop")
                Application.Exit();
        }
    }
}
