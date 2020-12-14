using System.Collections.Generic;
using TEServer.Includes;

namespace TEServer
{
    ///<summary>The PacketSend class is reponsible for assembling packets and sending them to the target client / clients.</summary>
    public class PacketSend
    {
        /// <summary>Verifies the player and initiates the handshake.</summary>
        /// <param name="clientID">The client identifier.</param>
        public static void WelcomeVerification(int clientID)
        {
            using (Packet packet = new Packet((int)ServerPackets.welcome))
            {
                string msg = "ONLINE-" + GameServer.CountConnectedPlayers().ToString() + "/" + GameServer.MaxPlayers.ToString();

                packet.Write(msg);
                packet.Write(clientID);

                SendTCPData(clientID, packet);
            }
        }

        /// <summary>Sends a list of players to the specified lobby.</summary>
        /// <param name="lobbyID">The lobby identifier.</param>
        public static void PlayerList(int lobbyID)
        {
            var lobby = GameServer.openLobbies[lobbyID];

            for (int i = 0; i < lobby.PlayerCount; i++)
            {
                if (lobby.GetPlayerList()[i] != null)
                {
                    int playerID = lobby.GetPlayerList()[i].UID;
                    var player = GameServer.connectedClients[playerID];

                    using (Packet packet = new Packet((int)ServerPackets.playerList))
                    {
                        packet.Write(player.Status);
                        packet.Write(player.UserName);
                        packet.Write(player.UID);

                        SendTCPDataToLobby(lobbyID, packet);
                        }
                    }
                }
        }


        /// <summary>Sends the lobby list to all players connected to the server.</summary>
        public static void LobbyList()
        {

            foreach (KeyValuePair<int, GameLobbyInstance> x in GameServer.openLobbies)
            {
                using (Packet packet = new Packet((int)ServerPackets.lobbyList))
                {
                    var lobby = x.Value;

                    packet.Write(lobby.UID);
                    packet.Write(lobby.Name);
                    packet.Write(lobby.PlayerCount);
                    packet.Write(lobby.IsFull);
                    packet.Write(GameServer.MaxPlayers / 4);

                    SendTCPDataToAll(packet);
                }
            }
        }


        /// <summary>Sends the new status of a client to all other clients in the lobby.</summary>
        /// <param name="lobbyID">The lobby identifier.</param>
        /// <param name="clientID">The client identifier.</param>
        /// <param name="msg">The new status of the player.</param>
        public static void PlayerReadyChange(int lobbyID, int clientID, int msg)
        {
            using (Packet packet = new Packet((int)ServerPackets.playerReadyChange))
            {
                packet.Write(msg);
                packet.Write(clientID);
                SendTCPDataToLobbyEx(lobbyID, clientID, packet);
            }
        }

        /// <summary>Sends the new player count to all clients in the server except the specified client.</summary>
        /// <param name="clientID">The client identifier.</param>
        public static void PlayerCountChange(int clientID)
        {
            using (Packet packet = new Packet((int)ServerPackets.playerCountChange))
            {
                packet.Write(GameServer.CountConnectedPlayers().ToString() + "/" + GameServer.MaxPlayers.ToString());
                packet.Write(clientID);

                SendTCPDataToAllEx(clientID, packet);
            }
        }


        /// <summary>Sends the specified players encoded grid to all other clients in the lobby.</summary>
        /// <param name="lobbyID">The lobby identifier.</param>
        /// <param name="clientID">The client identifier.</param>
        /// <param name="msg">The encoded grid data.</param>
        public static void PlayerGrid(int lobbyID, int clientID, string msg)
        {
            using (Packet packet = new Packet((int)ServerPackets.playerGrids))
            {
                packet.Write(msg);
                packet.Write(clientID);

                SendTCPDataToLobbyEx(lobbyID, clientID, packet);
            }
        }


        /// <summary>Sends the specified players score to all other clients in the lobby.</summary>
        /// <param name="lobbyID">The lobby identifier.</param>
        /// <param name="clientID">The client identifier.</param>
        /// <param name="score">The score.</param>
        public static void PlayerScore(int lobbyID, int clientID, int score)
        {
            using (Packet packet = new Packet((int)ServerPackets.playerScore))
            {
                packet.Write(score);
                packet.Write(clientID);

                SendTCPDataToLobbyEx(lobbyID, clientID, packet);
            }
        }

