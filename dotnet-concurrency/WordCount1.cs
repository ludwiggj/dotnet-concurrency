using System;
using System.Collections.Generic;
using System.Threading;

namespace dotnet_concurrency
{
    class WordCount1
    {
		private static List<string> wordList;
		private static List<string> curseWords;
		private static int currentWordIndex;
		private static Dictionary<string, int> wordCountDict;

		public static void CalculateWordCounts()
		{
			wordList = new List<string> { "the", "lazy", "fox", "jumped", "over", "the", "brown", "dog", "and", "kissed", "the", "cock", "bum" };
			curseWords = new List<string> { "bum", "cock" };
			currentWordIndex = 0;
			wordCountDict = new Dictionary<string, int>();

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
			bool atLeastOneWordRemaining;
			int thisWordIndex = currentWordIndex;
			currentWordIndex = currentWordIndex + 1;
			if (thisWordIndex >= wordList.Count) atLeastOneWordRemaining = false;
			else atLeastOneWordRemaining = true;

			while (atLeastOneWordRemaining)
			{
				string thisWord = wordList[thisWordIndex];
				bool firstOccurrenceOfWord = !wordCountDict.ContainsKey(thisWord);

				if (curseWords.Contains(thisWord)) Console.WriteLine("Curse word detected!");
				if (firstOccurrenceOfWord) wordCountDict.Add(thisWord, 1);
				else wordCountDict[thisWord] = wordCountDict[thisWord] + 1;

				thisWordIndex = currentWordIndex;
				currentWordIndex = currentWordIndex + 1;
				if (thisWordIndex >= wordList.Count) atLeastOneWordRemaining = false;
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
