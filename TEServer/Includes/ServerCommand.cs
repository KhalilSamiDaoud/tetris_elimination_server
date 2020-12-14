using System;

namespace TEServer.Includes
{
    /// <summary>The ServerCommand static class receives commands as a string, and processes those commands.</summary>
    public static class ServerCommand
    {

        /// <summary>The GameLobbyInstance class stores lobby information for a single lobby instance. It stores lobby PlayerCount,
        /// Name, IsFull, and the lobby UID</summary>
        public static void ProcessCommand(string cmd)
        {

            string[] tokens = cmd.Split(" ");

            switch(tokens[0])
            {
                case "/help":
                    Console.WriteLine(Constants.DO_HELP);
                    break;
                case "/kick":
                    Console.WriteLine(Constants.DO_KICK);
                    Kick(tokens[1]);
                    break;
                case "/exit":
                    Console.WriteLine(Constants.DO_SHUTDOWN);
                    Environment.Exit(0);
                    break;
                case "/disconnect":
                    Console.WriteLine(Constants.DO_DISCONNECT);
                    PacketSend.ServerDisconnect();
                    break;
                default:
                    Console.WriteLine(Constants.UNKNOWN);
                    break;
            }
        }

        /// <summary>Kicks the specified player. If multiple players with the same name are connected, kick only the first found one.</summary>
        /// <param name="player">The player.</param>
        private static void Kick(string player)
        {
            foreach(GameClientInstance x in GameServer.connectedClients.Values)
            {
                if (x.UserName == player)
                {
                    PacketSend.ServerKick(x.UID);
                    return;
                }
            }

            Console.WriteLine(Constants.USER_NOT_FOUND_ERROR + player);
        }
    }
}