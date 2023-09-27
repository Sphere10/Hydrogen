// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Ugochukwu Mmaduekwe, Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using Org.BouncyCastle.Math;

namespace Hydrogen.CryptoEx.EC;

public class MuSigSessionCache {
	public bool FinalNonceParity { get; internal set; }
	public bool PublicKeyParity { get; internal set; }
	public byte[] FinalNonce { get; internal set; }
	public BigInteger Challenge { get; internal set; }
	public BigInteger NonceCoefficient { get; internal set; }
}
