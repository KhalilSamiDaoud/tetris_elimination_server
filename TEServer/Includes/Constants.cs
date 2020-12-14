namespace TEServer
{
    /// <summary>The Constants class is a static class that defines constants used throughout the program. These are mostly prompts.</summary>
    public static class Constants
    {

        //SERVER TPS
        public const int TPS = 30;

        public const int MST = 1000 / TPS;

        //SERVER GENERAL PROMPTS 
        public static string TITLE                  = "Tetris Elemination Server V3.0a";

        public static string PROMPT_SERVER_PLAYERS  = "Enter the max players per lobby [2-5]: ";

        public static string PROMPT_SERVER_PORT     = "Enter the port to listen on: ";

        public static string BAD_INPUT_ERROR        = "Invalid input, check input and try again.";

        public static string INITIALIZE             = "Initializing. . .";

        public static string START_ATTEMPT          = "Server is starting. . . ";

        public static string CONNECTION_ATTEMPT     = "Client Attempting connect: ";

        public static string START_SUCCESS          = "Server has successfully started on ";

        public static string RECEIVE_CALLBACK_ERROR = "Error receiving data: ";

        public static string SERVER_FULL_ERROR      = "Connection Refused, Server is full";

        public static string USER_CONNECTED         = "User succesfully connected, ID: ";

        public static string TCP_SEND_ERROR         = "Packet failed to send! ID: ";

        public static string HANDSHAKE_COMPLETE     = "Player handshake complete, joining server as: ";

        public static string INCONSISTENT_ID_ERROR  = "**Warning** ID check failed between received and sender ID for user: ";

        public static string USER_NOT_FOUND_ERROR   = "User not found with Name: ";

        public static string MAIN_THREAD_STARTED    = "Main thread is starting [Lobby Controller]";

        public static string COMMAND_THREAD_STARTED = "Command thread is starting [Command Handler]";

        public static string PLAYER_DISSCONECTED    = "Player disconnected: ";

        public static string READY                  = " READY!";

        public static string NOT_READY              = " NOT READY!";

        public static string IN_GAME                = " IN GAME!";

        public static string LOBBY_OPENED           = "'s Lobby has opened";

        public static string LOBBY_CLOSED           = " has closed";

        public static string GAME_STARTED           = "Game starting in 3... 2... 1... [Main Thread] - ";

        public static string SHUTDOWN               = "INITIALIZING SAFE SHUTDOWN... \nSENDING DISCONNECT REQUEST...";

        //SERVER COMMAND PROMPTS
        public static string DO_HELP        = "\n ================================================ \n Supported commands: \n /help" +
                                              "\n /exit \n /kick \n /disconnect \n ================================================";

        public static string DO_SHUTDOWN    = "\n**shutdown initiated.";

        public static string DO_KICK        = "\n**kicking player: ";

        public static string DO_DISCONNECT  = "\n**kicking player: ";

        public static string UNKNOWN        = "\n**unknown command, try /help for a list of avaliable commands.";
    }
}