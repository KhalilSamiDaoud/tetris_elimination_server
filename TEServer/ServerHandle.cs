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

        public static void ClientReady(int clientID, Packet packet)
        {
            int id   = packet.ReadInt();
            bool msg = packet.ReadBool();

            if (msg)
            {
                Console.WriteLine(GameServer.connectedClients[id].UserName + Constants.NOT_READY);
            }
            else
            {
                Console.WriteLine(GameServer.connectedClients[id].UserName + Constants.READY);
            }

            GameServer.connectedClients[id].IsReady = msg;
            PacketSend.PlayerReadyChange(id, msg);
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
