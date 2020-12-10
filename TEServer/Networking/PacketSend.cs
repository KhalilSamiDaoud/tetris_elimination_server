using System.Collections.Generic;
using TEServer.Includes;

namespace TEServer
{
    public class PacketSend
    {
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

        public static void PlayerReadyChange(int lobbyID, int clientID, int msg)
        {
            using (Packet packet = new Packet((int)ServerPackets.playerReadyChange))
            {
                packet.Write(msg);
                packet.Write(clientID);
                SendTCPDataToLobbyEx(lobbyID, clientID, packet);
            }
        }

        public static void PlayerCountChange(int clientID)
        {
            using (Packet packet = new Packet((int)ServerPackets.playerCountChange))
            {
                packet.Write(GameServer.CountConnectedPlayers().ToString() + "/" + GameServer.MaxPlayers.ToString());
                packet.Write(clientID);

                SendTCPDataToAllEx(clientID, packet);
            }
        }

        public static void PlayerGrids(int lobbyID, int clientID, string msg)
        {
            using (Packet packet = new Packet((int)ServerPackets.playerGrids))
            {
                packet.Write(msg);
                packet.Write(clientID);

                SendTCPDataToLobbyEx(lobbyID, clientID, packet);
            }
        }

        public static void PlayerScore(int lobbyID, int clientID, int score)
        {
            using (Packet packet = new Packet((int)ServerPackets.playerScore))
            {
                packet.Write(score);
                packet.Write(clientID);

                SendTCPDataToLobbyEx(lobbyID, clientID, packet);
            }
        }

        public static void PlayerGameOver(int lobbyID, int clientID, bool gameOver)
        {
            using (Packet packet = new Packet((int)ServerPackets.playerGameOver))
            {
                packet.Write(gameOver);
                packet.Write(clientID);

                SendTCPDataToLobbyEx(lobbyID, clientID, packet);
            }
        }

        public static void StartGame(int lobbyID)
        {
            using (Packet packet = new Packet((int)ServerPackets.startGame))
            {
                packet.Write(true);

                SendTCPDataToLobby(lobbyID, packet);
            }
        }

        public static void ServerDisconnect()
        {
            using (Packet packet = new Packet((int)ServerPackets.serverDisconnect))
            {
                packet.Write(true);

                SendTCPDataToAll(packet);
            }
        }

        private static void SendTCPData(int clientID, Packet packet)
        {
            packet.WriteLength();
            GameServer.connectedClients[clientID].tcp.SendData(packet);
        }

        private static void SendTCPDataToAll(Packet packet)
        {
            packet.WriteLength();
            for (int i = 1; i <= GameServer.MaxPlayers; i++)
            {
                GameServer.connectedClients[i].tcp.SendData(packet);
            }
        }

        private static void SendTCPDataToAllEx(int clientID, Packet packet)
        {
            packet.WriteLength();
            for (int i = 1; i <= GameServer.MaxPlayers; i++)
            {
                if (i != clientID)
                GameServer.connectedClients[i].tcp.SendData(packet);
            }
        }

        private static void SendTCPDataToLobby(int lobbyID, Packet packet)
        {
            var lobby = GameServer.openLobbies[lobbyID];

            packet.WriteLength();

            for (int i = 0; i < lobby.PlayerCount; i++)
            {
                lobby.GetPlayerList()[i].tcp.SendData(packet);
            }
        }

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
