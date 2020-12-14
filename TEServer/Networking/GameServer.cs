using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System;

using TEServer.Includes;

namespace TEServer
{

    /// <summary>The GameServer class contains packethandlers, and initializez the player list. This class is responsible for managing both the lobby
    /// and player lists. This class contains the tcpListener which handles incoming connections.</summary>
    public class GameServer
    {
        public delegate void PacketHandler(int fromClient, Packet packet);

        public static Dictionary<int, GameClientInstance> connectedClients;
        public static Dictionary<int, GameLobbyInstance> openLobbies;
        public static Dictionary<int, PacketHandler> packetHandlers;
        public static TcpListener tcpListener;

        /// <summary>Gets the maximum players.</summary>
        /// <value>The maximum players.</value>
        public static int MaxPlayers { get; private set; }

        /// <summary>Gets the port.</summary>
        /// <value>The port.</value>
        public static int Port { get; private set; }


        /// <summary>Initializes the server by setting up the lobby and player dictionaries, and calls InitializeServerData() and 
        /// GetLocalIPAddress. This function also initializes private memebers.</summary>
        /// <param name="userMaxPlayers">The maximum players allowed to join.</param>
        /// <param name="userPort">The user port.</param>
        public static void InitializeServer(int userMaxPlayers, int userPort)
        {
            connectedClients = new Dictionary<int, GameClientInstance>();
            openLobbies      = new Dictionary<int, GameLobbyInstance>();
            MaxPlayers       = userMaxPlayers;
            Port             = userPort;
            InitializeServerData();

            tcpListener = new TcpListener(GetLocalIPAddress(), Port);
            tcpListener.Start();

            Console.WriteLine(Constants.START_ATTEMPT);

            tcpListener.BeginAcceptTcpClient(new AsyncCallback(ConnectCallBack), null);
            Console.WriteLine(Constants.START_SUCCESS + GetLocalIPAddress() + ":" + Port);
        }


        /// <summary>Gets the local ip address by querying the local DNS registry.</summary>
        /// <returns>The users local IP address.</returns>
        /// <exception cref="Exception">No network adapters with an IPv4 address in the system!</exception>
        /// <remarks>Credit to: Mrchief from stack overflow</remarks>
        public static IPAddress GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip;
                }
            }

            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        /// <summary>Initializes the server data and attaches ServerHandle methods to ClientPacket Enum values. This function also
        /// "eagerly" initializes the player list with GameClientInstance's</summary>
        private static void InitializeServerData()
        {
            packetHandlers = new Dictionary<int, PacketHandler>()
            {
                { (int)ClientPackets.welcomeReceived, ServerHandle.Welcome},
                { (int)ClientPackets.clientStatus, ServerHandle.ClientStatus},
                { (int)ClientPackets.clientGrid, ServerHandle.ClientGrid},
                { (int)ClientPackets.clientScore, ServerHandle.ClientScore},
                { (int)ClientPackets.clientGameOver, ServerHandle.ClientGameOver},
                { (int)ClientPackets.clientReconnect, ServerHandle.ClientReconnect},
                { (int)ClientPackets.clientLobbyCreate, ServerHandle.ClientLobbyCreate},
                { (int)ClientPackets.clientLobbyJoin, ServerHandle.ClientLobbyJoin}
            };


            for ( int i=1; i <= MaxPlayers; i++)
            {
                connectedClients.Add(i, new GameClientInstance(i));
            }
        }

        /// <summary>Attemps to verify a clients connection attemp. If the connectedClients list is not full of initialized TCP instances,
        /// then add the client and initialize their TCP instance. If it is full, reject the clients connection.</summary>
        /// <param name="result">The result of the Async operation.</param>
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

        /// <summary>Counts the connected players. This is done by checking if the TCP instance of a client is null.</summary>
        /// <returns> The amount of connected players.</returns>
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

        /// <summary>Counts the ready players in a lobby. This is done by checking if the Status member of a client is equal to 1.</summary>
        /// <param name="lobbyID">The lobby identifier.</param>
        /// <returns> The amount of ready players in a lobby.</returns>
        public static int CountReadyPlayers(int lobbyID)
        {
            int count = 0;

            for (int i = 0; i < openLobbies[lobbyID].PlayerCount; i++)
            {
                var player = openLobbies[lobbyID].GetPlayerList()[i];

                if (player.tcp.socket != null)
                {
                    if (player.Status == 1)
                    {
                        count++;
                    }
                }
            }

            return count;
        }
    }
}
