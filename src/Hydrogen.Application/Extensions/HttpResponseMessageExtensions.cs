using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Hydrogen.Application;

internal static class HttpResponseMessageExtensions {
	internal static async Task<T> ParseStreamAsync<T>(this HttpResponseMessage response, JsonSerializerSettings serializerSettings = null) {
		await using var stream = await response.Content.ReadAsStreamAsync();
		using var streamReader = new StreamReader(stream);
		using var jsonReader = new JsonTextReader(streamReader);
		var serializer = serializerSettings == null ? JsonSerializer.CreateDefault() : JsonSerializer.Create(serializerSettings);
		return serializer.Deserialize<T>(jsonReader);
	}
}