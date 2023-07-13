// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Hydrogen.Web.AspNetCore;

public class FormResult {

	[JsonProperty("result")] public bool Result { get; set; }

	[JsonProperty("message")] public string Message { get; set; }

	[JsonProperty("type")]
	[JsonConverter(typeof(StringEnumConverter))]
	public FormResultType ResultType { get; set; }

	[JsonProperty("url")] public string Url { get; set; }

	[JsonProperty("content")] public string Content { get; set; }
}
