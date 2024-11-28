// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using Hydrogen.CryptoEx;

namespace Hydrogen.DApp.Core.Mining;

public class CHFhasher : IMiningHasher {
	public CHF Algo { get; set; }

	public string GetDescription() {
		return Algo.ToString();
	}
	public byte[] Hash(byte[] input) {
		return Hashers.Hash(Algo, input);
	}
}


public class RandomHash2Hasher : IMiningHasher {
	public string GetDescription() {
		return "RH2";
	}
	public byte[] Hash(byte[] input) {
		return RandomHash2.Compute(input);
	}
}
