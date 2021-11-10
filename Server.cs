using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;

namespace mmorpg_server
{
    public class Server
    {
        public static int MaxPlayers;
        public static int Port;

        private static int playerCount = 0;
        private static Client[] clients;
        private static TcpListener tcpListener;

        private static List<Entity> entities;

        public delegate void PacketHandler(int clientID, ByteMessage packet);
        private static Dictionary<int, PacketHandler> packetHandlers;
        private static ConcurrentQueue<ByteMessage> packets = new ConcurrentQueue<ByteMessage>();

        public static PacketHandler GetPacketHandler(int id) => packetHandlers.ContainsKey(id) ? packetHandlers[id] : null;

        public static Client[] GetClients() => clients;
        public static List<Entity> GetEntities() => entities;
        public static string GetAddress() => tcpListener.Server.LocalEndPoint.ToString();
        public static int GetPlayerCount() => playerCount;

        public static void Start(int maxPlayers, int port)
        {
            MaxPlayers = maxPlayers;
            Port = port;

            Initialize();
            OpenConnection();

            Console.WriteLine($"Server started at {tcpListener.Server.LocalEndPoint}");
        }

        public static void Tick()
        {
            playerCount = 0;

            for (int i = 0; i < MaxPlayers; i++)
            {
                //  Count online players
                if (clients[i].Tcp.IsConnected())
                    playerCount++;
                else if (clients[i].Tcp.TimedOut())
                    clients[i].Tcp.Disconnect();
            }

            //  Send entity updates to all players
            foreach (Entity e in entities)
            {
                PacketSender.BroadcastEntityPosition(e);
            }
        }

        static void Initialize()
        {
            clients = new Client[MaxPlayers];

            for (int i = 0; i < MaxPlayers; i++)
                clients[i] = new Client(i);

            entities = new List<Entity>();

            packetHandlers = new Dictionary<int, PacketHandler>()
            {
                {(int) Packets.Handshake, PacketReader.Handshake},

                {(int) Packets.RegisterAttempt, PacketReader.RegisterAttempt},
                {(int) Packets.LoginAttempt, PacketReader.LoginAttempt},
                {(int) Packets.JoinAttempt, PacketReader.JoinAttempt},

                {(int) Packets.PlayerPosition, PacketReader.PlayerPosition},
            };
        }

        static void OpenConnection()
        {
            tcpListener = new TcpListener(IPAddress.Any, Port);
            tcpListener.Start();
            tcpListener.BeginAcceptTcpClient(new AsyncCallback(OnTcpConnect), null);
        }

        static void OnTcpConnect(IAsyncResult result)
        {
            TcpClient client = tcpListener.EndAcceptTcpClient(result);
            tcpListener.BeginAcceptTcpClient(new AsyncCallback(OnTcpConnect), null);
            Console.WriteLine($"Connection from {client.Client.RemoteEndPoint}...");

            //  Find an open spot and connect
            for (int i = 0; i < MaxPlayers; i++)
            {
                if (clients[i].Tcp.IsConnected() == false)
                {
                    clients[i].ID = i;
                    clients[i].Connect(client);
                    return;
                }
            }

            //  Server is full / connection is declined
            Console.WriteLine($"    Connection failed, server is full!");
        }
    }
}
