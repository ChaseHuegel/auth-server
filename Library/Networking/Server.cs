using System.Reflection;
using System;
using System.Net;
using Swordfish.Library.Networking.Packets;

namespace Swordfish.Library.Networking
{
    public class Server
    {
        public int Port { get; } = 42420;

        public NetController Net { get; }

        public Server(int port)
        {
            Port = port;
            Net = new NetController(Port);

            Console.WriteLine($"Server started on {Net.Session.EndPoint}");

            Net.PacketSent += OnPacketSent;
            Net.PacketReceived += OnPacketReceived;
            Net.PacketAccepted += OnPacketAccepted;
            Net.PacketRejected += OnPacketRejected;

            HandshakePacket handshake = new HandshakePacket {
                Message = "Hello World!"
            };
            
            Net.Send(handshake, "swordfishseven.com", Port);
        }

        public void OnPacketSent(object sender, NetEventArgs e)
        {
            Console.WriteLine($"net->sent {e.PacketID} to {e.EndPoint}");
        }

        public void OnPacketReceived(object sender, NetEventArgs e)
        {
            Console.WriteLine($"net->recieve {e.PacketID} from {sender}");
        }

        public void OnPacketAccepted(object sender, NetEventArgs e)
        {
            Console.WriteLine($"net->accept {e.PacketID} from {sender}");
        }

        public void OnPacketRejected(object sender, NetEventArgs e)
        {
            Console.WriteLine($"net->reject {e.PacketID} from {sender}");
        }
    }
}
