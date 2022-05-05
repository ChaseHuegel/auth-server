using System;
using System.Net;

using Swordfish.Library.Networking.Attributes;
using Swordfish.Library.Networking.Interfaces;

namespace Swordfish.Library.Networking.Packets
{
    [Packet(RequiresSession = false)]
    public struct HandshakePacket : ISerializedPacket
    {
        public int ClientID;

        public int ServerID;

        public HandshakePacket(int id)
        {
            ClientID = id;
            ServerID = NetSession.LocalOrUnassigned;
        }

        private static Func<EndPoint, bool> ValidateHandshakeCallback;
        
        public static void SetValidationCallback(Func<EndPoint, bool> callback) => ValidateHandshakeCallback = callback;

        [ClientPacketHandler]
        public static void ClientHandshakeHandler(NetController net, HandshakePacket packet, NetEventArgs e)
        {
            if (net.Session.ID == NetSession.LocalOrUnassigned && net.TryAddSession(e.EndPoint, packet.ServerID, out NetSession serverSession))
            {
                net.Session.ID = packet.ClientID;
                Console.WriteLine($"Joined [{serverSession}] with session [{net.Session}]");
            }
            else
            {
                //  ? is there a situation where we should accept a new session from the server?
                //  If we already have a session, do nothing and assume the packet was a mistake on the server's part.
                Console.WriteLine($"Recieved a session handshake from {e.EndPoint} but already registered [{net.Session}] with [{e.Session}].");
            }
        }

        [ServerPacketHandler]
        public static void ServerHandshakeHandler(NetController net, HandshakePacket packet, NetEventArgs e)
        {
            //  Attempt to validate the handshake.
            //  If the callback is undefined, all handshakes are considered valid.
            if (ValidateHandshakeCallback?.Invoke(e.EndPoint) ?? true)
            {
                net.TryAddSession(e.EndPoint, out NetSession newSession);

                Console.WriteLine($"{e.EndPoint} joined, assigning session: [{newSession}]");

                HandshakePacket handshake = new HandshakePacket() {
                    ClientID = newSession.ID,
                    ServerID = net.Session.ID
                };

                net.Send(handshake, e.EndPoint);
            }
            else
            {
                Console.WriteLine($"{e.EndPoint} tried to join, failed to validate handshake.");
            }
        }
    }
}
