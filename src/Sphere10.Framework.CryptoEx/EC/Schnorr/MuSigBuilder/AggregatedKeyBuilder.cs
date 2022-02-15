using System;
using System.Collections.Generic;
using System.Linq;
using Org.BouncyCastle.Math;

namespace Sphere10.Framework.CryptoEx.EC.MuSigBuilder;

public class AggregatedKeyBuilder {
	private readonly MuSig _muSig;
	private readonly Dictionary<byte[], BigInteger> _keyCoefficients;
	public AggregatedKeyBuilder(MuSig muSig, Dictionary<byte[], BigInteger> keyCoefficients) {
		_muSig = muSig ?? throw new ArgumentNullException(nameof(muSig));
		_keyCoefficients = keyCoefficients ?? throw new ArgumentNullException(nameof(keyCoefficients));
	}

	public PublicKeyAggregationData Build() {
		if (!_keyCoefficients.Any()) {
			throw new Exception("you need to have calculated key coefficients before aggregating keys");
		}

		// combine public keys.
		// use ordered form
		var keyCoefficients = new List<Tuple<byte[], BigInteger>>();
		foreach (var (key, value) in _keyCoefficients) {
			keyCoefficients.Add(new Tuple<byte[], BigInteger>(key, value));
		}
		return _muSig.CombinePublicKeys(keyCoefficients.Select(x => x.Item2).ToArray(), keyCoefficients.Select(x => x.Item1).ToArray());
	}
}
