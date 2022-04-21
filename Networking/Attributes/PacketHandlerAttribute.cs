using System;

namespace Swordfish.Networking.Attributes
{
    /// <summary>
    /// Decorates a method to process a received packet.
    /// <para/>
    /// The method must be public static and accept parameters for <see cref="Swordfish.Networking.Interfaces.IPacket"/> and <see cref="Swordfish.Networking.ClientConnection"/>
    /// <para/>
    /// Signature:
    /// public static void OnPacketReceived(IPacket packet, ClientConnection client)
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class PacketHandlerAttribute : Attribute
    {
        public Type PacketType;

        public PacketHandlerAttribute(Type packetType)
        {
            PacketType = packetType;
        }
    }
}
