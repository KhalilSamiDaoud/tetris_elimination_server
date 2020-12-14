using System.Collections.Generic;
using System;

namespace TEServer
{
    /// <summary>The Thread Manager makes sure certain actions are executed on the main thread as to avoid threads stepping over one another</summary>
    /// <remarks>Class Provided by Tom Weiland for educational purposes</remarks>
    public class ThreadManager
    {
        private static readonly List<Action> executeOnMainThread = new List<Action>();
        private static readonly List<Action> executeCopiedOnMainThread = new List<Action>();
        private static bool actionToExecuteOnMainThread = false;

        /// <summary>Executes specified action the on main thread.</summary>
        /// <param name="action">Action to execute</param>
        public static void ExecuteOnMainThread(Action action)
        {
            if (action == null)
            {
                return;
            }

            lock (executeOnMainThread)
            {
                executeOnMainThread.Add(action);
                actionToExecuteOnMainThread = true;
            }
        }

        /// <summary>Updates the mainThread, makes sure that no other threads step on this section.</summary>
        public static void UpdateMain()
        {
            if (actionToExecuteOnMainThread)
            {
                executeCopiedOnMainThread.Clear();
                lock (executeOnMainThread)
                {
                    executeCopiedOnMainThread.AddRange(executeOnMainThread);
                    executeOnMainThread.Clear();
                    actionToExecuteOnMainThread = false;
                }

                for (int i = 0; i < executeCopiedOnMainThread.Count; i++)
                {
                    executeCopiedOnMainThread[i]();
                }
            }
        }
    }
}

