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
public class JsonResponse {
	[JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "jsonrpc")]
	public string JsonRpc { get; set; } = "2.0";

	[JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "result")]
	public object Result { get; set; }

	[JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "error")]
	public JsonRpcException Error { get; set; }

	[JsonProperty(PropertyName = "id")] public int Id { get; set; }
}
