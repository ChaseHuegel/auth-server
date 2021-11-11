using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;

namespace mmorpg_server
{
    public class TcpConnection
    {
        public const int DATA_BUFFER_SIZE = 4096;

        public TcpClient Socket;

        private Stopwatch lastCommunication = new Stopwatch();
        private int id;
        private NetworkStream stream;
        private byte[] receiveBuffer;
        private Packet receivePacket;
        private int receivePacketSize;

        public TcpConnection(int id)
        {
            this.id = id;
        }

        private bool VerifyClient(int target) => (id == target);

        public bool TimedOut() => WasConnected() && IsTimedOut();
        public bool IsTimedOut() => lastCommunication.Elapsed.Seconds > Program.TIMEOUT;
        public bool IsConnected() => WasConnected() && !IsTimedOut();

        public bool WasConnected()
        {
            return (
                    Socket != null &&
                    Socket.Client != null &&
                    Socket.Connected
                    );
        }

        public void Connect(TcpClient socket)
        {
            this.Socket = socket;

            Socket.ReceiveBufferSize = DATA_BUFFER_SIZE;
            Socket.SendBufferSize = DATA_BUFFER_SIZE;

            //  Start reading data
            stream = Socket.GetStream();
            receiveBuffer = new byte[DATA_BUFFER_SIZE];
            receivePacket = new Packet();

            stream.BeginRead(receiveBuffer, 0, DATA_BUFFER_SIZE, OnReceive, null);

            lastCommunication.Restart();

            PacketSender.Handshake(id, "Connection accepted.");

            Console.WriteLine($"{Socket.Client.RemoteEndPoint} connected");
        }

        public void Disconnect()
        {
            if (Socket != null)
            {
                string timeOut = TimedOut() ? " (Timed out)" : "";
                Console.WriteLine($"{Socket.Client.RemoteEndPoint} disconnected{timeOut}");
                Socket.Close();
            }
            else
            {
                Console.WriteLine($"Client [{id}] was lost or disconnected");
            }

            stream = null;
            receiveBuffer = null;
            receivePacket = null;
            Socket = null;
        }

        public void SendData(byte[] data)
        {
            if (!IsConnected()) return;

            try
            {
                stream.BeginWrite(data, 0, data.Length, null, null);
            }
            catch (Exception e)
            {
                Console.WriteLine($"OnReceive error: {e}");
            }
        }

        void OnReceive(IAsyncResult result)
        {
            if (!IsConnected()) return;

            try
            {
                //  Reset the stopwatch for timing out
                lastCommunication.Restart();

                int length = stream.EndRead(result);

                if (length <= 0)
                {
                    Server.GetClients()[id].Disconnect();
                    return;
                }

                //  Grab the incoming data
                byte[] data = new byte[DATA_BUFFER_SIZE];
                Array.Copy(receiveBuffer, data, length);

                receivePacket.Append(data);

                //  If we are waiting for a packet, get the next packet's size in bytes
                if (receivePacket.Length >= 4 && receivePacketSize == 0)
                    receivePacketSize = receivePacket.ResetReader().ReadInt();

                //  Read the packet if it is complete
                if (receivePacket.Length >= receivePacketSize)
                {
                    //  Pull the packet from the received packet buffer
                    Packet packet = receivePacket.Grab(4, receivePacketSize);

                    int packetID = packet.ReadInt();
                    int senderID = packet.ReadInt();

                    if (VerifyClient(senderID))
                    {
                        Server.PacketHandler handler = Server.GetPacketHandler(packetID);
                        if (handler != null)
                            handler.Invoke(id, packet);
                        else
                            Console.WriteLine($"Received unknown packet [{packetID}] from client [{senderID}]");
                    }
                    else
                    {
                        Console.WriteLine($"Unable to verify packet! Receiver ID [{id}] mismatch sender ID [{senderID}]. Discarding packet.");
                    }

                    //  Reset to begin collecting the next packet
                    receivePacket.Reset();
                    receivePacketSize = 0;
                }

                //  Continue reading data
                stream.BeginRead(receiveBuffer, 0, DATA_BUFFER_SIZE, OnReceive, null);
            }
            catch (Exception e)
            {
                Console.WriteLine($"OnReceive error: {e}");
                Server.GetClients()[id].Disconnect();
            }
        }
    }
}
