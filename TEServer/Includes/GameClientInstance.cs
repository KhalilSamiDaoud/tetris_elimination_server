using System;

namespace TEServer
{
    public class GameClientInstance
    {
        public TCP tcp;

        public string UserName { get; set; }
        public int Status      { get; set; }
        public int UID         { get; private set; }
        public int LobbyID     { get; set; }

        public GameClientInstance(int clientID)
        {
            UID      = clientID;
            LobbyID  = -1;
            tcp      = new TCP(UID);
            UserName = null;
            Status   = 0;
        }

        public void Disconnect()
        {
            Console.WriteLine(Constants.PLAYER_DISSCONECTED + UserName);
            tcp.Disconnect();

            if (LobbyID != -1)
            {
                PacketSend.PlayerList(LobbyID);
                PacketSend.PlayerGameOver(LobbyID, UID, true);

                GameServer.openLobbies[LobbyID].RemovePlayer(this);

                if (GameServer.openLobbies[LobbyID].PlayerCount == 0)
                {
                    PacketSend.LobbyList();
                    GameServer.openLobbies.Remove(LobbyID);
                }
            }

            PacketSend.LobbyList();
            PacketSend.PlayerCountChange(UID);

            UserName = null;
            Status   = 0;
            LobbyID  = -1;
        }
    }
}
