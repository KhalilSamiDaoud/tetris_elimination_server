using System.Collections.Generic;
using System;


namespace TEServer.Includes
{
    ///<summary>The GameLobbyInstance class stores lobby information for a single lobby instance. It stores lobby PlayerCount,
    ///Name, IsFull, and the lobby UID</summary>
    public class GameLobbyInstance
    {
        private List<GameClientInstance> playersInLobby;

        public int PlayerCount { get; set; }
        public string Name     { get; set; }
        public bool IsFull     { get; set; }
        public int UID         { get; private set; }

        ///<summary>GameLobbtInstance Constructor</summary>
        ///<param name="lobbyID">The lobbys unique ID</param>
        ///<param name="lobbyLeader">The client that created the lobby</param>
        public GameLobbyInstance(int lobbyID, GameClientInstance lobbyLeader)
        {
            PlayerCount = 1;
            UID         = lobbyID;
            Name        = lobbyLeader.UserName + "'s lobby";
            IsFull      = false;

            playersInLobby = new List<GameClientInstance>();
            playersInLobby.Add(lobbyLeader);
        }

        ///<summary>Add a player to the lobby, if the server is not full. If adding a player would make the server full, then set
        ///"IsFull" to true. Increment playerCount by 1.</summary>
        ///<param name="player">Client to add to the lobby</param>
        public void AddPlayer(GameClientInstance player)
        {
            if (!IsFull)
            {
                playersInLobby.Add(player);
                PlayerCount++;

                if (PlayerCount == (GameServer.MaxPlayers / 4))
                {
                    IsFull = true;
                }
            }
        }

        ///<summary>Remove a player from the lobby. If the lobby becomes empty, remove this lobby from the lobby list and dispose this.</summary>
        ///<param name="player">Client to remove from the lobby</param>
        ///<remarks> This function sends a "LobbyList" packet.</remarks>
        public void RemovePlayer(GameClientInstance player)
        {
            if (playersInLobby.Contains(player))
            {
                playersInLobby.Remove(player);
                PlayerCount--;

                IsFull = false;
            }

            if (PlayerCount == 0)
            {
                GameServer.openLobbies.Remove(UID);

                Console.WriteLine(Name + Constants.LOBBY_CLOSED);

                PacketSend.LobbyList();
            }
        }

        ///<summary>Get the clients that are in this lobby.</summary>
        public List<GameClientInstance> GetPlayerList()
        {
            return playersInLobby;
        }
    }
}
