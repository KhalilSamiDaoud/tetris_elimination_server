namespace TEServer
{
    public static class Constants
    {
        public const int TPS = 20;

        public const int MST = 1000 / TPS;

        public static string TITLE                  = "Tetris Elemination Server V1.1";

        public static string PROMPT_SERVER_PLAYERS  = "Enter the max players [2-5] that can join: ";

        public static string PROMPT_SERVER_PORT     = "Enter the port to listen on: ";

        public static string BAD_INPUT_ERROR        = "Invalid input, check input and try again.";

        public static string INITIALIZE             = "Initializing. . .";

        public static string START_ATTEMPT          = "Server is starting. . . ";

        public static string CONNECTION_ATTEMPT     = "Client Attempting connect: ";

        public static string START_SUCCESS          = "Server has successfully started on port ";

        public static string RECEIVE_CALLBACK_ERROR = "Error receiving data: ";

        public static string SERVER_FULL_ERROR      = "Connection Refused, Server is full";

        public static string USER_CONNECTED         = "User succesfully connected, ID: ";

        public static string TCP_SEND_ERROR         = "Packet failed to send! ID: ";

        public static string HANDSHAKE_COMPLETE     = "Player handshake complete, joining lobby as: ";

        public static string INCONSISTENT_ID_ERROR  = "**Warning** ID check failed between received and sender ID for user: ";

        public static string MAIN_THREAD_STARTED    = "Main thread is starting [Lobby Controller]";

        public static string PLAYER_DISSCONECTED    = "Player disconnected: ";

        public static string READY                  = " READY!";

        public static string NOT_READY              = " NOT READY!";
    }
}
