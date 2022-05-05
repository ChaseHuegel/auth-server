using Swordfish.Library.Networking;
using Swordfish.Library.Networking.Attributes;
using Swordfish.Library.Networking.Interfaces;
using Swordfish.MMORPG.Client;
using Swordfish.MMORPG.Data;
using Swordfish.MMORPG.Server;

namespace Swordfish.MMORPG.Packets
{
    [Packet(RequiresSession = true)]
    public struct EntityPacket : ISerializedPacket
    {
        public int ID;
        public float X;
        public float Y;
        public float Z;
        public float Heading;
        public float Speed;
        public float Direction;
        public bool Jumped;
        public bool Moving;

        [ServerPacketHandler]
        public static void OnEntitySnapshotServer(NetServer server, EntityPacket packet, NetEventArgs e)
        {
            //  Update the entity if it exists; otherwise create it.
            if (GameServer.Instance.Characters.TryGetValue(packet.ID, out LivingEntity character))
            {
                character.X = packet.X;
                character.Y = packet.Y;
                character.Z = packet.Z;
                character.Heading = packet.Heading;
                character.Speed = packet.Speed;
                character.Direction = packet.Direction;
                character.Jumped = packet.Jumped;
                character.Moving = packet.Moving;
            }
            else
            {
                character = new LivingEntity {
                    ID = packet.ID,
                    X = packet.X,
                    Y = packet.Y,
                    Z = packet.Z,
                    Heading = packet.Heading,
                    Speed = packet.Speed,
                    Direction = packet.Direction,
                    Jumped = packet.Jumped,
                    Moving = packet.Moving
                };

                GameServer.Instance.Characters.TryAdd(packet.ID, character);
            }
        }

        [ClientPacketHandler]
        public static void OnEntitySnapshotClient(NetClient client, EntityPacket packet, NetEventArgs e)
        {
            //  Update the entity if it exists; otherwise create it.
            if (GameClient.Instance.Characters.TryGetValue(packet.ID, out LivingEntity character))
            {
                character.X = packet.X;
                character.Y = packet.Y;
                character.Z = packet.Z;
                character.Heading = packet.Heading;
                character.Speed = packet.Speed;
                character.Direction = packet.Direction;
                character.Jumped = packet.Jumped;
                character.Moving = packet.Moving;
            }
            else
            {
                character = new LivingEntity {
                    ID = packet.ID,
                    X = packet.X,
                    Y = packet.Y,
                    Z = packet.Z,
                    Heading = packet.Heading,
                    Speed = packet.Speed,
                    Direction = packet.Direction,
                    Jumped = packet.Jumped,
                    Moving = packet.Moving
                };

                GameClient.Instance.Characters.TryAdd(packet.ID, character);
            }
        }
    }
}
