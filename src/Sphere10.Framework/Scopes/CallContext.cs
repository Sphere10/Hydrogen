﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Sphere10.Framework {
	/// <summary>
	/// Provides a way to set contextual data that flows with the call and 
	/// async context of a test or invocation.
	/// </summary>
	public static class CallContext {
		private static readonly SynchronizedDictionary<string, AsyncLocal<object>> State = new();

		/// <summary>
		/// Stores a given object and associates it with the specified name.
		/// </summary>
		/// <param name="name">The name with which to associate the new item in the call context.</param>
		/// <param name="data">The object to store in the call context.</param>
		public static void LogicalSetData(string name, object data) {
			using (State.EnterWriteScope()) {
				if (!State.TryGetValue(name, out var asyncLocal)) {
					asyncLocal = new AsyncLocal<object>();
					State.Add(name, asyncLocal);
				}
				asyncLocal.Value = data;
			}
		}

		/// <summary>
		/// Retrieves an object with the specified name from the <see cref="CallContext"/>.
		/// </summary>
		/// <param name="name">The name of the item in the call context.</param>
		/// <returns>The object in the call context associated with the specified name, or <see langword="null"/> if not found.</returns>
		public static object LogicalGetData(string name) =>
			State.TryGetValue(name, out var data) ? data.Value : null;


		public static IEnumerable<object> LogicalSearchData(string prefix) {
			using (State.EnterReadScope()) {
				foreach(var (key, _) in State) {
					if (key.StartsWith(prefix))
						yield return key;
				}
			}
		}
	}
}
