using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace dotnet_concurrency.lesson2
{
    class CountAsync2
    {
		private static int counter;
		private static int COUNTER_MIN_TARGET = -10;
		private static int COUNTER_MAX_TARGET = 10;
		private static Random rnd = new Random();
		private static readonly object lockObj = new object();

		public async static Task StartCount()
		{
			var updateTasks = (new List<int>() { 1, 2, 3, 4 }).Select(i =>
			{
				return UpdateCounter();
			});

			var readerTasks = (new List<int>() { 1 }).Select(i =>
			{
				return ReadCounter();
			});

			await Task.WhenAll(updateTasks.Concat(readerTasks));
			Console.WriteLine("Tasks completed");
		}

		private async static Task UpdateCounter()
		{
			while (true)
			{
				if ((counter > COUNTER_MIN_TARGET) && (counter < COUNTER_MAX_TARGET))
				{
					if (rnd.Next(0, 2) == 0)
					{
						lock (lockObj)
						{
							counter++;
							Console.WriteLine("PLUS!! > count [" + counter + "] threadId [" + Thread.CurrentThread.ManagedThreadId + "]");
						}
					}
					else
					{
						lock (lockObj)
						{
							counter--;
							Console.WriteLine("MINUS! > count [" + counter + "] threadId [" + Thread.CurrentThread.ManagedThreadId + "]");
						}
					}
				}
				else break;
				await Task.Delay(rnd.Next(50, 500));
			}
		}

		private async static Task ReadCounter()
		{
			while (true)
			{
				if ((counter > COUNTER_MIN_TARGET) && (counter < COUNTER_MAX_TARGET))
				{
					Console.WriteLine("READER > count [" + counter + "] threadId [" + Thread.CurrentThread.ManagedThreadId + "]");
				}
				else break;
				await Task.Delay(1500);
			}
		}

		public static async Task Main(string[] args)
		{
			await StartCount();

			Console.WriteLine("count [" + counter + "]");
		}
	}
}