using System;
using System.Text;

namespace mmorpg_server
{
    public class PacketReader
    {
        public static void Handshake(int clientID, ByteMessage packet)
        {
            Console.WriteLine($"Handshake success! Client [{clientID}] says: {packet.ReadString()}");
        }

        public static void RegisterAttempt(int clientID, ByteMessage packet)
        {
            string username = packet.ReadString();
            string password = packet.ReadString();

            //  TODO this is MESSY

            if (username.Length < 4)
                PacketSender.RegisterFailed(clientID, RegisterErrorCode.USERNAME_SHORT);
            else if (username.Length > 16)
                PacketSender.RegisterFailed(clientID, RegisterErrorCode.USERNAME_LONG);
            else if (password.Length < 8)
                PacketSender.RegisterFailed(clientID, RegisterErrorCode.PASSWORD_SHORT);
            else if (password.Length > 24)
                PacketSender.RegisterFailed(clientID, RegisterErrorCode.PASSWORD_LONG);
            else if (password == string.Empty || password.Contains(" "))
                PacketSender.RegisterFailed(clientID, RegisterErrorCode.PASSWORD_CRITERA);
            else if (username == string.Empty || username.Contains(" "))
                PacketSender.RegisterFailed(clientID, RegisterErrorCode.USERNAME_CRITERA);

            else if (!Database.VerifyUsername(username))
            {
                byte[] salt = Database.GenerateSalt(16);
                byte[] hash = Database.GenerateSaltedHash(Encoding.ASCII.GetBytes(password), salt);

                Guid guid = Guid.NewGuid();

                Database.RegisterUser(username, hash, salt, guid);

                Console.WriteLine($"Client [{clientID}] registered new user [{username}]]");

                PacketSender.RegisterSuccess(clientID);
            }
            else
            {
                PacketSender.RegisterFailed(clientID, RegisterErrorCode.USERNAME_UNAVAILABLE);
            }
        }

        public static void LoginAttempt(int clientID, ByteMessage packet)
        {
            string username = packet.ReadString();
            string password = packet.ReadString();

            if (Database.VerifyUsername(username))
            {
                if (Database.VerifyPassword(username, password))
                {
                    Console.WriteLine($"Client [{clientID}] logged in as user [{username}]]");
                    PacketSender.LoginSuccess(clientID, username);
                }
                else
                    PacketSender.LoginFailed(clientID, LoginErrorCode.PASSWORD_INCORRECT);
            }
            else
            {
                PacketSender.LoginFailed(clientID, LoginErrorCode.USERNAME_INCORRECT);
            }
        }

        public static void JoinAttempt(int clientID, ByteMessage packet)
        {
            string guid = packet.ReadString();

            if (Database.VerifyCharacterGuid(guid))
            {
                Console.WriteLine($"Client [{clientID}] joined with guid [{guid}]");

                //  Send all existing entities to the client
                foreach (Entity e in Server.GetEntities())
                    PacketSender.SpawnEntity(clientID, e);

                //  Create server side entity
                Server.GetClients()[clientID].entity = new Entity(Guid.Parse(guid));
                Server.GetEntities().Add(Server.GetClients()[clientID].entity);

                //  Send the new entity to all clients except this one
                PacketSender.BroadcastSpawnEntityEx(clientID, Server.GetClients()[clientID].entity);

                PacketSender.JoinSuccess(clientID, Guid.Parse(guid));
            }
            else
            {
                PacketSender.JoinFailed(clientID, JoinErrorCode.CHARACTER_NOT_FOUND);
            }
        }

        public static void PlayerPosition(int clientID, ByteMessage packet)
        {
            //  ! This is BAD very BAD only for testing purposes
            //  TODO should not use client authed positions! server will eventually take auth over this
            float x = packet.ReadFloat();
            float y = packet.ReadFloat();
            float z = packet.ReadFloat();

            float rotX = packet.ReadFloat();
            float rotY = packet.ReadFloat();

            Entity e = Server.GetClients()[clientID].entity;

            e.x = x;
            e.y = y;
            e.z = z;
            e.rotX = rotX;
            e.rotY = rotY;
        }
    }
}
