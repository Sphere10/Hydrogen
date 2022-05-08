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

namespace Hydrogen {
	/// <example>
	/// TypeSwitch.Do(
	///		sender,
	///		TypeSwitch.Case<Button>(() => textBox1.Text = "Hit a Button"),
	///		TypeSwitch.Case<CheckBox>(x => textBox1.Text = "Checkbox is " + x.Checked),
	///		TypeSwitch.Default(() => textBox1.Text = "Not sure what is hovered over")
	/// );
	/// </example>
	public static class TypeSwitch {
		public class CaseInfo {
			public bool IsDefault { get; set; }
			public Type Target { get; set; }
			public Action<object> Action { get; set; }
		}

		public static void For(object source, params CaseInfo[] cases) {
			var found = false;
			var type = source.GetType();
			foreach (var entry in cases) {
				if (entry.IsDefault || entry.Target.IsAssignableFrom(type)) {
					entry.Action(source);
					found = true;
					break;
				}
			}
			if (!found)
				throw new InvalidOperationException($"Unrecognized type '{type}'");
		}

		public static void ForType(Type type, params CaseInfo[] cases) {
			var found = false;
			foreach (var entry in cases) {
				if (entry.IsDefault || entry.Target.IsAssignableFrom(type)) {
					entry.Action(type);
					found = true;
					break;
				}
			}
			if (!found)
				throw new InvalidOperationException($"Unrecognized type '{type}'");
		}

		public static CaseInfo Case<T>(Action action) {
			return new CaseInfo() {
				Action = x => action(),
				Target = typeof(T)
			};
		}

		public static CaseInfo Case<T>(Action<T> action) {
			return new CaseInfo() {
				Action = (x) => action((T)x),
				Target = typeof(T)
			};
		}

		public static CaseInfo Default(Action action) {
			return new CaseInfo() {
				Action = x => action(),
				IsDefault = true
			};
		}
	}
}
