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
            PacketSend.PlayerListToAll();

            ValidatePlayerID(clientID, id);
        }

        public static void ClientStatus(int clientID, Packet packet)
        {
            int id   = packet.ReadInt();
            int msg = packet.ReadInt();

            switch(msg)
            {
                case 0:
                    Console.WriteLine(GameServer.connectedClients[id].UserName + Constants.NOT_READY);
                    break;
                case 1:
                    Console.WriteLine(GameServer.connectedClients[id].UserName + Constants.READY);
                    break;
                case 2:
                    Console.WriteLine(GameServer.connectedClients[id].UserName + Constants.IN_GAME);
                    break;
            }

            GameServer.connectedClients[id].Status = msg;
            PacketSend.PlayerReadyChange(id, msg);

            if( GameServer.CountReadyPlayers() == GameServer.MaxPlayers)
            {
                PacketSend.StartGame();
                Console.WriteLine(Constants.GAME_STARTED);
            }

            ValidatePlayerID(clientID, id);
        }

        public static void ClientGrid(int clientID, Packet packet)
        {
            int id     = packet.ReadInt();
            string msg = packet.ReadString();

            PacketSend.PlayerGrids(id, msg);

            ValidatePlayerID(clientID, id);
        }

        public static void ClientScore(int clientID, Packet packet)
        {
            int id  = packet.ReadInt();
            int msg = packet.ReadInt();

            PacketSend.PlayerScore(id, msg);

            ValidatePlayerID(clientID, id);
        }

        public static void ClientGameOver(int clientID, Packet packet)
        {
            int id   = packet.ReadInt();
            bool msg = packet.ReadBool();

            PacketSend.PlayerGameOver(id, msg);

            ValidatePlayerID(clientID, id);
        }

        public static void ClientReconnect(int clientID, Packet packet)
        {
            int id = packet.ReadInt();

            PacketSend.WelcomeVerification(id);

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
