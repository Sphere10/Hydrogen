using System;
using System.Text;
using Hydrogen.Application;
using Hydrogen.Web.AspNetCore;
using Microsoft.AspNetCore.Http;

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

