using System;

namespace dotnet_concurrency.skeet
{

    using System.Threading;

    public class TestWithProperty
    {
        static bool stop;
        static readonly object stopLock = new object();
        static bool Stop
        {
            get
            {
                lock (stopLock)
                {
                    return stop;
                }
            }
            set
            {
                lock (stopLock)
                {
                    stop = value;
                }
            }
        }

        static void Main()
        {
            ThreadStart job = new ThreadStart(ThreadJob);
            Thread thread = new Thread(job);
            thread.Start();

            // Let the thread start running
            Thread.Sleep(2000);

            // Now tell it to stop counting
            Stop = true;
        }

        static void ThreadJob()
        {
            int count = 0;
            while (!Stop)
            {
                Console.WriteLine("Extra thread: count {0}", count);
                Thread.Sleep(100);
                count++;
            }
        }
    }
}
