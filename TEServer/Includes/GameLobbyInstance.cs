using System.Collections.Generic;

namespace TEServer.Includes
{
    public class GameLobbyInstance
    {
        private List<GameClientInstance> playersInLobby;

        public int PlayerCount { get; set; }
        public string Name     { get; set; }
        public bool IsFull     { get; set; }
        public int UID         { get; private set; }

        public GameLobbyInstance(int lobbyID, GameClientInstance lobbyLeader)
        {
            PlayerCount = 1;
            UID         = lobbyID;
            Name        = lobbyLeader.UserName + "'s lobby";
            IsFull      = false;

            playersInLobby = new List<GameClientInstance>();
            playersInLobby.Add(lobbyLeader);
        }

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

        public void RemovePlayer(GameClientInstance player)
        {
            if (playersInLobby.Contains(player))
            {
                playersInLobby.Remove(player);
                PlayerCount--;

                IsFull = false;
            }
        }

        public List<GameClientInstance> GetPlayerList()
        {
            return playersInLobby;
        }
    }
}
