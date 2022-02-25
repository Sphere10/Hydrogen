using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sphere10.Framework;

namespace Tools {
	public static class Debugger {
		
		public static bool BreakConditionA { get; set; } = false;
		public static bool BreakConditionB { get; set; } = false;
		public static bool BreakConditionC { get; set; } = false;
		public static bool BreakConditionD { get; set; } = false;
		public static bool BreakConditionE { get; set; } = false;

		public static int CounterA { get; set; } = 0;
		public static int CounterB { get; set; } = 0;
		public static int CounterC { get; set; } = 0;
		public static int CounterD { get; set; } = 0;
		public static int CounterE { get; set; } = 0;

		public static List<string> Messages { get; } = new(4096);

		public static HashSet<object> ProducersWaitingSemaphore { get; } = new();

		public static SynchronizedSet<object> ProducersInsideSemaphore { get; } = new();
		public static SynchronizedSet<object> ProducersWaitingLock { get; } = new();
		public static SynchronizedSet<object> ProducersInsideLock { get; } = new();
		public static SynchronizedSet<object> ConsumersWaitingSemaphore { get; } = new();
		public static SynchronizedSet<object> ConsumersInsideSemaphore { get; } = new();
		public static SynchronizedSet<object> ConsumersWaitingLock { get; } = new();
		public static SynchronizedSet<object> ConsumersInsideLock { get; } = new();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void AddMessage(string message) => Messages.Add($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] {message}");

	}
}
