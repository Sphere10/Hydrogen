// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Text;

namespace Microsoft.AspNetCore.Http;

public static class ISessionExtensions {
	public static void SetJsonObject<T>(this ISession session, string key, T item) {
		var bytes = Encoding.UTF8.GetBytes(Tools.Json.WriteToString(item));
		session.Set(key, bytes);
	}

	public static bool TryGetJsonObject<T>(this ISession session, string key, out T item) {
		item = default;
		if (!session.TryGetValue(key, out var bytes))
			return false;
		item = Tools.Json.ReadFromString<T>(Encoding.UTF8.GetString(bytes));
		return true;
	}

	public static T GetJsonObject<T>(this ISession session, string key) {
		if (!session.TryGetJsonObject<T>(key, out var obj))
			throw new InvalidOperationException($"No object found with key '{key}'");
		return obj;
	}

	public static T GetOrCreateJsonObject<T>(this ISession session, string key) where T : new()
		=> session.GetOrCreateJsonObject<T>(key, () => new());

	public static T GetOrCreateJsonObject<T>(this ISession session, string key, Func<T> factory) {
		if (!session.TryGetJsonObject<T>(key, out var obj))
			session.SetJsonObject(key, factory());
		return session.GetJsonObject<T>(key);
	}

	public static T GetJsonObjectOrDefault<T>(this ISession session, string key, T @default = default) {
		if (!session.TryGetJsonObject<T>(key, out var obj))
			return @default;
		return obj;
	}


}
