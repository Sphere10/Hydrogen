using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto.EC;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Math.EC.Multiplier;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Encoders;

namespace Sphere10.Framework.CryptoEx
{

    public class ECDSA : StatelessDigitalSignatureScheme<ECDSA.PrivateKey, ECDSA.PublicKey>
    {
        private readonly ECDSAKeyType _keyType;

        private static readonly X9ECParameters CurveSecp256K1, CurveSecp384R1, CurveSect283K1, CurveSecp521R1;
        private static readonly ECDomainParameters DomainSecp256K1, DomainSecp384R1, DomainSect283K1, DomainSecp521R1;

        static ECDSA()
        {
            // Init Curves and Domains for quick usage
            CurveSecp256K1 = CustomNamedCurves.GetByName("SECP256K1");
            DomainSecp256K1 = new ECDomainParameters(CurveSecp256K1.Curve, CurveSecp256K1.G, CurveSecp256K1.N,
                CurveSecp256K1.H, CurveSecp256K1.GetSeed());
            CurveSecp384R1 = CustomNamedCurves.GetByName("SECP384R1");
            DomainSecp384R1 = new ECDomainParameters(CurveSecp384R1.Curve, CurveSecp384R1.G, CurveSecp384R1.N,
                CurveSecp384R1.H, CurveSecp384R1.GetSeed());
            CurveSect283K1 = CustomNamedCurves.GetByName("SECT283K1");
            DomainSect283K1 = new ECDomainParameters(CurveSect283K1.Curve, CurveSect283K1.G, CurveSect283K1.N,
                CurveSect283K1.H, CurveSect283K1.GetSeed());
            CurveSecp521R1 = CustomNamedCurves.GetByName("SECP521R1");
            DomainSecp521R1 = new ECDomainParameters(CurveSecp521R1.Curve, CurveSecp521R1.G, CurveSecp521R1.N,
                CurveSecp521R1.H, CurveSecp521R1.GetSeed());
        }

        private static void GetCurveAndDomainParameters(ECDSAKeyType keyType, ref X9ECParameters curve,
            ref ECDomainParameters domain)
        {
            switch (keyType)
            {
                case ECDSAKeyType.SECP256K1:
                    curve = CurveSecp256K1;
                    domain = DomainSecp256K1;
                    return;
                case ECDSAKeyType.SECP384R1:
                    curve = CurveSecp384R1;
                    domain = DomainSecp384R1;
                    return;
                case ECDSAKeyType.SECP521R1:
                    curve = CurveSecp521R1;
                    domain = DomainSecp521R1;
                    return;
                case ECDSAKeyType.SECT283K1:
                    curve = CurveSect283K1;
                    domain = DomainSect283K1;
                    return;
                default:
                    throw new Exception($"Invalid Curve Type: {Enum.GetName(typeof(ECDSAKeyType), keyType)}");
            }
        }

        private static ECDomainParameters GetDomainParameters(ECDSAKeyType keyType) =>
            keyType switch
            {
                ECDSAKeyType.SECP256K1 => DomainSecp256K1,
                ECDSAKeyType.SECP384R1 => DomainSecp384R1,
                ECDSAKeyType.SECP521R1 => DomainSecp521R1,
                ECDSAKeyType.SECT283K1 => DomainSect283K1,
                _ => throw new Exception($"Invalid Curve Type: {Enum.GetName(typeof(ECDSAKeyType), keyType)}")
            };

        public ECDSA(ECDSAKeyType keyType) : this(keyType, CHF.SHA2_256)
        {
        }

        public ECDSA(ECDSAKeyType keyType, CHF digestCHF) : base(digestCHF) => _keyType = keyType;

        public override bool TryParsePublicKey(ReadOnlySpan<byte> bytes, out PublicKey publicKey)
            => PublicKey.TryParse(bytes, _keyType, out publicKey);

        public override bool TryParsePrivateKey(ReadOnlySpan<byte> bytes, out PrivateKey privateKey)
            => PrivateKey.TryParse(bytes, _keyType, out privateKey);

