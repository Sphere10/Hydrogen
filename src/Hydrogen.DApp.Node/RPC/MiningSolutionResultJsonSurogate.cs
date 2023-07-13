// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Hydrogen.DApp.Core.Mining;

namespace Hydrogen.DApp.Node.RPC;

[Serializable]
public class MiningSolutionResultJsonSurogate {
	[JsonConverter(typeof(StringEnumConverter))]
	public MiningSolutionResult SolutionResult { get; set; }

	public uint TimeStamp { get; set; }
	public uint BlockNumber { get; set; }
}
