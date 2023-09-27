// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Ugochukwu Mmaduekwe, Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using Org.BouncyCastle.Crypto.EC;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;

namespace Hydrogen.CryptoEx.EC;

public class CurveData {
	public ECPoint G { get; internal set; }
	public ECCurve Curve { get; internal set; }
	public BigInteger P { get; internal set; }
	public BigInteger N { get; internal set; }
}


public class CurveParam {
	private static readonly Dictionary<ECDSAKeyType, CurveData> CurveParams = new();
	static CurveParam() {

		foreach (ECDSAKeyType keyType in Enum.GetValues(typeof(ECDSAKeyType))) {
			var curve = CustomNamedCurves.GetByName(keyType.ToString());
			CurveParams.Add(keyType, new CurveData { G = curve.G, Curve = curve.Curve, P = curve.Curve.Field.Characteristic, N = curve.N });
		}
	}

	public static CurveData GetCurveData(ECDSAKeyType keyType) {
		return CurveParams.TryGetValue(keyType, out var result) ? result : throw new Exception($"{nameof(keyType)} not found.");
	}
}
