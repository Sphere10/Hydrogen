// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using Newtonsoft.Json;
using Hydrogen.Communications;

namespace Hydrogen.DApp.Node.RPC;

[Serializable]
public class MiningSolutionJsonSurogate {
	[JsonConverter(typeof(HexadecimalValueConverterReader))]
	public uint WorkID { get; set; }

	public string MinerTag { get; set; }

	[JsonConverter(typeof(HexadecimalValueConverterReader))]
	public uint Nonce { get; set; }

	[JsonConverter(typeof(HexadecimalValueConverterReader))]
	public uint Time { get; set; }
}
