using System.Net.Sockets;
using System;

namespace TEServer
{
    /// <summary> The TCP class manages tcp connections between the server and the client. The server only handles the TCP protocol.</summary>
    public class TCP
    {
        public TcpClient socket;

        private NetworkStream byteStream;
        private readonly int bufferSize;
        private byte[] receiveBuffer;
        private Packet dataIn;

        public int ID { get; private set; }

        /// <summary>Initializes a new instance of the <see cref="TCP" /> class.</summary>
        /// <param name="uniqueID">The unique identifier.</param>
        public TCP(int uniqueID)
        {
            bufferSize = 4096; //bytes
            ID         = uniqueID;
        }

        /// <summary>Connects the specified client to the server. Initializes the receive buffer and socket.</summary>
        /// <param name="clientsSocket">The clients socket.</param>
        /// <remarks>This function sends a "WelcomeVerification" packet.</remarks>
        public void Connect(TcpClient clientsSocket)
        {
            socket = clientsSocket;
            socket.ReceiveBufferSize = bufferSize;
            socket.SendBufferSize = bufferSize;

            byteStream = socket.GetStream();

            dataIn        = new Packet();
            receiveBuffer = new byte[bufferSize];

            byteStream.BeginRead(receiveBuffer, 0, bufferSize, ReceiveCallback, null);

            PacketSend.WelcomeVerification(ID);
        }

        /// <summary>Writes the packet to the bytestream and sends it to the client.</summary>
        /// <param name="packet">The packet to send.</param>
        public void SendData(Packet packet)
        {
            try
            {
                if (socket != null)
                {
                    byteStream.BeginWrite(packet.ToArray(), 0, packet.Length(), null, null);
                }
            }
            catch (Exception)
            {
                Console.WriteLine(Constants.TCP_SEND_ERROR + ID);
            }
        }

        /// <summary>Receives client packets and reads the data from it. If there are any errors with the packet, then the client
        /// is disconnected from the server and an error message is displayed (This is unlikely with TCP).</summary>
        /// <param name="result">The Async result.</param>
        private void ReceiveCallback(IAsyncResult result)
        {
            try
            {
                int byteLength = byteStream.EndRead(result);
                if (byteLength <= 0)
                {
                    GameServer.connectedClients[ID].Disconnect();
                    return;
                }

                byte[] receivedData = new byte[byteLength];
                Array.Copy(receiveBuffer, receivedData, byteLength);

                dataIn.Reset(HandleData(receivedData));
                byteStream.BeginRead(receiveBuffer, 0, bufferSize, ReceiveCallback, null);


            }
            catch (Exception)
            {
                Console.WriteLine(Constants.RECEIVE_CALLBACK_ERROR);
                GameServer.connectedClients[ID].Disconnect();
            }
        }

        /// <summary>Puts received bytes in the datain array. If the packet length is greater than 4, then there is still unread data.
        /// if it is less than or equal to 4, then the packet has reached its end.</summary>
        /// <param name="receivedData">The received data.</param>
        /// <returns>True if the packet is fully read</returns>
        private bool HandleData(byte[] receivedData)
        {
            int packetLength = 0;

            dataIn.SetBytes(receivedData);

            if (dataIn.UnreadLength() >= 4)
            {
                packetLength = dataIn.ReadInt();
                if (packetLength <= 0)
                {
                    return true;
                }
            }

            while (packetLength > 0 && packetLength <= dataIn.UnreadLength())
            {
                byte[] packetBytes = dataIn.ReadBytes(packetLength);

                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (Packet packet = new Packet(packetBytes))
                    {
                        int packetID = packet.ReadInt();
                        GameServer.packetHandlers[packetID](ID, packet);
                    }
                });

                packetLength = 0;

                if (dataIn.UnreadLength() >= 4)
                {
                    packetLength = dataIn.ReadInt();
                    if (packetLength <= 0)
                    {
                        return true;
                    }
                }
            }

            if (packetLength <= 1)
            {
                return true;
            }

            return false;
        }

        /// <summary>Disconnects the client, and resets the state of this TCP instance.</summary>
        public void Disconnect()
        {
            socket.Close();
            byteStream    = null;
            receiveBuffer = null;
            dataIn        = null;
            socket        = null;
        }
    }
}
