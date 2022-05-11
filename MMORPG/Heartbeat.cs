using System;

using Swordfish.Library.Networking;
using Swordfish.MMORPG.Client;
using Swordfish.MMORPG.Data;
using Swordfish.MMORPG.Packets;
using Swordfish.MMORPG.Server;
using Swordfish.Threading;

namespace Swordfish.MMORPG
{
    public class Heartbeat
    {
        public const int TICK_RATE = 15;
        public static GameClient Client;
        public static GameServer Server;

        private ThreadWorker thread;

        public void Initialize()
        {
            thread = new ThreadWorker(Tick, false, "Heartbeat");
            thread.TargetTickRate = TICK_RATE;

            Start();
            thread.Start();
        }

        public void Start()
        {
            PacketManager.Initialize();
            Server = new GameServer();
            Client = new GameClient();
        }

        public void Stop()
        {
            thread.Stop();
        }

        public void Tick(float deltaTime)
        {
            foreach (LivingEntity character in Server.Characters.Values)
            {
                Server.Broadcast(new EntityPacket {
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
                });
            }

            UpdateTitle();
        }

        private void UpdateTitle()
        {
            Console.Title = $"MMORPG Server | {Server?.Session.EndPoint} | {TICK_RATE}/s | {Server?.SessionCount}/{Server?.MaxSessions}";
        }
    }
}
