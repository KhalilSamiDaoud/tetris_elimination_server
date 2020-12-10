using TEServer.Includes;
using System;

namespace TEServer
{
    class ServerHandle
    {
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

            if (GameServer.CountReadyPlayers(player.LobbyID) == (GameServer.MaxPlayers / 4))
            {
                PacketSend.StartGame(player.LobbyID);
                Console.WriteLine(Constants.GAME_STARTED + GameServer.openLobbies[player.LobbyID].Name);
            }

            ValidatePlayerID(clientID, id);
        }

        public static void ClientGrid(int clientID, Packet packet)
        {
            int id     = packet.ReadInt();
            string msg = packet.ReadString();

            PacketSend.PlayerGrids(GameServer.connectedClients[id].LobbyID, id, msg);

            ValidatePlayerID(clientID, id);
        }

        public static void ClientScore(int clientID, Packet packet)
        {
            int id  = packet.ReadInt();
            int msg = packet.ReadInt();

            PacketSend.PlayerScore(GameServer.connectedClients[id].LobbyID, id, msg);

            ValidatePlayerID(clientID, id);
        }

        public static void ClientGameOver(int clientID, Packet packet)
        {
            int id   = packet.ReadInt();
            bool msg = packet.ReadBool();

            PacketSend.PlayerGameOver(GameServer.connectedClients[id].LobbyID, id, msg);

            ValidatePlayerID(clientID, id);
        }

        public static void ClientReconnect(int clientID, Packet packet)
        {
            int id = packet.ReadInt();

            PacketSend.WelcomeVerification(id);

            ValidatePlayerID(clientID, id);
        }

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
            }

            ValidatePlayerID(clientID, id);
        }

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

        private static void ValidatePlayerID(int senderID, int packetID)
        {
            if (senderID != packetID)
            {
                Console.WriteLine(Constants.INCONSISTENT_ID_ERROR + senderID);
            }
        }
    }
}
