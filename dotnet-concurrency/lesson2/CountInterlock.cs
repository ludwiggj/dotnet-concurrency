using System;
using System.Threading;

namespace dotnet_concurrency.lesson2
{
    class CountInterlock
    {
		private static int counter;
		private static int COUNTER_TARGET = 50;
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
			int prevValue = counter;
			while (prevValue < COUNTER_TARGET)
			{
				Interlocked.CompareExchange(ref counter, prevValue + 1, prevValue);
				prevValue = counter;

				Console.WriteLine("count [" + prevValue + "] threadId [" + Thread.CurrentThread.ManagedThreadId + "]");
				//Thread.Sleep(rnd.Next(50, 500));
			}
		}

		static void Main(string[] args)
		{
			StartCount();
			
			Console.WriteLine("count [" + counter + "]");
		}
	}
}
