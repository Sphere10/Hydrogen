// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Ugochukwu Mmaduekwe, Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto.EC;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Math.EC.Multiplier;
using Org.BouncyCastle.Security;
using Hydrogen.CryptoEx.EC.IES;

namespace Hydrogen.CryptoEx.EC;

public class ECDSA : StatelessDigitalSignatureScheme<ECDSA.PrivateKey, ECDSA.PublicKey> {
	private readonly ECDSAKeyType _keyType;
	private readonly X9ECParameters _curveParams;
	private readonly ECDomainParameters _domainParams;
	private readonly SecureRandom _secureRandom;

	private bool ValidatePrivateKey(BigInteger scalar) {
		// 1 to n - 1
		return scalar.CompareTo(BigInteger.One) >= 0 && scalar.CompareTo(N.Subtract(BigInteger.One)) <= 0;
	}

	public ECDSA(ECDSAKeyType keyType) : this(keyType, CHF.SHA2_256) {
	}

	public ECDSA(ECDSAKeyType keyType, CHF digestCHF) : base(digestCHF) {
		_keyType = keyType;
		_curveParams = CustomNamedCurves.GetByName(keyType.ToString());
		_domainParams = new ECDomainParameters(_curveParams.Curve,
			_curveParams.G,
			_curveParams.N,
			_curveParams.H,
			_curveParams.GetSeed());
		_secureRandom = new SecureRandom();
		Traits = Traits & DigitalSignatureSchemeTraits.ECDSA & DigitalSignatureSchemeTraits.SupportsIES;
	}

	public override IIESAlgorithm IES => new ECIES(); // defaults to a pascalcoin style ECIES
	private ECCurve Curve => _curveParams.Curve;
	private BigInteger N => _curveParams.N;
	public int KeySize => (Curve.FieldSize + 7) >> 3;
	public int CompressedPublicKeySize => KeySize + 1;

	public override bool TryParsePublicKey(ReadOnlySpan<byte> bytes, out PublicKey publicKey) {
		publicKey = null;
		if (bytes.Length != CompressedPublicKeySize) {
			return false;
		}
		var pubKey = bytes.ToArray();
		// we only allow compressed ECDSA public keys
		if (pubKey[0] != 0x02 && pubKey[0] != 0x03) {
			return false;
		}
		try {
			publicKey = new PublicKey(Curve.DecodePoint(pubKey), _keyType, _curveParams, _domainParams);
			return true;
		} catch (Exception) {
			return false;
		}
	}

	public override bool TryParsePrivateKey(ReadOnlySpan<byte> bytes, out PrivateKey privateKey) {
		if (bytes.Length != KeySize) {
			privateKey = null;
			return false;
		}
		var secretKey = bytes.ToArray();
		var d = BigIntegerUtils.BytesToBigIntegerPositive(secretKey);
		if (!ValidatePrivateKey(d)) {
			privateKey = null;
			return false;
		}
		privateKey = new PrivateKey(secretKey, _keyType, _curveParams, _domainParams);
		return true;
	}

	public override PrivateKey GeneratePrivateKey(ReadOnlySpan<byte> seed) {
		var keyPairGenerator = GeneratorUtilities.GetKeyPairGenerator("ECDSA");
		// add seed to RNG
		_secureRandom.SetSeed(seed.ToArray());
		keyPairGenerator.Init(new ECKeyGenerationParameters(_domainParams, _secureRandom));
		var keyPair = keyPairGenerator.GenerateKeyPair();
		var privateKeyBytes = BigIntegerUtils.BigIntegerToBytes((keyPair.Private as ECPrivateKeyParameters)?.D, KeySize);
		return (PrivateKey)this.ParsePrivateKey(privateKeyBytes);
	}

