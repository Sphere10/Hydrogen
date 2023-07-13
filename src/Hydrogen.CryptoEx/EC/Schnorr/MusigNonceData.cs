// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Ugochukwu Mmaduekwe, Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using Org.BouncyCastle.Math;
using Org.BouncyCastle.Utilities;

namespace Hydrogen.CryptoEx.EC;

internal class MuSigNonceData {
	internal byte[] PrivateNonce { get; set; }
	internal byte[] PublicNonce { get; set; }
}


internal class MuSigPrivateNonce {
	internal byte[] K1 { get; set; }
	internal byte[] K2 { get; set; }

	internal byte[] GetFullNonce() {
		return Arrays.Concatenate(K1, K2);
	}
}


internal class MuSigPublicNonce {
	internal byte[] R1 { get; set; }
	internal byte[] R2 { get; set; }

	internal byte[] GetFullNonce() {
		return Arrays.Concatenate(R1, R2);
	}
}


public class AggregatedPublicNonce {
	public byte[] AggregatedNonce { get; internal set; }
	public byte[] FinalNonce { get; internal set; }
	public BigInteger NonceCoefficient { get; internal set; }
	public bool FinalNonceParity { get; internal set; }
}
