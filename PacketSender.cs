using System;

namespace mmorpg_server
{
    public class PacketSender
    {
        static void Send(int clientID, Packet packet)
        {
            Server.GetClients()[clientID].Tcp.SendData(packet.Pack());
        }

        static void SendAll(Packet packet)
        {
            packet.Pack();
            byte[] data = packet.GetBytes();

            for (int i = 0; i < Server.MaxPlayers; i++)
                Server.GetClients()[i].Tcp.SendData(data);
        }

        static void SendAllExcept(int exceptionID, Packet packet)
        {
            packet.Pack();
            byte[] data = packet.GetBytes();

            for (int i = 0; i < Server.MaxPlayers; i++)
                if (i != exceptionID)
                    Server.GetClients()[i].Tcp.SendData(data);
        }

        public static void Handshake(int clientID, string msg)
        {
            Packet packet = new Packet();

            packet.Write( (int) Packets.Handshake );
            packet.Write(clientID);
            packet.Write(msg);

            Send(clientID, packet);
        }

        public static void RegisterSuccess(int clientID)
        {
            Packet packet = new Packet();

            packet.Write( (int) Packets.RegisterSuccess );

            Send(clientID, packet);
        }

        public static void RegisterFailed(int clientID, RegisterErrorCode error)
        {
            Packet packet = new Packet();

            packet.Write( (int) Packets.RegisterFailed );
            packet.Write((int)error);

            Send(clientID, packet);
        }

        public static void LoginSuccess(int clientID, string username)
        {
            Packet packet = new Packet();

            packet.Write( (int) Packets.LoginSuccess );
            packet.Write(Database.GetCharacterGuid(username).ToString());

            Send(clientID, packet);
        }

        public static void LoginFailed(int clientID, LoginErrorCode error)
        {
            Packet packet = new Packet();

            packet.Write( (int) Packets.LoginFailed );
            packet.Write((int)error);

            Send(clientID, packet);
        }

        public static void JoinSuccess(int clientID, Guid guid)
        {
            Packet packet = new Packet();

            packet.Write( (int) Packets.JoinSuccess );
            packet.Write(guid.ToString());

            Send(clientID, packet);
        }

        public static void JoinFailed(int clientID, JoinErrorCode error)
        {
            Packet packet = new Packet();

            packet.Write( (int) Packets.JoinFailed );
            packet.Write((int)error);

            Send(clientID, packet);
        }

        public static void EntityPosition(int clientID, Entity entity)                  => Send(clientID, EntityPositionPacket(entity));
        public static void BroadcastEntityPosition(Entity entity)                       => SendAll(EntityPositionPacket(entity));
        public static void BroadcastEntityPositionEx(int exceptionID, Entity entity)    => SendAllExcept(exceptionID, EntityPositionPacket(entity));

        private static Packet EntityPositionPacket(Entity entity)
        {
            Packet packet = new Packet();

            packet.Write( (int) Packets.EntityPosition );
            packet.Write(entity.GUID.ToString());
            packet.Write(entity.x);
            packet.Write(entity.y);
            packet.Write(entity.z);
            packet.Write(entity.rotX);
            packet.Write(entity.rotY);

            return packet;
        }

        public static void SpawnEntity(int clientID, Entity entity)                 => Send(clientID, SpawnEntityPacket(entity));
        public static void BroadcastSpawnEntity(Entity entity)                      => SendAll(SpawnEntityPacket(entity));
        public static void BroadcastSpawnEntityEx(int exceptionID, Entity entity)   => SendAllExcept(exceptionID, SpawnEntityPacket(entity));

        private static Packet SpawnEntityPacket(Entity entity)
        {
            Packet packet = new Packet();

            packet.Write( (int) Packets.SpawnEntity );
            packet.Write(entity.GUID.ToString());
            packet.Write(entity.x);
            packet.Write(entity.y);
            packet.Write(entity.z);
            packet.Write(entity.rotX);
            packet.Write(entity.rotY);

            return packet;
        }

        public static void RemoveEntity(int clientID, Entity entity)                 => Send(clientID, RemoveEntityPacket(entity));
        public static void BroadcastRemoveEntity(Entity entity)                      => SendAll(RemoveEntityPacket(entity));
        public static void BroadcastRemoveEntityEx(int exceptionID, Entity entity)   => SendAllExcept(exceptionID, RemoveEntityPacket(entity));

        private static Packet RemoveEntityPacket(Entity entity)
        {
            Packet packet = new Packet();

            packet.Write( (int) Packets.RemoveEntity );
            packet.Write(entity.GUID.ToString());

            return packet;
        }
    }
}
