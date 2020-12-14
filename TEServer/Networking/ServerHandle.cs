using TEServer.Includes;
using System;

namespace TEServer
{
    /// <summary>This class handles all incoming packet types from clients. Packets are usually send in lresponse to incoming packets.</summary>
    class ServerHandle
    {

        /// <summary>Welcomes the specified client. The client receives all the nessesary server information. Informs other
        /// clients that the player count has changed</summary>
        /// <param name="clientID">The client identifier.</param>
        /// <param name="packet">The incoming packet.</param>
        /// <remarks>This function sends a "PlayerCountChange" packet.</remarks>
        /// <remarks>This function sends a "LobbyList" packet.</remarks>
        public static void Welcome(int clientID, Packet packet)
        {
            int id     = packet.ReadInt();
            string msg = packet.ReadString();

            Console.WriteLine(Constants.HANDSHAKE_COMPLETE + msg);
            GameServer.connectedClients[id].UserName = msg;

            PacketSend.PlayerCountChange(id);
            PacketSend.LobbyList();

            ValidatePlayerID(clientID, id);
        }

        /// <summary>Receives a clients new status, all other clients in the lobby are informed of the new status. This
        /// function calls CheckLobbyReady() when a status changes.</summary>
        /// <param name="clientID">The client identifier.</param>
        /// <param name="packet">The incoming packet.</param>
        /// <remarks>This function sends a "PlayerReadyChange" packet.</remarks>
        public static void ClientStatus(int clientID, Packet packet)
        {
            int id  = packet.ReadInt();
            int msg = packet.ReadInt();

            var player = GameServer.connectedClients[id];

            switch (msg)
            {
                case 0:
                    Console.WriteLine(player.UserName + Constants.NOT_READY);
                    break;
                case 1:
                    Console.WriteLine(player.UserName + Constants.READY);
                    break;
                case 2:
                    Console.WriteLine(player.UserName + Constants.IN_GAME);
                    break;
            }

            player.Status = msg;
            PacketSend.PlayerReadyChange(player.LobbyID, id, msg);

            CheckLobbyReady(player);
            ValidatePlayerID(clientID, id);
        }

        /// <summary>Receives a clients encoded grid. All other clients in the lobby are notified of this.</summary>
        /// <param name="clientID">The client identifier.</param>
        /// <param name="packet">The incoming packet.</param>
        /// <remarks>This function sends a "PlayerGrid" packet.</remarks>
        public static void ClientGrid(int clientID, Packet packet)
        {
            int id     = packet.ReadInt();
            string msg = packet.ReadString();

            PacketSend.PlayerGrid(GameServer.connectedClients[id].LobbyID, id, msg);

            ValidatePlayerID(clientID, id);
        }

        /// <summary>Receives a clients score. All other clients in the lobby are notified of this.</summary>
        /// <param name="clientID">The client identifier.</param>
        /// <param name="packet">The incoming packet.</param>
        /// <remarks>This function sends a "PlayerScore" packet.</remarks>
        public static void ClientScore(int clientID, Packet packet)
        {
            int id  = packet.ReadInt();
            int msg = packet.ReadInt();

            PacketSend.PlayerScore(GameServer.connectedClients[id].LobbyID, id, msg);

            ValidatePlayerID(clientID, id);
        }

        /// <summary>Receives a clients 'gameover'. All other clients in the lobby are notified of this.</summary>
        /// <param name="clientID">The client identifier.</param>
        /// <param name="packet">The incoming packet.</param>
        /// <remarks>This function sends a "PlayerGameOver" packet.</remarks>
        public static void ClientGameOver(int clientID, Packet packet)
        {
            int id   = packet.ReadInt();
            bool msg = packet.ReadBool();

            PacketSend.PlayerGameOver(GameServer.connectedClients[id].LobbyID, id, msg);

            ValidatePlayerID(clientID, id);
        }

        /// <summary>Reconnects the client to the server by sending them a new welcome verification.</summary>
        /// <param name="clientID">The client identifier.</param>
        /// <param name="packet">The incoming packet.</param>
        /// <remarks>This function sends a "WelcomeVerification" packet.</remarks>
        public static void ClientReconnect(int clientID, Packet packet)
        {
            int id = packet.ReadInt();

            PacketSend.WelcomeVerification(id);

            ValidatePlayerID(clientID, id);
        }

        /// <summary>Creates a lobby as requested by a client. The lobby will not be created if there are already four lobbies.
        /// If the lobby is created, inform all other clients of the new lobby.</summary>
        /// <param name="clientID">The client identifier.</param>
        /// <param name="packet">The incoming packet.</param>
        /// <remarks>This function sends a "LobbyList" packet.</remarks>
        public static void ClientLobbyCreate(int clientID, Packet packet)
        {
            int id = packet.ReadInt();

            var player    = GameServer.connectedClients[id];
            var lobbyList = GameServer.openLobbies;

            if (lobbyList.Count < 4)
            {
                GameLobbyInstance temp = new GameLobbyInstance(id, player);
                lobbyList.Add(id, temp);

                player.LobbyID = id;

                PacketSend.LobbyList();

                Console.WriteLine(player.UserName + Constants.LOBBY_OPENED);
            }

            ValidatePlayerID(clientID, id);
        }

        /// <summary>Puts a client into the specified lobby (specified in the packet). The client is not connected if the lobby is full.
        /// If the client is connected, update the lobby list and player list for all other clients.</summary>
        /// <param name="clientID">The client identifier.</param>
        /// <param name="packet">The incoming packet.</param>
        /// <remarks>This function sends a "PlayerList" packet.</remarks>
        /// <remarks>This function sends a "LobbyList" packet.</remarks>
        public static void ClientLobbyJoin(int clientID, Packet packet)
        {
            int id      = packet.ReadInt();
            int lobbyID = packet.ReadInt();

            var lobby  = GameServer.openLobbies[lobbyID];
            var player = GameServer.connectedClients[id];

            if (!lobby.IsFull)
            {
                player.LobbyID = lobbyID;
                lobby.AddPlayer(player);

                PacketSend.PlayerList(lobbyID);
                PacketSend.LobbyList();
            }

            ValidatePlayerID(clientID, id);
        }

        /// <summary>Checks if the entire lobby is ready, if it is, then start the game.</summary>
        /// <param name="player">The player.</param>
        /// <remarks>This function sends a "StartGame" packet.</remarks>
        private static void CheckLobbyReady(GameClientInstance player)
        {
            if (GameServer.CountReadyPlayers(player.LobbyID) == (GameServer.MaxPlayers / 4))
            {
                PacketSend.StartGame(player.LobbyID);
                Console.WriteLine(Constants.GAME_STARTED + GameServer.openLobbies[player.LobbyID].Name);
            }
        }

        /// <summary>Validates that the senders ID matches the ID attached to the packet. This is to prevent packet mishandling</summary>
        /// <param name="senderID">The sender identifier.</param>
        /// <param name="packetID">The packet identifier.</param>
        private static void ValidatePlayerID(int senderID, int packetID)
        {
            if (senderID != packetID)
            {
                Console.WriteLine(Constants.INCONSISTENT_ID_ERROR + senderID);
            }
        }
    }
}
