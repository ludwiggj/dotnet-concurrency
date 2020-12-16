using System;
using System.Threading;

namespace dotnet_concurrency.lesson2
{
    class NonAtomicCompoundOp
    {
		private static int sharedState = 0;
		private const int INCREMENT_PER_THREAD = 100000;

		static void Main(string[] args)
		{
			Thread writerThread1 = new Thread(WriterThreadEntry);
			Thread writerThread2 = new Thread(WriterThreadEntry);
			Thread writerThread3 = new Thread(WriterThreadEntry);
			Thread writerThread4 = new Thread(WriterThreadEntry);

			writerThread1.Start();
			writerThread2.Start();
			writerThread3.Start();
			writerThread4.Start();

			writerThread1.Join();
			writerThread2.Join();
			writerThread3.Join();
			writerThread4.Join();

			Console.WriteLine("NonAtomicCompoundOp Done. Value: " + sharedState + " (should be 400000)");
			Console.WriteLine("Press a key");
			Console.ReadKey();
		}

		private static void WriterThreadEntry()
		{
			for (int i = 0; i < INCREMENT_PER_THREAD; ++i)
			{
				sharedState++;
			}
		}
	}
}
