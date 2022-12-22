using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Hydrogen.Data.NHibernate;

public class JsonWorker {
	public static readonly JsonSerializerSettings Settings;

	static JsonWorker() {
		Settings = new JsonSerializerSettings {
			ContractResolver = new CamelCasePropertyNamesContractResolver(),
			Converters = new List<JsonConverter> { new StringEnumConverter() },
			ObjectCreationHandling = ObjectCreationHandling.Auto,
			ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
			TypeNameHandling = TypeNameHandling.Auto
		};
	}

	public static JsonSerializerSettings Configure(params Action<JsonSerializerSettings>[] actions) {
		actions.ToList().ForEach(x => x(Settings));
		return Settings;
	}

	public static string Serialize(object obj)
		=> Tools.Json.WriteToString(obj);

	public static T Deserialize<T>(string json)
		=> Tools.Json.ReadFromString<T>(json);

	public static void PopulateObject<T>(string json, T obj) {
		JsonConvert.PopulateObject(json, obj, Settings);
	}
}
