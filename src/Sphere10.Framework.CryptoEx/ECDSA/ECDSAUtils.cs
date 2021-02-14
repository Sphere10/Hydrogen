using System;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto.EC;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC.Multiplier;
using Org.BouncyCastle.Security;

namespace Sphere10.Framework.CryptoEx
{

    public static class ECDSAUtils
    {
        private static readonly SecureRandom Random;
        private static readonly X9ECParameters CurveSecp256K1, CurveSecp384R1, CurveSect283K1, CurveSecp521R1;
        private static readonly ECDomainParameters DomainSecp256K1, DomainSecp384R1, DomainSect283K1, DomainSecp521R1;

        static ECDSAUtils()
        {
            Random = new SecureRandom();
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

        private static ECPublicKeyParameters GetCorrespondingPublicKey(ECPrivateKeyParameters privateKeyParameters)
        {
            var domainParameters = privateKeyParameters.Parameters;
            var ecPoint =
                (new FixedPointCombMultiplier() as ECMultiplier).Multiply(domainParameters.G, privateKeyParameters.D);
            return new ECPublicKeyParameters(privateKeyParameters.AlgorithmName, ecPoint, domainParameters);
        }

        public static IPublicKey DoGetPublicKey(ECDSAKeyType keyType, IPrivateKey privateKey)
        {
            var domain = GetDomainParameters(keyType);
            var d = new BigInteger(1, privateKey.RawBytes); // Obtain a positive bigInteger based on private key
            var privateKeyParameters = new ECPrivateKeyParameters("ECDSA", d, domain);
            var publicKey = GetCorrespondingPublicKey(privateKeyParameters);

            return new ECDSA.PublicKey(publicKey.Q.GetEncoded());
        }

        public static byte[] DoGetRandomPrivateKey(ECDSAKeyType keyType)
        {
            X9ECParameters curve = default;
            ECDomainParameters domain = default;
            GetCurveAndDomainParameters(keyType, ref curve, ref domain);
            var keyPairGenerator = GeneratorUtilities.GetKeyPairGenerator("ECDSA");
            keyPairGenerator.Init(new ECKeyGenerationParameters(domain, Random));
            var keyPair = keyPairGenerator.GenerateKeyPair();
            return (keyPair.Private as ECPrivateKeyParameters)?.D.ToByteArray();
        }

        internal static byte[] DoECDSASign(ECDSAKeyType keyType, IPrivateKey privateKey,
            ReadOnlySpan<byte> messageDigest)
        {
            var domain = GetDomainParameters(keyType);
            var d = new BigInteger(1, privateKey.RawBytes); // Obtain a positive bigInteger based on private key
            var privateKeyParameters = new ECPrivateKeyParameters("ECDSA", d, domain);
            var parametersWithRandom = new ParametersWithRandom(privateKeyParameters, Random);
            var ecDsaSigner = SignerUtilities.GetSigner("NONEwithECDSA");
            ecDsaSigner.Init(true, parametersWithRandom);
            ecDsaSigner.BlockUpdate(messageDigest.ToArray(), 0, messageDigest.Length);
            return ecDsaSigner.GenerateSignature();
        }

        internal static bool DoECDSAVerify(ECDSAKeyType keyType, ReadOnlySpan<byte> signature,
            ReadOnlySpan<byte> messageDigest,
            ReadOnlySpan<byte> publicKey)
        {
            X9ECParameters curve = default;
            ECDomainParameters domain = default;
            GetCurveAndDomainParameters(keyType, ref curve, ref domain);
            var ecPoint = curve.Curve.DecodePoint(publicKey.ToArray());
            var pubKeyParams = new ECPublicKeyParameters("ECDSA", ecPoint, domain);

            var ecDsaSigner = SignerUtilities.GetSigner("NONEwithECDSA");
            ecDsaSigner.Init(false, pubKeyParams);
            ecDsaSigner.BlockUpdate(messageDigest.ToArray(), 0, messageDigest.Length);
            return ecDsaSigner.VerifySignature(signature.ToArray());
        }
    }

}