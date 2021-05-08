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
using Sphere10.Framework.CryptoEx.EC.IES;

namespace Sphere10.Framework.CryptoEx.EC {

	public class ECDSA : StatelessDigitalSignatureScheme<ECDSA.PrivateKey, ECDSA.PublicKey> {
		private readonly ECDSAKeyType _keyType;
		private readonly X9ECParameters _curveParams;
		private readonly ECDomainParameters _domainParams;
		private readonly SecureRandom _secureRandom;

		public ECDSA(ECDSAKeyType keyType) : this(keyType, CHF.SHA2_256) {
		}

		public ECDSA(ECDSAKeyType keyType, CHF digestCHF) : base(digestCHF) {
			_keyType = keyType;
			_curveParams = CustomNamedCurves.GetByName(keyType.ToString());
			_domainParams = new ECDomainParameters(_curveParams.Curve, _curveParams.G, _curveParams.N, _curveParams.H, _curveParams.GetSeed());
			_secureRandom = new SecureRandom();
			Traits = Traits & DigitalSignatureSchemeTraits.ECDSA & DigitalSignatureSchemeTraits.SupportsIES;
		}

		public override IIESAlgorithm IES => new ECIES();  // defaults to a pascalcoin style ECIES

		public override bool TryParsePublicKey(ReadOnlySpan<byte> bytes, out PublicKey publicKey) {
			if (bytes.Length <= 0) {  // TODO: VALIDATE UPPER BOUND OF rawBytes as well!
				publicKey = null;
				return false;
			}
			publicKey = new PublicKey(bytes.ToArray(), _keyType, _curveParams, _domainParams);
			return true;
		}

		public override bool TryParsePrivateKey(ReadOnlySpan<byte> bytes, out PrivateKey privateKey) {
			if (bytes.Length <= 0) {
				privateKey = null;
				return false;
			}
			var order = _keyType.GetAttribute<KeyTypeOrderAttribute>().Value;
			var d = new BigInteger(1, bytes.ToArray());
			if (d.CompareTo(BigInteger.One) < 0 || d.CompareTo(order) >= 0) {
				privateKey = null;
				return false;
			}
			privateKey = new PrivateKey(bytes.ToArray(), _keyType, _curveParams, _domainParams);
			return true;
		}

		public override PrivateKey GeneratePrivateKey(ReadOnlySpan<byte> secret) {
			var keyPairGenerator = GeneratorUtilities.GetKeyPairGenerator("ECDSA");
			keyPairGenerator.Init(new ECKeyGenerationParameters(_domainParams, _secureRandom));
			var keyPair = keyPairGenerator.GenerateKeyPair();
			var privateKeyBytes = (keyPair.Private as ECPrivateKeyParameters)?.D.ToByteArray();
			return (PrivateKey)this.ParsePrivateKey(privateKeyBytes);
		}

		public override PublicKey DerivePublicKey(PrivateKey privateKey) {
			var privateKeyParameters = new ECPrivateKeyParameters("ECDSA", privateKey.AsInteger.Value, _domainParams);
			var domainParameters = privateKeyParameters.Parameters;
			var ecPoint = (new FixedPointCombMultiplier() as ECMultiplier).Multiply(domainParameters.G, privateKeyParameters.D);
			var pubKeyParams = new ECPublicKeyParameters(privateKeyParameters.AlgorithmName, ecPoint, domainParameters);
			return new PublicKey(pubKeyParams.Q.GetEncoded(), _keyType, _curveParams, _domainParams);
		}

		public override bool IsPublicKey(PrivateKey privateKey, ReadOnlySpan<byte> publicKeyBytes)
			=> DerivePublicKey(privateKey).RawBytes.AsSpan().SequenceEqual(publicKeyBytes);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public byte[] Sign(PrivateKey privateKey, ReadOnlySpan<byte> message) {
			var messageDigest = CalculateMessageDigest(message);
			return SignDigest(privateKey, messageDigest);
		}

		public override byte[] SignDigest(PrivateKey privateKey, ReadOnlySpan<byte> messageDigest) {
			var signer = SignerUtilities.GetSigner("NONEwithECDSA");
			var parametersWithRandom = new ParametersWithRandom(privateKey.Parameters.Value, _secureRandom);
			signer.Init(true, parametersWithRandom);
			signer.BlockUpdate(messageDigest.ToArray(), 0, messageDigest.Length);
			return signer.GenerateSignature();
		}

		public override bool VerifyDigest(ReadOnlySpan<byte> signature, ReadOnlySpan<byte> messageDigest, ReadOnlySpan<byte> publicKey) { 
			if (!TryParsePublicKey(publicKey, out var pubKey))
				return false;
			var signer = SignerUtilities.GetSigner("NONEwithECDSA");
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
				AsInteger = Tools.Values.LazyLoad(() => new BigInteger(1, RawBytes));
				AsPoint = Tools.Values.LazyLoad(() => curveParams.Curve.DecodePoint(RawBytes));
			}

			public ECDSAKeyType KeyType { get; }

			public byte[] RawBytes { get; }

			internal X9ECParameters CurveParams { get; }

			internal ECDomainParameters DomainParams { get; }

			internal IFuture<BigInteger> AsInteger { get; }

			internal IFuture<ECPoint> AsPoint { get; }

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
			public PrivateKey(byte[] rawKeyBytes, ECDSAKeyType keyType, X9ECParameters curveParams, ECDomainParameters domainParams) : base(rawKeyBytes, keyType, curveParams, domainParams) {
				Parameters = Tools.Values.LazyLoad( () => new ECPrivateKeyParameters("ECDSA", AsInteger.Value, DomainParams));
			}

			public IFuture<ECPrivateKeyParameters> Parameters { get; }
		}

		public class PublicKey : Key, IPublicKey {
			public PublicKey(byte[] rawKeyBytes, ECDSAKeyType keyType, X9ECParameters curveParams, ECDomainParameters domainParams) :
				base(rawKeyBytes, keyType, curveParams, domainParams) {
				Parameters = Tools.Values.LazyLoad(() => new ECPublicKeyParameters("ECDSA", AsPoint.Value, DomainParams));
			}

			public IFuture<ECPublicKeyParameters> Parameters { get; }

		}
	}

}