        public override PrivateKey GeneratePrivateKey(ReadOnlySpan<byte> secret)
        {
            TryParsePrivateKey(secret.ToArray(), out var privateKey);
            return privateKey;
        }

        public override PublicKey DerivePublicKey(PrivateKey privateKey)
            => PublicKey.DoGetPublicKey(_keyType, privateKey);

        public override bool IsPublicKey(PrivateKey privateKey, ReadOnlySpan<byte> publicKeyBytes)
            => DerivePublicKey(privateKey).RawBytes.AsSpan().SequenceEqual(publicKeyBytes);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte[] Sign(PrivateKey privateKey, ReadOnlySpan<byte> message)
        {
            var messageDigest = CalculateMessageDigest(message);
            return SignDigest(privateKey, messageDigest);
        }

        public override byte[] SignDigest(PrivateKey privateKey, ReadOnlySpan<byte> messageDigest)
            => privateKey.Sign(messageDigest);

        public override bool VerifyDigest(ReadOnlySpan<byte> signature, ReadOnlySpan<byte> messageDigest,
            ReadOnlySpan<byte> publicKey) =>
            TryParsePublicKey(publicKey, out var pubKey) && pubKey.Verify(signature, messageDigest);


        public abstract class Key : IKey
        {
            protected Key(byte[] immutableRawBytes)
            {
                RawBytes = immutableRawBytes;
            }

            public readonly byte[] RawBytes;

            public override bool Equals(object obj)
            {
                if (obj is Key key)
                {
                    return Equals(key);
                }
                return false;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private bool Equals(Key other) => RawBytes.SequenceEqual(other.RawBytes);

            public override int GetHashCode() => (RawBytes != null ? RawBytes.GetHashCode() : 0);

            #region IKey

            byte[] IKey.RawBytes => RawBytes;

            #endregion
        }

        public class PrivateKey : Key, IPrivateKey
        {
            private ECPrivateKeyParameters PrivateKeyParameters { get; }
            private ParametersWithRandom ParametersWithRandom { get; }
            private static SecureRandom SecureRandom { get; } = new();

            public static Dictionary<ECDSAKeyType, BigInteger> KeyTypeOrders { get; } =
                new()
                {
                    {
                        ECDSAKeyType.SECP256K1,
                        new BigInteger(1,
                            Hex.DecodeStrict("FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEBAAEDCE6AF48A03BBFD25E8CD0364141"))
                    },
                    {
                        ECDSAKeyType.SECP384R1,
                        new BigInteger(1,
                            Hex.DecodeStrict(
                                "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFC7634D81F4372DDF581A0DB248B0A77AECEC196ACCC52973"))
                    },
                    {
                        ECDSAKeyType.SECP521R1,
                        new BigInteger(1,
                            Hex.DecodeStrict(
                                "01FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFA51868783BF2F966B7FCC0148F709A5D03BB5C9B8899C47AEBB6FB71E91386409"))
                    },
                    {
                        ECDSAKeyType.SECT283K1,
                        new BigInteger(1,
                            Hex.DecodeStrict(
                                "01FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFE9AE2ED07577265DFF7F94451E061E163C61"))
                    },
                };

            internal PrivateKey(byte[] rawKeyBytes, ECDSAKeyType ecdsaKeyType) : base(rawKeyBytes)
            {
                var domain = GetDomainParameters(ecdsaKeyType);
                var d = new BigInteger(1, rawKeyBytes); // Obtain a positive bigInteger based on private key
                PrivateKeyParameters = new ECPrivateKeyParameters("ECDSA", d, domain);
                ParametersWithRandom = new ParametersWithRandom(PrivateKeyParameters, SecureRandom);
            }

