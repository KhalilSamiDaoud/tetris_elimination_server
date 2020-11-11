﻿using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System;

namespace TEServer
{
    public class GameServer
    {
        public static TcpListener tcpListener;
        public delegate void PacketHandler(int fromClient, Packet packet);
        public static Dictionary<int, GameClientInstance> connectedClients;
        public static Dictionary<int, PacketHandler> packetHandlers;

        public static int MaxPlayers { get; private set; }
        public static int Port { get; private set; }

        public static void InitializeServer(int userMaxPlayers, int userPort)
        {
            connectedClients = new Dictionary<int, GameClientInstance>();
            MaxPlayers       = userMaxPlayers;
            Port             = userPort;
            InitializeServerData();

            tcpListener = new TcpListener(IPAddress.Any, Port);
            tcpListener.Start();

            Console.WriteLine(Constants.START_ATTEMPT);

            tcpListener.BeginAcceptTcpClient(new AsyncCallback(ConnectCallBack), null);
            Console.WriteLine(Constants.START_SUCCESS + Port);
        }

        private static void InitializeServerData()
        {
            packetHandlers = new Dictionary<int, PacketHandler>()
            {
                { (int)ClientPackets.welcomeReceived, ServerHandle.Welcome},
                { (int)ClientPackets.clientReady, ServerHandle.ClientReady}
            };


            for ( int i=1; i <= MaxPlayers; i++)
            {
                connectedClients.Add(i, new GameClientInstance(i));
            }
        }

        private static void ConnectCallBack(IAsyncResult result)
        {
            TcpClient client = tcpListener.EndAcceptTcpClient(result);
            tcpListener.BeginAcceptTcpClient(new AsyncCallback(ConnectCallBack), null);

            Console.WriteLine(Constants.CONNECTION_ATTEMPT + client.Client.RemoteEndPoint);

            for (int i=1; i <= MaxPlayers; i++)
            {
                if (connectedClients[i].tcp.socket == null)
                {
                    connectedClients[i].tcp.Connect(client);
                    Console.WriteLine(Constants.USER_CONNECTED + connectedClients[i].tcp.ID);
                    return;
                }
            }

            Console.WriteLine(Constants.SERVER_FULL_ERROR);
        }

        public static int CountConnectedPlayers()
        {
            int count = 0;

            for (int i = 1; i <= MaxPlayers; i++)
            {
                if (connectedClients[i].tcp.socket != null)
                {
                    count++;
                }
            }

            return count;
        }
    }
}