using System;
using System.Collections.Generic;
using System.Threading;

namespace dotnet_concurrency.lesson2
{
    class WordCounter_3_Chunked
    {
		private static List<string> wordList;
		private static List<string> curseWords;
		private static Dictionary<string, int> wordCountDict;
		private static readonly object wordCountCalculatorSyncObj = new object();

		private static int NUM_SECTIONS_PER_THREAD = 4;

		private struct ThreadWorkBlock
		{
			public readonly int StartingIndex;
			public readonly int Count;

			public ThreadWorkBlock(int startingIndex, int count)
			{
				StartingIndex = startingIndex;
				Count = count;
			}
		}

		public static void CalculateWordCounts()
		{
			wordList = new List<string> { "the", "lazy", "fox", "jumped", "over", "THE", "brown", "dog", "and", "kissed", "the", "dog's", "cock", "bum", "cock-bum" };
			curseWords = new List<string> { "BUM", "COCK" };
			wordCountDict = new Dictionary<string, int>();

			Thread threadA = new Thread(ThreadDoWork);
			Thread threadB = new Thread(ThreadDoWork);
			Thread threadC = new Thread(ThreadDoWork);
			Thread threadD = new Thread(ThreadDoWork);
			int numWords = wordList.Count;
			int quarterOfWords = numWords / 4;
			threadA.Start(new ThreadWorkBlock(quarterOfWords * 0, quarterOfWords));
			threadB.Start(new ThreadWorkBlock(quarterOfWords * 1, quarterOfWords));
			threadC.Start(new ThreadWorkBlock(quarterOfWords * 2, quarterOfWords));
			threadD.Start(new ThreadWorkBlock(quarterOfWords * 3, numWords - quarterOfWords * 3));
			threadA.Join();
			threadB.Join();
			threadC.Join();
			threadD.Join();
		}

		private static void ThreadDoWork(object threadStartParameter)
		{
			ThreadWorkBlock localWorkBlock = (ThreadWorkBlock)threadStartParameter;

			Dictionary<string, int> localWordCountDict = new Dictionary<string, int>();
			for (int i = localWorkBlock.StartingIndex; i < localWorkBlock.StartingIndex + localWorkBlock.Count; ++i)
			{
				string thisWord = wordList[i].ToUpper().Replace("-", String.Empty).Replace("'", String.Empty).Trim();
				bool firstLocalOccurrenceOfWord = !localWordCountDict.ContainsKey(thisWord);
				if (firstLocalOccurrenceOfWord) localWordCountDict.Add(thisWord, 1);
				else localWordCountDict[thisWord] = localWordCountDict[thisWord] + 1;
			}
			lock (wordCountCalculatorSyncObj)
			{
				foreach (var kvp in localWordCountDict)
				{
					bool firstGlobalOccurrenceOfWord = !wordCountDict.ContainsKey(kvp.Key);
					if (firstGlobalOccurrenceOfWord) wordCountDict.Add(kvp.Key, kvp.Value);
					else wordCountDict[kvp.Key] += kvp.Value;
				}
			}
		}

		private static void ThreadDoWorkSectioned(object threadStartParameter)
		{
			ThreadWorkBlock localWorkBlock = (ThreadWorkBlock)threadStartParameter;

			Dictionary<string, int> localWordCountDict = new Dictionary<string, int>();
			int countPerSection = localWorkBlock.Count / NUM_SECTIONS_PER_THREAD;
			for (int section = 0; section < NUM_SECTIONS_PER_THREAD; ++section)
			{
				int startingIndex = localWorkBlock.StartingIndex + countPerSection * section;
				int count = section < NUM_SECTIONS_PER_THREAD - 1 ? countPerSection : localWorkBlock.Count - countPerSection * (NUM_SECTIONS_PER_THREAD - 1);
				localWordCountDict.Clear();

				for (int i = startingIndex; i < startingIndex + count; ++i)
				{
					string thisWord = wordList[i].ToUpper().Replace("-", String.Empty).Replace("'", String.Empty).Trim();
					bool firstLocalOccurrenceOfWord = !localWordCountDict.ContainsKey(thisWord);
					if (firstLocalOccurrenceOfWord) localWordCountDict.Add(thisWord, 1);
					else localWordCountDict[thisWord] = localWordCountDict[thisWord] + 1;
				}
				lock (wordCountCalculatorSyncObj)
				{
					foreach (var kvp in localWordCountDict)
					{
						bool firstGlobalOccurrenceOfWord = !wordCountDict.ContainsKey(kvp.Key);
						if (firstGlobalOccurrenceOfWord) wordCountDict.Add(kvp.Key, kvp.Value);
						else wordCountDict[kvp.Key] += kvp.Value;
					}
				}
			}
		}

		static void Main(string[] args)
		{
			CalculateWordCounts();

			foreach (KeyValuePair<string, int> kv in wordCountDict)
				Console.WriteLine("Key [" + kv.Key.ToString() + "] Value [" + kv.Value.ToString() + "]");
		}
	}
}