            internal byte[] Sign(ReadOnlySpan<byte> messageDigest)
            {
                var ecDsaSigner = SignerUtilities.GetSigner("NONEwithECDSA");
                ecDsaSigner.Init(true, ParametersWithRandom);
                ecDsaSigner.BlockUpdate(messageDigest.ToArray(), 0, messageDigest.Length);
                return ecDsaSigner.GenerateSignature();
            }

            public static bool TryParse(ReadOnlySpan<byte> rawBytes, ECDSAKeyType ecdsaKeyType,
                out PrivateKey privateKey)
            {
                privateKey = null;
                if (rawBytes.Length <= 0) return false;
                if (!KeyTypeOrders.TryGetValue(ecdsaKeyType, out var order)) return false;
                if (order == null) return false; // should never happens though.
                var d = new BigInteger(1, rawBytes.ToArray());
                if (d.CompareTo(BigInteger.One) < 0 || d.CompareTo(order) >= 0) return false;
                privateKey = new PrivateKey(rawBytes.ToArray(), ecdsaKeyType);
                return true;
            }

            public static byte[] DoGetRandomPrivateKey(ECDSAKeyType keyType)
            {
                X9ECParameters curve = default;
                ECDomainParameters domain = default;
                GetCurveAndDomainParameters(keyType, ref curve, ref domain);
                var keyPairGenerator = GeneratorUtilities.GetKeyPairGenerator("ECDSA");
                keyPairGenerator.Init(new ECKeyGenerationParameters(domain, SecureRandom));
                var keyPair = keyPairGenerator.GenerateKeyPair();
                return (keyPair.Private as ECPrivateKeyParameters)?.D.ToByteArray();
            }
        }

        public class PublicKey : Key, IPublicKey
        {
            private ECPublicKeyParameters PublicKeyParameters { get; }
            private ECPoint Point { get; }

            internal PublicKey(byte[] rawKeyBytes, ECDSAKeyType ecdsaKeyType) : base(rawKeyBytes)
            {
                X9ECParameters curve = default;
                ECDomainParameters domain = default;
                GetCurveAndDomainParameters(ecdsaKeyType, ref curve, ref domain);
                Point = curve.Curve.DecodePoint(rawKeyBytes);
                PublicKeyParameters = new ECPublicKeyParameters("ECDSA", Point, domain);
            }

            internal bool Verify(ReadOnlySpan<byte> signature, ReadOnlySpan<byte> messageDigest)
            {
                var ecDsaSigner = SignerUtilities.GetSigner("NONEwithECDSA");
                ecDsaSigner.Init(false, PublicKeyParameters);
                ecDsaSigner.BlockUpdate(messageDigest.ToArray(), 0, messageDigest.Length);
                return ecDsaSigner.VerifySignature(signature.ToArray());
            }

            private static ECPublicKeyParameters GetCorrespondingPublicKey(ECPrivateKeyParameters privateKeyParameters)
            {
                var domainParameters = privateKeyParameters.Parameters;
                var ecPoint =
                    (new FixedPointCombMultiplier() as ECMultiplier).Multiply(domainParameters.G,
                        privateKeyParameters.D);
                return new ECPublicKeyParameters(privateKeyParameters.AlgorithmName, ecPoint, domainParameters);
            }

            public static bool TryParse(ReadOnlySpan<byte> rawBytes, ECDSAKeyType keyType, out PublicKey publicKey)
            {
                publicKey = null;
                if (rawBytes.Length <= 0) return false;
                publicKey = new PublicKey(rawBytes.ToArray(), keyType);
                return true;
            }

            public static PublicKey DoGetPublicKey(ECDSAKeyType keyType, PrivateKey privateKey)
            {
                var domain = GetDomainParameters(keyType);
                var d = new BigInteger(1, privateKey.RawBytes); // Obtain a positive bigInteger based on private key
                var privateKeyParameters = new ECPrivateKeyParameters("ECDSA", d, domain);
                var publicKey = GetCorrespondingPublicKey(privateKeyParameters);

                return new PublicKey(publicKey.Q.GetEncoded(), keyType);
            }
        }
    }

}