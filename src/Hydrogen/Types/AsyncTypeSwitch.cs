//-----------------------------------------------------------------------
// <copyright file="TypeSwitch.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (http://www.sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// <author>Herman Schoenfeld</author>
// <date>2018</date>
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Threading.Tasks;

namespace Hydrogen {
	
	public static class AsyncTypeSwitch {
		public class CaseInfo {
			public bool IsDefault { get; set; }
			public Type Target { get; set; }
			public Func<object, Task> Action { get; set; }
		}

		public static async Task For(object source, params CaseInfo[] cases) {
			var found = false;
			var type = source.GetType();
			foreach (var entry in cases) {
				if (entry.IsDefault || entry.Target.IsAssignableFrom(type)) {
					await entry.Action(source);
					found = true;
					break;
				}
			}
			if (!found)
				throw new InvalidOperationException($"Unrecognized type '{type}'");
		}

		public static async Task ForType(Type type, params CaseInfo[] cases) {
			var found = false;
			foreach (var entry in cases) {
				if (entry.IsDefault || entry.Target.IsAssignableFrom(type)) {
					await entry.Action(type);
					found = true;
					break;
				}
			}
			if (!found)
				throw new InvalidOperationException($"Unrecognized type '{type}'");
		}

		public static CaseInfo Case<T>(Func<Task> action) {
			return new CaseInfo() {
				Action = x => action(),
				Target = typeof(T)
			};
		}

		public static CaseInfo Case<T>(Func<T, Task> action) {
			return new CaseInfo() {
				Action = (x) => action((T)x),
				Target = typeof(T)
			};
		}

		public static CaseInfo Default(Func<Task> action) {
			return new CaseInfo() {
				Action = x => action(),
				IsDefault = true
			};
		}
	}
}
