using System;

namespace mmorpg_server
{
    public class PacketSender
    {
        static void Send(int clientID, ByteMessage packet)
        {
            Server.GetClients()[clientID].Tcp.SendData(packet.GetMessage());
        }

        static void SendAll(ByteMessage packet)
        {
            for (int i = 0; i < Server.MaxPlayers; i++)
                Server.GetClients()[i].Tcp.SendData(packet.GetMessage());
        }

        static void SendAllExcept(int exceptionID, ByteMessage packet)
        {
            for (int i = 0; i < Server.MaxPlayers; i++)
                if (i != exceptionID)
                    Server.GetClients()[i].Tcp.SendData(packet.GetMessage());
        }

        public static void Handshake(int clientID, string msg)
        {
            ByteMessage packet = new ByteMessage();

            packet.Write( (int) Packets.Handshake );
            packet.Write(clientID);
            packet.Write(msg);

            Send(clientID, packet);
        }

        public static void RegisterSuccess(int clientID)
        {
            ByteMessage packet = new ByteMessage();

            packet.Write( (int) Packets.RegisterSuccess );

            Send(clientID, packet);
        }

        public static void RegisterFailed(int clientID, RegisterErrorCode error)
        {
            ByteMessage packet = new ByteMessage();

            packet.Write( (int) Packets.RegisterFailed );
            packet.Write((int)error);

            Send(clientID, packet);
        }

        public static void LoginSuccess(int clientID, string username)
        {
            ByteMessage packet = new ByteMessage();

            packet.Write( (int) Packets.LoginSuccess );
            packet.Write(Database.GetCharacterGuid(username).ToString());

            Send(clientID, packet);
        }

        public static void LoginFailed(int clientID, LoginErrorCode error)
        {
            ByteMessage packet = new ByteMessage();

            packet.Write( (int) Packets.LoginFailed );
            packet.Write((int)error);

            Send(clientID, packet);
        }

        public static void JoinSuccess(int clientID, Guid guid)
        {
            ByteMessage packet = new ByteMessage();

            packet.Write( (int) Packets.JoinSuccess );
            packet.Write(guid.ToString());

            Send(clientID, packet);
        }

        public static void JoinFailed(int clientID, JoinErrorCode error)
        {
            ByteMessage packet = new ByteMessage();

            packet.Write( (int) Packets.JoinFailed );
            packet.Write((int)error);

            Send(clientID, packet);
        }

        public static void EntityPosition(int clientID, Entity entity)                  => Send(clientID, EntityPositionPacket(entity));
        public static void BroadcastEntityPosition(Entity entity)                       => SendAll(EntityPositionPacket(entity));
        public static void BroadcastEntityPositionEx(int exceptionID, Entity entity)    => SendAllExcept(exceptionID, EntityPositionPacket(entity));

        private static ByteMessage EntityPositionPacket(Entity entity)
        {
            ByteMessage packet = new ByteMessage();

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

        private static ByteMessage SpawnEntityPacket(Entity entity)
        {
            ByteMessage packet = new ByteMessage();

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

        private static ByteMessage RemoveEntityPacket(Entity entity)
        {
            ByteMessage packet = new ByteMessage();

            packet.Write( (int) Packets.RemoveEntity );
            packet.Write(entity.GUID.ToString());

            return packet;
        }
    }
}
