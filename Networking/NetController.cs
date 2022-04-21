using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Net;
using System.Net.Sockets;
using Swordfish.Networking.Interfaces;

namespace Swordfish.Networking
{
    public class NetController
    {
        private UdpClient Udp;

        public int Port { get; }

        public string Address => Dns.GetHostEntry(Dns.GetHostName()).AddressList.First(x => x.AddressFamily == Udp.Client.AddressFamily).ToString();

        public EventHandler<NetEventArgs> PacketSent;

        public EventHandler<NetEventArgs> PacketReceived;

        public NetController(int port)
        {
            Port = port;

            Udp = new UdpClient(Port);
            Udp.BeginReceive(new AsyncCallback(OnReceive), null);
        }

        private void OnSend(IAsyncResult result)
        {
            Udp.EndSend(result);
            PacketSent.Invoke(this, (NetEventArgs) result.AsyncState);
            
            //  TODO If it isn't a fire and forget packet, we should resend with a delay until a response is received
        }

        private void OnReceive(IAsyncResult result)
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);
            byte[] buffer = Udp.EndReceive(result, ref endPoint);

            PacketReceived.Invoke(endPoint, new NetEventArgs {
                Packet = new Packet(buffer),
                EndPoint = endPoint
            });

            //  Continue receiving data
            Udp.BeginReceive(new AsyncCallback(OnReceive), null);
        }

        private static Packet CreateSignedPacket(IPacket value)
        {
            Packet packet = new Packet();
            packet.Write(PacketManager.GetPacketId(value));
            packet.Serialize(value);

            return packet;
        }

        public void Send(IPacket value, string address, int port) => SendSimple(CreateSignedPacket(value), address, port);

        public void Send(IPacket value, IPAddress address, int port) => SendSimple(CreateSignedPacket(value), address, port);

        public void SendSimple(Packet packet, string address, int port) => SendRaw(packet, IPAddress.Parse(address), port);

        public void SendSimple(Packet packet, IPAddress address, int port) => SendRaw(packet.GetBytes(), address, port);

        public void SendRaw(byte[] buffer, IPAddress address, int port)
        {
            IPEndPoint endPoint = new IPEndPoint(address, port);
            NetEventArgs netEventArgs = new NetEventArgs{
                Packet = new Packet(buffer),
                EndPoint = endPoint
            };

            Udp.BeginSend(buffer, buffer.Length, endPoint, OnSend, netEventArgs);
        }
    }
}
