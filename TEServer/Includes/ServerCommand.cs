using System;

namespace TEServer.Includes
{
    public static class ServerCommand
    {
        public static void ProcessCommand(string cmd)
        {
            switch(cmd)
            {
                case "/help":
                    Console.WriteLine(Constants.DO_HELP);
                    break;
                case "/log":
                    Console.WriteLine(Constants.DO_LOG);
                    break;
                case "/kick":
                    Console.WriteLine(Constants.DO_KICK);
                    break;
                case "/shutdown":
                case "/exit":
                    Console.WriteLine(Constants.DO_SHUTDOWN);
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine(Constants.UNKNOWN);
                    break;
            }
        }
    }
}