using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Reflection;

using Swordfish.Library.Networking.Interfaces;

namespace Swordfish.Library.Networking
{
    public class NetController
    {
        private UdpClient Udp { get; }


        private ConcurrentDictionary<IPEndPoint, NetSession> Sessions { get; }

        public NetSession Session { get; }

        public EventHandler<NetEventArgs> PacketSent;
        public EventHandler<NetEventArgs> PacketAccepted;
        public EventHandler<NetEventArgs> PacketReceived;
        public EventHandler<NetEventArgs> PacketRejected;

        public NetController(int port, int sessionID = 0)
        {
            Sessions = new ConcurrentDictionary<IPEndPoint, NetSession>();
            Udp = new UdpClient(port);
            Udp.BeginReceive(new AsyncCallback(OnReceive), null);

            Session = new NetSession {
                EndPoint = (IPEndPoint)Udp.Client.LocalEndPoint,
                ID = sessionID
            };
            Sessions.TryAdd(Session.EndPoint, Session);

            Console.WriteLine($"NetController session [{Session.ID}] started @ {Session.EndPoint}");
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
            Packet packet = Udp.EndReceive(result, ref endPoint);

            int sessionID = packet.ReadInt();
            int packetID = packet.ReadInt();
            PacketDefinition packetDefinition = PacketManager.GetPacketDefinition(packetID);

            PacketReceived?.Invoke(endPoint, new NetEventArgs {
                Packet = packet,
                EndPoint = endPoint
            });

            //  The packet is accepted if the session validates OR the packet doesn't require a session
            if (VerifySession(endPoint, sessionID, out NetSession session) || !packetDefinition.RequiresSession)
            {
                PacketAccepted?.Invoke(endPoint, new NetEventArgs {
                    Packet = packet,
                    EndPoint = endPoint,
                    Session = session
                });

                //  Deserialize the packet and invoke it's handlers
                object deserializedPacket = (ISerializedPacket) packet.Deserialize(packetDefinition.Type);
                foreach (MethodInfo handler in packetDefinition.Handlers)
                    handler.Invoke(null, new object[] { deserializedPacket, session });
            }
            else
            {
                PacketRejected?.Invoke(endPoint, new NetEventArgs {
                    Packet = packet,
                    EndPoint = endPoint
                });
            }

            //  Continue receiving data
            Udp.BeginReceive(new AsyncCallback(OnReceive), null);
        }

        private bool VerifySession(IPEndPoint endPoint, int sessionID, out NetSession netSession)
        {
            //  TODO for testing session is valid if local
            if (endPoint.Address.Equals(IPAddress.Loopback))
            {
                netSession = Session;
                return true;
            }
            else
            {
                bool validEndpoint = Sessions.TryGetValue(endPoint, out NetSession validSession);
                netSession = validSession;
                return validEndpoint && sessionID == validSession.ID; 
            }
        }

        private Packet SignPacket(ISerializedPacket value)
        {
            return Packet.Create()
                    .Write(Session.ID)
                    .Write(PacketManager.GetPacketDefinition(value).ID)
                    .Serialize(value);
        }

        public void Send(ISerializedPacket value, NetSession session) => Send(SignPacket(value), session.EndPoint.Address, session.EndPoint.Port);

        public void Send(ISerializedPacket value, IPEndPoint endPoint) => Send(SignPacket(value), endPoint.Address, endPoint.Port);

        public void Send(ISerializedPacket value, IPAddress address, int port) => Send(SignPacket(value), address, port);

        public void Send(ISerializedPacket value, string hostname, int port) => Send(SignPacket(value), hostname, port);

        public void Send(byte[] buffer, IPAddress address, int port)
        {
            IPEndPoint endPoint = new IPEndPoint(address, port);
            NetEventArgs netEventArgs = new NetEventArgs {
                Packet = buffer,
                EndPoint = endPoint
            };

            Udp.BeginSend(buffer, buffer.Length, endPoint, OnSend, netEventArgs);
        }

        public void Send(byte[] buffer, string hostname, int port)
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.None, port);
            NetEventArgs netEventArgs = new NetEventArgs {
                Packet = buffer,
                EndPoint = endPoint
            };

            Udp.BeginSend(buffer, buffer.Length, hostname, port, OnSend, netEventArgs);
        }
    }
}
