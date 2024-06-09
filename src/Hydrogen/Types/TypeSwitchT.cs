// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

/// <example>
/// var value = TypeSwitchExpression<string>.For(
///		sender,
///		TypeSwitch.Case<Button>(() => "Hit a Button"),
///		TypeSwitch.Case<CheckBox>(x => "Checkbox is " + x.Checked),
///		TypeSwitch.Default(() => "Not sure what is hovered over")
/// );
/// </example>
public static class TypeSwitch<T> {
	public class CaseInfo {
		public bool IsDefault { get; set; }
		public Type Target { get; set; }
		public Func<object, T> Func { get; set; }
	}


	public static T For(object source, params CaseInfo[] cases) {
		var type = source.GetType();
		foreach (var entry in cases) {
			if (entry.IsDefault || entry.Target.IsAssignableFrom(type)) {
				return entry.Func(source);
			}
		}
		throw new InvalidOperationException($"Unrecognized type '{type}'");
	}

	public static T ForType(Type type, params CaseInfo[] cases) {
		foreach (var entry in cases) {
			if (entry.IsDefault || entry.Target.IsAssignableFrom(type)) {
				return entry.Func(type);
			}
		}
		throw new InvalidOperationException($"Unrecognized type '{type}'");
	}

	//public static CaseInfo Case(Func<T> func) {
	//	return new CaseInfo() {
	//		Func = x => func(),
	//		Target = typeof(T)
	//	};
	//}

	public static CaseInfo Case<TConcrete>(Func<TConcrete, T> func) {
		return new CaseInfo() {
			Func = (x) => func((TConcrete)x),
			Target = typeof(TConcrete)
		};
	}

	public static CaseInfo Default(Func<T> func) {
		return new CaseInfo() {
			Func = _ => func(),
			IsDefault = true
		};
	}
}
