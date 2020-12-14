using System;

namespace TEServer
{
    /// <summary>The GameClientInstance class stores client information for a sinle connected client. It stores the clients
    /// TCP instance, Status, Name, and connected LobbyID.</summary>
    public class GameClientInstance
    {
        public TCP tcp;

        public string UserName { get; set; }
        public int Status      { get; set; }
        public int UID         { get; private set; }
        public int LobbyID     { get; set; }

        /// <summary>GameClientInstance Constructor</summary>
        /// <param name="clientID">The clients unique ID</param>
        public GameClientInstance(int clientID)
        {
            UID      = clientID;
            LobbyID  = -1;
            tcp      = new TCP(UID);
            UserName = null;
            Status   = 0;
        }

        /// <summary>Disconnect the client from the server, notify other users of this event, and reset the state of this client instance. 
        /// If the client was in a lobby, check to see if that lobby is not empty. If so, delete the lobby.</summary>
        /// <param name="clientID">The clients unique ID</param>
        /// <remarks> This function sends a "PlayerList" packet.</remarks>
        /// <remarks> This function sends a "PlayerGameOver" packet.</remarks>
        /// <remarks> This function sends a "LobbyList" packet.</remarks>
        /// <remarks> This function sends a "PlayerCountChange" packet.</remarks>
        public void Disconnect()
        {
            Console.WriteLine(Constants.PLAYER_DISSCONECTED + UserName);
            tcp.Disconnect();

            if (LobbyID != -1)
            {
                PacketSend.PlayerList(LobbyID);
                PacketSend.PlayerGameOver(LobbyID, UID, true);

                GameServer.openLobbies[LobbyID].RemovePlayer(this);
            }

            PacketSend.LobbyList();
            PacketSend.PlayerCountChange(UID);

            UserName = null;
            Status   = 0;
            LobbyID  = -1;
        }
    }
}
