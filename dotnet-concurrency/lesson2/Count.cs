using System;
using System.Threading;

namespace dotnet_concurrency.lesson2
{
    class Count
    {
		private static int counter;
		private static int COUNTER_MIN_TARGET = -5;
		private static int COUNTER_MAX_TARGET = 5;
		private static Random rnd = new Random();		
		private static readonly object lockObj = new object();

		public static void StartCount()
		{
			Thread threadA = new Thread(ThreadDoWork);
			Thread threadB = new Thread(ThreadDoWork);
			Thread threadC = new Thread(ThreadDoWork);
			Thread threadD = new Thread(ThreadDoWork);
			threadA.Start();
			threadB.Start();
			threadC.Start();
			threadD.Start();
			threadA.Join();
			threadB.Join();
			threadC.Join();
			threadD.Join();
		}

		private static void ThreadDoWork()
		{
				while (true)
				{
					lock (lockObj)
					{
					if ((counter > COUNTER_MIN_TARGET) && (counter < COUNTER_MAX_TARGET)) {
						if (rnd.Next(0, 2) == 0)
						{
							counter++;
							Console.WriteLine("PLUS!! > count [" + counter + "] threadId [" + Thread.CurrentThread.ManagedThreadId + "]");
						}
						else
						{
							counter--;
							Console.WriteLine("MINUS! > count [" + counter + "] threadId [" + Thread.CurrentThread.ManagedThreadId + "]");
						}
						Thread.Sleep(rnd.Next(50, 500));
					}
					else break;
					}
				}
		}

		static void Main(string[] args)
		{
			StartCount();
			
			Console.WriteLine("count [" + counter + "]");
		}
	}
}
