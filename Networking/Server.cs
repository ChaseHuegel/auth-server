using System.Reflection;
using System;
using System.Net;
using Swordfish.Networking.Packets;

namespace Swordfish.Networking
{
    public class Server
    {
        public int Port { get; } = 42420;

        public NetController Net { get; }

        public Server(int port)
        {
            Port = port;
            Net = new NetController(Port);

            Console.WriteLine($"Server started on {Net.Address}:{Net.Port}");

            Net.PacketSent += OnPacketSent;
            Net.PacketReceived += OnPacketReceived;

            HandshakePacket handshake = new HandshakePacket {
                Message = "Hello World!"
            };
            
            Net.Send(handshake, "127.0.0.1", Port);
        }

        public void OnPacketSent(object sender, NetEventArgs e)
        {
            Console.WriteLine($"net     {e.Packet.ReadInt()} -> {e.EndPoint}");
        }

        public void OnPacketReceived(object sender, NetEventArgs e)
        {
            Console.WriteLine($"net     {e.Packet.ReadInt()} <- {sender}");

            //  Identify the client that sent the packet
            ClientConnection client = GetClient((IPEndPoint)sender);

            //  Process the packet
            PacketManager.Process(e.Packet, client);
        }

        private ClientConnection GetClient(IPEndPoint endPoint)
        {
            //  TODO get or create ClientConnection
            ClientConnection client = new ClientConnection {
                EndPoint = endPoint,
                ID = 0
            };

            return client;
        }
    }
}
