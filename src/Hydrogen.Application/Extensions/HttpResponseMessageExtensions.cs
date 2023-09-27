// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.IO;
using System.Net.Http;
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
