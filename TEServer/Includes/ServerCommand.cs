using System;

namespace TEServer.Includes
{
    public static class ServerCommand
    {
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