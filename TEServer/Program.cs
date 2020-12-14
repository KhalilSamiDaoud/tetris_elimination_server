using System.Net.NetworkInformation;
using TEServer.Includes;
using System.Threading;
using System;

namespace TEServer
{
    /// <summary> The Program class is the starting point of TEServer and contains the Main function. In here, the command thread
    /// and the lobby controller thread are created. The server player count and the port are specified here.</summary>
    class Program
    {
        private static bool isRunning;
        private static int maxPlayers;
        private static int port;

        private static Thread mainThread;
        private static Thread commandThread;

        /// <summary>On exit, disconnect all players and then join all threads. There is a 3 second delay on exit.</summary>
        /// <param name="sender">The Event Sender, Process</param>
        /// <param name="e">The event argument, OnProcessExit</param>
        private static void OnProcessExit(object sender, EventArgs e)
        {

           isRunning = false;

            if (GameServer.tcpListener != null)
            {
                Console.WriteLine(Constants.SHUTDOWN);

                PacketSend.ServerDisconnect();

                commandThread.Join();
                mainThread.Join();
            }
        }

        /// <summary>Before starting threads, attempt to get server port and player count.</summary>
        private static void StartupPrompt()
        {
            bool validPlayers = false;
            bool validPort    = false;
            bool success      = false;
            int input;

            while(!validPlayers)
            {
                Console.WriteLine(Constants.PROMPT_SERVER_PLAYERS);
                success = Int32.TryParse(Console.ReadLine(), out input);

                if(input >= 2 && input <= 5 && success)
                {
                    validPlayers = true;
                    maxPlayers   = input * 4;
                }
                else
                {
                    Console.WriteLine(Constants.BAD_INPUT_ERROR);
                }
            }

            success = false;

            while(!validPort)
            {
                Console.WriteLine(Constants.PROMPT_SERVER_PORT);
                if (Int32.TryParse(Console.ReadLine(), out input) && CheckPort(input))
                {
                    success = true;
                }

                if(success)
                {
                    validPort = true;
                    port      = input;
                }
                else
                {
                    Console.WriteLine(Constants.BAD_INPUT_ERROR);
                }
            }
        }

        /// <summary>Check port provided by user, if its open return true. Otherwise return false.</summary>
        /// <param name="port">The port number provided by the user.</param>
        /// <returns> True if port is valid, False otherwise.</returns>
        private static bool CheckPort(int port)
        {
            IPGlobalProperties ports              = IPGlobalProperties.GetIPGlobalProperties();
            TcpConnectionInformation[] portsArray = ports.GetActiveTcpConnections();

            foreach (TcpConnectionInformation tcpi in portsArray)
            {
                if (tcpi.LocalEndPoint.Port == port || port <= 0 || port > 65535)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>The MainThread loop, which puts the thread to sleep until the time interval has elapsed. This thread
        /// controls all lobby and network related events.</summary>
        private static void MainThread()
        {
            Console.WriteLine(Constants.MAIN_THREAD_STARTED);
            Console.WriteLine("TPS: " + Constants.TPS + " | M/s Per Tick: " + Constants.MST);

            DateTime nextLoop = DateTime.Now;

            while (isRunning)
            {
                while (nextLoop < DateTime.Now)
                {
                    Update();

                    nextLoop = nextLoop.AddMilliseconds(Constants.MST);

                    if (nextLoop > DateTime.Now)
                    {
                        Thread.Sleep(nextLoop - DateTime.Now);
                    }
                }
            }
        }

        /// <summary>The CommandThread loop, which puts the thread to sleep until the time interval has elapsed. This thread
        /// controls all user input events and processes them.</summary>
        private static void CommandThread()
        {
            Console.WriteLine(Constants.COMMAND_THREAD_STARTED);

            DateTime nextLoop = DateTime.Now;

            while (isRunning)
            {
                while (nextLoop < DateTime.Now)
                {

                    string cmd = Console.ReadLine();
                    ServerCommand.ProcessCommand(cmd);

                    Update();

                    nextLoop = nextLoop.AddMilliseconds(Constants.MST);

                    if (nextLoop > DateTime.Now)
                    {
                        Thread.Sleep(nextLoop - DateTime.Now);
                    }
                }
            }
        }

        /// <summary>Call UpdateMain() in the ThreadManager.</summary>
        private static void Update()
        {
            ThreadManager.UpdateMain();
        }

        /// <summary>The starting point of TEServer. This function is responsible for outputing initial prompts and initializeing the server, 
        /// and for starting mainThread and the commandThread threads. It also sets the windows title.</summary>
        static void Main(string[] args)
        {
            isRunning     = true;
            Console.Title = Constants.TITLE;

            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);

            StartupPrompt();
            Console.Clear();

            Console.WriteLine(Constants.INITIALIZE);
            GameServer.InitializeServer(maxPlayers, port);

            mainThread    = new Thread(new ThreadStart(MainThread));
            mainThread.Start();

            commandThread = new Thread(new ThreadStart(CommandThread));
            commandThread.Start();
        }
    }
}
