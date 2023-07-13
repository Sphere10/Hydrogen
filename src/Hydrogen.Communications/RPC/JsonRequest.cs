// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Polyminer
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using Newtonsoft.Json;

namespace Hydrogen.Communications.RPC;

[JsonObject(MemberSerialization.OptIn)]
public class JsonRequest {
	public JsonRequest() {
	}

	[JsonProperty("jsonrpc")]
	public string JsonRpc {
		get { return "2.0"; }
	}

	[JsonProperty("method")] public string Method { get; set; }

	[JsonProperty("params")] public object Params { get; set; }

	[JsonProperty("id")] public object Id { get; set; }
}
