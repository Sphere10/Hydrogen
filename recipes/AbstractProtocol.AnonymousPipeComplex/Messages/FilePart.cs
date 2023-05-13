// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using Hydrogen;
using System;

namespace AbstractProtocol.AnonymousPipeComplex {

	[Serializable]
	public class FilePart {
		public byte[] Data { get; set; }

		internal static FilePart GenRandom() {
			return new FilePart { Data = Tools.Maths.RNG.NextBytes(Tools.Maths.RNG.Next(0, 64)) };
		}
	}

}
