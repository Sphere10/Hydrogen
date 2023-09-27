// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

public abstract class OTSConfig : ICloneable {
	public readonly AMSOTS AMSID;
	public readonly CHF HashFunction;
	public readonly bool UsePublicKeyHashOptimization;
	public readonly int DigestSize;
	public readonly OTSKeySize KeySize;
	public readonly OTSKeySize PublicKeySize;
	public readonly OTSKeySize SignatureSize;

	protected OTSConfig(AMSOTS id, CHF chf, int digestSize, bool publicKeyIsHashed, OTSKeySize keySize, OTSKeySize publicKeySize, OTSKeySize signatureSize) {
		AMSID = id;
		HashFunction = chf;
		DigestSize = digestSize;
		UsePublicKeyHashOptimization = publicKeyIsHashed;
		KeySize = keySize;
		PublicKeySize = publicKeySize;
		SignatureSize = signatureSize;
	}

	public abstract object Clone();
}
