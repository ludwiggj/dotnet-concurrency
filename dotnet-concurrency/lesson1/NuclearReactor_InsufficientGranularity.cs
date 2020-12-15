using System;
using System.Threading.Tasks;

namespace dotnet_concurrency.lesson1
{
	class NuclearReactor_InsufficientGranularity
	{
		public static class ReactorEventDispatcher
		{
			private static readonly object eventDispatcherSyncObj = new object();

			private static event Action startingUp;
			private static event Action heatCritical;
			private static event Action safetyShutdown;

			public static event Action StartingUp
			{
				add
				{
					lock (eventDispatcherSyncObj)
					{
						startingUp += value;
					}
				}
				remove
				{
					lock (eventDispatcherSyncObj)
					{
						startingUp -= value;
					}
				}
			}

			public static event Action HeatCritical
			{
				add
				{
					lock (eventDispatcherSyncObj)
					{
						heatCritical += value;
					}
				}
				remove
				{
					lock (eventDispatcherSyncObj)
					{
						heatCritical -= value;
					}
				}
			}

			public static event Action SafetyShutdown
			{
				add
				{
					lock (eventDispatcherSyncObj)
					{
						safetyShutdown += value;
					}
				}
				remove
				{
					lock (eventDispatcherSyncObj)
					{
						safetyShutdown -= value;
					}
				}
			}

			internal static void TriggerStartUp()
			{
				lock (eventDispatcherSyncObj)
				{
					if (startingUp != null) startingUp();
				}
			}

			internal static void TriggerHeatCritical()
			{
				lock (eventDispatcherSyncObj)
				{
					if (heatCritical != null) heatCritical();
				}
			}

			internal static void TriggerSafetyShutdown()
			{
				lock (eventDispatcherSyncObj)
				{
					if (safetyShutdown != null) safetyShutdown();
				}
			}

			internal static void TriggerSafetyShutdownSafe()
			{
				Action safetyShutdownLocal;
				lock (eventDispatcherSyncObj)
				{
					safetyShutdownLocal = safetyShutdown;
				}
				if (safetyShutdownLocal != null) safetyShutdownLocal();
			}
		}

		static void Main(string[] args)
		{
			ReactorEventDispatcher.HeatCritical += () =>
			{
				var triggerShutdownTask = Task.Run((Action)ReactorEventDispatcher.TriggerSafetyShutdown);

				// OtherLongRunningSafetyCode();

				triggerShutdownTask.Wait();
			};
		}
	}
}