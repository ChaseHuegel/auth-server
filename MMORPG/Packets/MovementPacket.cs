using System;
using Swordfish.Library.Networking;
using Swordfish.Library.Networking.Attributes;
using Swordfish.Library.Networking.Interfaces;
using Swordfish.Library.Types;
using Swordfish.MMORPG.Client;
using Swordfish.MMORPG.Data;
using Swordfish.MMORPG.Server;

namespace Swordfish.MMORPG.Packets
{
    [Packet(RequiresSession = true)]
    public struct MovementPacket : ISerializedPacket
    {
        public int ID;
        public float Heading;
        public float Direction;
        public MultiBool State;

        [ServerPacketHandler]
        public static void OnMovementServer(NetServer server, MovementPacket packet, NetEventArgs e)
        {
            //  Update the entity if it exists
            if (GameServer.Instance.Characters.TryGetValue(packet.ID, out LivingEntity character))
            {
                character.Heading = packet.Heading;
                character.Direction = packet.Direction;
                character.Jumped = packet.State[0];
                character.Moving = packet.State[1];

                EntityPacket snapshot = new EntityPacket {
                    ID = character.ID,
                    X = character.X,
                    Y = character.Y,
                    Z = character.Z,
                    Heading = character.Heading,
                    Speed = character.Speed,
                    Direction = character.Direction,
                    State = {
                        [0] = character.Jumped,
                        [1] = character.Moving
                    }
                };

                server.BroadcastExcept(snapshot, e.Session);
            }
        }

        [ClientPacketHandler]
        public static void OnMovementClient(NetClient client, MovementPacket packet, NetEventArgs e)
        {
            //  Update the entity if it exists
            if (GameClient.Instance.Characters.TryGetValue(packet.ID, out LivingEntity character))
            {
                character.Heading = packet.Heading;
                character.Direction = packet.Direction;
                character.Jumped = packet.State[0];
                character.Moving = packet.State[1];
            }
        }
    }
}
