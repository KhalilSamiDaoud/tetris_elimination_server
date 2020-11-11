namespace TEServer
{
    public class PacketSend
    {
        public static void WelcomeVerification(int clientID, string msg)
        {
            using (Packet packet = new Packet((int)ServerPackets.welcome))
            {
                packet.Write(msg);
                packet.Write(clientID);

                SendTCPData(clientID, packet);
            }
        }

        public static void PlayerListToAll()
        {
            for (int i = 1; i <= GameServer.connectedClients.Count; i++)
            {
                if (GameServer.connectedClients[i].tcp.socket != null)
                {
                    using (Packet packet = new Packet((int)ServerPackets.playerListToAll))
                    {
                        packet.Write(GameServer.connectedClients[i].IsReady);
                        packet.Write(GameServer.connectedClients[i].UserName);
                        packet.Write(GameServer.connectedClients[i].uid);

                        SendTCPDataToAll(packet);
                    }
                }
            }
        }

        public static void PlayerListToOne(int clientID)
        {
            for (int i = 1; i <= GameServer.connectedClients.Count; i++)
            {
                if (GameServer.connectedClients[i].tcp.socket != null)
                {
                    using (Packet packet = new Packet((int)ServerPackets.playerListToOne))
                    {
                        packet.Write(GameServer.connectedClients[i].IsReady);
                        packet.Write(GameServer.connectedClients[i].UserName);
                        packet.Write(GameServer.connectedClients[i].uid);

                        SendTCPData(clientID, packet);
                    }
                }
            }
        }

        public static void PlayerReadyChange(int clientID, bool msg)
        {
            using (Packet packet = new Packet((int)ServerPackets.playerReadyChange))
            {
                packet.Write(msg);
                packet.Write(clientID);
                SendTCPDataToAllEx(clientID, packet);
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
    }
}
