﻿using System.Net.Sockets;
using System;

namespace TEServer
{
    public class TCP
    {
        public TcpClient socket;

        private NetworkStream byteStream;
        private Packet dataIn;
        private Byte[] receiveBuffer;
        private readonly int bufferSize;

        public int ID { get; private set; }

        public TCP(int uniqueID)
        {
            bufferSize = 4096; //bytes
            ID         = uniqueID;
        }

        public void Connect(TcpClient clientsSocket)
        {
            socket = clientsSocket;
            socket.ReceiveBufferSize = bufferSize;
            socket.SendBufferSize = bufferSize;

            byteStream = socket.GetStream();

            dataIn        = new Packet();
            receiveBuffer = new byte[bufferSize];

            byteStream.BeginRead(receiveBuffer, 0, bufferSize, ReceiveCallback, null);

            string serverInformation = "ONLINE-" + GameServer.CountConnectedPlayers().ToString() + "/" + GameServer.MaxPlayers.ToString();
            PacketSend.WelcomeVerification(ID, serverInformation);
        }

        public void SendData(Packet packet)
        {
            try
            {
                if (socket != null)
                {
                    byteStream.BeginWrite(packet.ToArray(), 0, packet.Length(), null, null);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(Constants.TCP_SEND_ERROR + ID + ex);
            }
        }

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
            catch (Exception e)
            {
                Console.WriteLine(Constants.RECEIVE_CALLBACK_ERROR + e);
                GameServer.connectedClients[ID].Disconnect();
            }
        }

        private bool HandleData(Byte[] receivedData)
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
                Byte[] packetBytes = dataIn.ReadBytes(packetLength);

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