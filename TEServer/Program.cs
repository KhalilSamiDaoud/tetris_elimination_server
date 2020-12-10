﻿using System.Net.NetworkInformation;
using TEServer.Includes;
using System.Threading;
using System;

namespace TEServer
{
    class Program
    {
        private static bool isRunning;
        private static int maxPlayers;
        private static int port;

        private static Thread mainThread;
        private static Thread commandThread;

        static void OnProcessExit(object sender, EventArgs e)
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

        public static void StartupPrompt()
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
                    success      = false;
                    validPlayers = true;
                    maxPlayers   = input * 4;
                }
                else
                {
                    Console.WriteLine(Constants.BAD_INPUT_ERROR);
                }
            }

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

        public static void Update()
        {
            ThreadManager.UpdateMain();
        }

        static void Main(string[] args)
        {
            isRunning     = true;
            Console.Title = Constants.TITLE;

            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);

            StartupPrompt();
            Console.Clear();

            Console.WriteLine(Constants.INITIALIZE);
            GameServer.InitializeServer(maxPlayers, port);

            mainThread = new Thread(new ThreadStart(MainThread));
            mainThread.Start();

            commandThread = new Thread(new ThreadStart(CommandThread));
            commandThread.Start();
        }
    }
}
