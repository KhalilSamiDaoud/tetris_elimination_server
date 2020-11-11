using System;

namespace TEServer
{
    public class GameClientInstance
    {
        public int uid;
        public TCP tcp;

        public string UserName { get; set; }
        public bool IsReady    { get; set; }

        public GameClientInstance(int clientID)
        {
            uid      = clientID;
            tcp      = new TCP(uid);
            UserName = "null";
            IsReady  = false;
        }

        public void Disconnect()
        {
            Console.WriteLine(Constants.PLAYER_DISSCONECTED + UserName);
            tcp.Disconnect();

            PacketSend.PlayerCountChange(uid);
            PacketSend.PlayerListToAll();
        }
    }
}
