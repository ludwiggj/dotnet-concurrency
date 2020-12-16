using System;
using System.Threading;

namespace dotnet_concurrency.lesson3
{
    class Atomicity
    {
		private const int NUM_ITERATIONS = 2000;
		private const decimal DENOMINATOR = 200;
		private const decimal MAX_POSSIBLE_VALUE = NUM_ITERATIONS / DENOMINATOR;

		private static decimal sharedState;
		public static decimal SharedState
		{
			get
			{
				return sharedState;
			}
			set
			{
				sharedState = value;
			}
		}

		static void Main(string[] args)
		{
			Thread writerThread = new Thread(WriterThreadEntry);
			Thread readerThread = new Thread(ReaderThreadEntry);

			writerThread.Start();
			readerThread.Start();

			writerThread.Join();
			readerThread.Join();

			Console.WriteLine(MAX_POSSIBLE_VALUE);
			Console.ReadKey();
		}

		private static void WriterThreadEntry()
		{
			for (int i = 0; i < NUM_ITERATIONS; ++i)
			{
				SharedState = i / DENOMINATOR;
			}
		}

		private static void ReaderThreadEntry()
		{
			for (int i = 0; i < NUM_ITERATIONS; ++i)
			{
				var sharedStateLocal = SharedState;
				if (sharedStateLocal > MAX_POSSIBLE_VALUE) Console.WriteLine("Impossible value detected: " + sharedStateLocal);
			}
		}
	}
}
