using System;
using System.Collections.Generic;
using System.Linq;
using Org.BouncyCastle.Math;

namespace Sphere10.Framework.CryptoEx.EC.MuSigBuilder;

public class KeyCoefficientBuilder {
	private readonly MuSig _muSig;
	private readonly HashSet<byte[]> _publicKeys;
	public KeyCoefficientBuilder(MuSig muSig) {
		_muSig = muSig ?? throw new ArgumentNullException(nameof(muSig));
		// using a HashSet as we don't want to add duplicate public keys
		_publicKeys = new HashSet<byte[]>(ByteArrayEqualityComparer.Instance);
	}

	public void AddPublicKey(byte[] publicKey) {
		if (publicKey == null) {
			throw new ArgumentNullException(nameof(publicKey));
		}
		_publicKeys.Add(publicKey);
	}

	public Dictionary<byte[], BigInteger> Build() {
		if (!_publicKeys.Any()) {
			throw new Exception("you need to add public keys before calculating coefficients");
		}

		var result = new Dictionary<byte[], BigInteger>(ByteArrayEqualityComparer.Instance);

		var publicKeys = _publicKeys.ToArray();
		// 1. compute the public keys hash.
		var publicKeysHash = _muSig.ComputeEll(publicKeys);
		// 2. get second public key
		var secondPublicKey = _muSig.GetSecondPublicKey(publicKeys);
		// 3. compute key coefficients
		foreach (var publicKey in publicKeys) {
			var tmp = _muSig.ComputeKeyAggregationCoefficient(publicKeysHash,
				publicKey,
				secondPublicKey);
			result.Add(publicKey, tmp);
		}
		return result;
	}

}