        /// <summary>Sends the specified players "gameover" to all other clients in the lobby.</summary>
        /// <param name="lobbyID">The lobby identifier.</param>
        /// <param name="clientID">The client identifier.</param>
        /// <param name="gameOver">if set to <c>true</c> [game over].</param>
        public static void PlayerGameOver(int lobbyID, int clientID, bool gameOver)
        {
            using (Packet packet = new Packet((int)ServerPackets.playerGameOver))
            {
                packet.Write(gameOver);
                packet.Write(clientID);

                SendTCPDataToLobbyEx(lobbyID, clientID, packet);
            }
        }


        /// <summary>Sends a StartGame packet to all clients in a lobby.</summary>
        /// <param name="lobbyID">The lobby identifier.</param>
        public static void StartGame(int lobbyID)
        {
            using (Packet packet = new Packet((int)ServerPackets.startGame))
            {
                packet.Write(true);

                SendTCPDataToLobby(lobbyID, packet);
            }
        }


        /// <summary>Kicks a player from the server (sends them a disconnect request).</summary>
        /// <param name="clientID">The client identifier.</param>
        public static void ServerKick(int clientID)
        {
            using (Packet packet = new Packet((int)ServerPackets.serverDisconnect))
            {
                packet.Write(true);

                SendTCPData(clientID, packet);
            }
        }


        /// <summary>Disconnects all clients from the server.</summary>
        public static void ServerDisconnect()
        {
            using (Packet packet = new Packet((int)ServerPackets.serverDisconnect))
            {
                packet.Write(true);

                SendTCPDataToAll(packet);
            }
        }

        /// <summary>Sends the TCP data to a single client.</summary>
        /// <param name="clientID">The client identifier.</param>
        /// <param name="packet">The packet.</param>
        private static void SendTCPData(int clientID, Packet packet)
        {
            packet.WriteLength();
            GameServer.connectedClients[clientID].tcp.SendData(packet);
        }

        /// <summary>Sends the TCP data to all clients.</summary>
        /// <param name="packet">The packet.</param>
        private static void SendTCPDataToAll(Packet packet)
        {
            packet.WriteLength();
            for (int i = 1; i <= GameServer.MaxPlayers; i++)
            {
                GameServer.connectedClients[i].tcp.SendData(packet);
            }
        }

        /// <summary>Sends the TCP data to all clients except the specified client.</summary>
        /// <param name="clientID">The client identifier.</param>
        /// <param name="packet">The packet.</param>
        private static void SendTCPDataToAllEx(int clientID, Packet packet)
        {
            packet.WriteLength();
            for (int i = 1; i <= GameServer.MaxPlayers; i++)
            {
                if (i != clientID)
                GameServer.connectedClients[i].tcp.SendData(packet);
            }
        }

        /// <summary>Sends the TCP data to a single lobby.</summary>
        /// <param name="lobbyID">The lobby identifier.</param>
        /// <param name="packet">The packet.</param>
        private static void SendTCPDataToLobby(int lobbyID, Packet packet)
        {
            var lobby = GameServer.openLobbies[lobbyID];

            packet.WriteLength();

            for (int i = 0; i < lobby.PlayerCount; i++)
            {
                lobby.GetPlayerList()[i].tcp.SendData(packet);
            }
        }

        /// <summary>Sends the TCP data to a sinle lobby except for the specified client.</summary>
        /// <param name="lobbyID">The lobby identifier.</param>
        /// <param name="clientID">The client identifier.</param>
        /// <param name="packet">The packet.</param>
        private static void SendTCPDataToLobbyEx(int lobbyID, int clientID, Packet packet)
        {
            var lobby = GameServer.openLobbies[lobbyID];

            packet.WriteLength();
            
            for (int i = 0; i < lobby.PlayerCount; i++)
            {
                if(clientID !=lobby.GetPlayerList()[i].UID)
                lobby.GetPlayerList()[i].tcp.SendData(packet);
            }
        }
    }
}
