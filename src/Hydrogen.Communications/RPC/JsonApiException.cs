// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Polyminer
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using Newtonsoft.Json;

namespace Hydrogen.Communications.RPC;

[Serializable]
[JsonObject(MemberSerialization.OptIn)]
public class JsonRpcException : System.Exception {
	[JsonProperty] public int code { get; set; }

	[JsonProperty("Message")] public string text { get; set; }

	public override string ToString() {
		return $"RPC error {code}: {text}";
	}

	public JsonRpcException(int _code, string _text) : base(_text) {
		code = _code;
		text = _text;
	}
}