	public override PublicKey DerivePublicKey(PrivateKey privateKey) {
		var privateKeyParameters = new ECPrivateKeyParameters("ECDSA", privateKey.AsInteger.Value, _domainParams);
		var domainParameters = privateKeyParameters.Parameters;
		var ecPoint = (new FixedPointCombMultiplier() as ECMultiplier).Multiply(domainParameters.G, privateKeyParameters.D);
		var pubKeyParams = new ECPublicKeyParameters(privateKeyParameters.AlgorithmName, ecPoint, domainParameters);
		return new PublicKey(pubKeyParams.Q, _keyType, _curveParams, _domainParams);
	}

	public override bool IsPublicKey(PrivateKey privateKey, ReadOnlySpan<byte> publicKeyBytes)
		=> DerivePublicKey(privateKey).RawBytes.AsSpan().SequenceEqual(publicKeyBytes);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public byte[] Sign(PrivateKey privateKey, ReadOnlySpan<byte> message) {
		var messageDigest = CalculateMessageDigest(message);
		return SignDigest(privateKey, messageDigest);
	}

	public override byte[] SignDigest(PrivateKey privateKey, ReadOnlySpan<byte> messageDigest) {
		var signer = CustomEcDsaSigner.GetRfc6979DeterministicSigner();
		var parametersWithRandom = new ParametersWithRandom(privateKey.Parameters.Value, _secureRandom);
		signer.Init(true, parametersWithRandom);
		signer.BlockUpdate(messageDigest.ToArray(), 0, messageDigest.Length);
		return signer.GenerateSignature();
	}

	public override bool VerifyDigest(ReadOnlySpan<byte> signature, ReadOnlySpan<byte> messageDigest, ReadOnlySpan<byte> publicKey) {
		if (!TryParsePublicKey(publicKey, out var pubKey))
			return false;
		var signer = CustomEcDsaSigner.GetRfc6979DeterministicSigner();
		signer.Init(false, pubKey.Parameters.Value);
		signer.BlockUpdate(messageDigest.ToArray(), 0, messageDigest.Length);
		return signer.VerifySignature(signature.ToArray());
	}


	public abstract class Key : IKey {
		protected Key(byte[] immutableRawBytes, ECDSAKeyType keyType, X9ECParameters curveParams, ECDomainParameters domainParams) {
			RawBytes = immutableRawBytes;
			KeyType = keyType;
			CurveParams = curveParams;
			DomainParams = domainParams;
			AsInteger = Tools.Values.Future.LazyLoad(() => BigIntegerUtils.BytesToBigIntegerPositive(RawBytes));
		}

		public ECDSAKeyType KeyType { get; }

		public byte[] RawBytes { get; }

		internal X9ECParameters CurveParams { get; }

		internal ECDomainParameters DomainParams { get; }

		internal IFuture<BigInteger> AsInteger { get; }

		public override bool Equals(object obj) {
			if (obj is Key key) {
				return Equals(key);
			}
			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool Equals(Key other) => RawBytes.SequenceEqual(other.RawBytes);

		public override int GetHashCode() => (RawBytes != null ? RawBytes.GetHashCode() : 0);

	}


	public class PrivateKey : Key, IPrivateKey {
		public PrivateKey(byte[] rawKeyBytes, ECDSAKeyType keyType, X9ECParameters curveParams, ECDomainParameters domainParams) :
			base(rawKeyBytes, keyType, curveParams, domainParams) {
			Parameters = Tools.Values.Future.LazyLoad(() => new ECPrivateKeyParameters("ECDSA", AsInteger.Value, DomainParams));
		}

		public IFuture<ECPrivateKeyParameters> Parameters { get; }
	}


	public class PublicKey : Key, IPublicKey {
		public PublicKey(ECPoint point, ECDSAKeyType keyType, X9ECParameters curveParams, ECDomainParameters domainParams) :
			base(point.GetEncoded(true), keyType, curveParams, domainParams) {
			AsPoint = Tools.Values.Future.LazyLoad(() => point);
			Parameters = Tools.Values.Future.LazyLoad(() => new ECPublicKeyParameters("ECDSA", AsPoint.Value, DomainParams));
		}

		public IFuture<ECPublicKeyParameters> Parameters { get; }
		internal IFuture<ECPoint> AsPoint { get; }

	}
}
