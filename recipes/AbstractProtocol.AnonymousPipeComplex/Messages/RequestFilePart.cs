// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using Hydrogen;
using System;

namespace AbstractProtocol.AnonymousPipeComplex;

[Serializable]
public class RequestFilePart {
	public string Filename { get; set; }

	public long Offset { get; set; }

	public int Length { get; set; }

	internal static RequestFilePart GenRandom() => new() {
		Filename = $"SomeFile-{Guid.NewGuid().ToStrictAlphaString()}.dat",
		Offset = Tools.Maths.RNG.Next(0, 65536),
		Length = Tools.Maths.RNG.Next(0, 65536)
	};
}
