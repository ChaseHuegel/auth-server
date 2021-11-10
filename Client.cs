using System;
using System.Net.Sockets;

namespace mmorpg_server
{
    public class Client
    {
        public int ID = -1;
        public TcpConnection Tcp;

        public Entity entity;

        public Client(int id)
        {
            this.ID = id;
            this.Tcp = new TcpConnection(id);
        }

        public void Connect(TcpClient socket)
        {
            Tcp.Connect(socket);
        }

        public void Disconnect()
        {
            ID = -1;
            entity = null;
            Tcp.Disconnect();
        }
    }
}
