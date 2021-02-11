using System;
using System.Diagnostics;
using System.Linq;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto.Agreement;
using Org.BouncyCastle.Crypto.EC;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC.Multiplier;
using Org.BouncyCastle.Security;

namespace Sphere10.Framework
{
    public static class CryptoLibHelper
    {
        private const int PKCS5_SALT_LEN = 8;
        private const int SALT_MAGIC_LEN = 8;
        private const int SALT_SIZE = 8;
        private const string SALT_MAGIC = "Salted__";

        private static readonly SecureRandom Random;
        private static readonly X9ECParameters CurveSecp256K1, CurveSecp384R1, CurveSect283K1, CurveSecp521R1;
        private static readonly ECDomainParameters DomainSecp256K1, DomainSecp384R1, DomainSect283K1, DomainSecp521R1;
        private static readonly PascalCoinIesEngine PascalCoinIesEngine;
        private static readonly IesParameterSpec IesParameterSpec;

        static CryptoLibHelper()
        {
            static IesParameterSpec GetIesParameterSpec() =>
                new(null, null, 256, 256, new byte[16], true);

            static PascalCoinIesEngine GetEciesPascalCoinCompatibilityEngine() =>
                new(new ECDHBasicAgreement(), new PascalCoinEciesKdfBytesGenerator
                        (DigestUtilities.GetDigest("SHA-512")),
                    MacUtilities.GetMac("HMAC-MD5"), new PaddedBufferedBlockCipher(new CbcBlockCipher(new AesEngine()),
                        new ZeroBytePadding()));

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
            // Init ECIES
            PascalCoinIesEngine = GetEciesPascalCoinCompatibilityEngine();
            IesParameterSpec = GetIesParameterSpec();
        }

        private static bool GetCurveAndDomainParameters(
            ushort ecOpensslNid, ref X9ECParameters curve, ref ECDomainParameters domain,
            bool raiseIfNotForPascal = true)
        {
            switch (ecOpensslNid)
            {
                case Constants.CT_NID_secp256k1:
                    curve = CurveSecp256K1;
                    domain = DomainSecp256K1;
                    return true;
                case Constants.CT_NID_secp384r1:
                    curve = CurveSecp384R1;
                    domain = DomainSecp384R1;
                    return true;
                case Constants.CT_NID_secp521r1:
                    curve = CurveSecp521R1;
                    domain = DomainSecp521R1;
                    return true;
                case Constants.CT_NID_sect283k1:
                    curve = CurveSect283K1;
                    domain = DomainSect283K1;
                    return true;
                default:
                    return raiseIfNotForPascal
                        ? throw new Exception($"Invalid Curve Type: {ecOpensslNid}")
                        : false;
            }
        }

        private static ECDomainParameters GetDomainParameters(ushort ecOpensslNid) =>
            ecOpensslNid switch
            {
                Constants.CT_NID_secp256k1 => DomainSecp256K1,
                Constants.CT_NID_secp384r1 => DomainSecp384R1,
                Constants.CT_NID_secp521r1 => DomainSecp521R1,
                Constants.CT_NID_sect283k1 => DomainSect283K1,
                _ => throw new Exception($"Invalid Curve Type: {ecOpensslNid}")
            };

        private static byte[] EVP_GetSalt()
        {
            var result = new byte[PKCS5_SALT_LEN];
            Random.NextBytes(result);
            return result;
        }

        private static bool EVP_GetKeyIV(
            byte[] password, byte[] salt, ref byte[] key, ref byte[] iv)
        {
            const int keySize = 32; // AES256 CBC Key Length
            const int ivSize = 16; // AES256 CBC IV Length
            Array.Resize(ref key, keySize);
            Array.Resize(ref iv, keySize);
            // Max size to start then reduce it at the end
            var digest = DigestUtilities.GetDigest("SHA-256"); // SHA2_256
            Debug.Assert(digest.GetDigestSize() >= keySize);
            Debug.Assert(digest.GetDigestSize() >= ivSize);
            // Derive Key First
            digest.BlockUpdate(password, 0, password.Length);
            if (salt?.Any() == true)
                digest.BlockUpdate(salt, 0, salt.Length);
            digest.DoFinal(key, 0);
            // Derive IV Next
            digest.Reset();
            digest.BlockUpdate(key, 0, key.Length);
            digest.BlockUpdate(password, 0, password.Length);
            if (salt?.Any() == true)
                digest.BlockUpdate(salt, 0, salt.Length);
            digest.DoFinal(iv, 0);
            Array.Resize(ref iv, ivSize);
            return true;
        }

        private static ECPublicKeyParameters GetCorrespondingPublicKey(ECPrivateKeyParameters privateKeyParameters)
        {
            var domainParameters = privateKeyParameters.Parameters;
            var ecPoint =
                (new FixedPointCombMultiplier() as ECMultiplier).Multiply(domainParameters.G, privateKeyParameters.D);
            return new ECPublicKeyParameters(privateKeyParameters.AlgorithmName, ecPoint, domainParameters);
        }

        public static byte[] DoSHA256(byte[] message) => Hashers.Hash(CHF.SHA2_256, message);

        public static byte[] DoRIPEMD160(byte[] message) => Hashers.Hash(CHF.RIPEMD_160, message);

        public static ECDSA_Public DoGetPublicKey(ushort ecOpensslNid, byte[] privateKey)
        {
            var domain = GetDomainParameters(ecOpensslNid);
            var d = new BigInteger(1, privateKey); // Obtain a positive bigInteger based on private key
            var privateKeyParameters = new ECPrivateKeyParameters("ECDSA", d, domain);
            var publicKey = GetCorrespondingPublicKey(privateKeyParameters);

            ECDSA_Public result = default;
            result.EC_OpenSSL_NID = ecOpensslNid;
            result.X = publicKey.Q.AffineXCoord.ToBigInteger().ToByteArrayUnsigned();
            result.Y = publicKey.Q.AffineYCoord.ToBigInteger().ToByteArrayUnsigned();
            return result;
        }

        public static byte[] DoGetRandomPrivateKey(ushort ecOpensslNid)
        {
            X9ECParameters curve = default;
            ECDomainParameters domain = default;
            GetCurveAndDomainParameters(ecOpensslNid, ref curve, ref domain);
            var keyPairGenerator = GeneratorUtilities.GetKeyPairGenerator("ECDSA");
            keyPairGenerator.Init(new ECKeyGenerationParameters(domain, Random));
            var keyPair = keyPairGenerator.GenerateKeyPair();
            return (keyPair.Private as ECPrivateKeyParameters)?.D.ToByteArray();
        }

        internal static bool DoECDSASign(ushort ecOpensslNid, byte[] privateKey, byte[] message,
            ref ECDSA_SIG signature)
        {
            var domain = GetDomainParameters(ecOpensslNid);
            var d = new BigInteger(1, privateKey); // Obtain a positive bigInteger based on private key
            var privateKeyParameters = new ECPrivateKeyParameters("ECDSA", d, domain);
            var parametersWithRandom = new ParametersWithRandom(privateKeyParameters, Random);
            var ecDsaSigner = new ECDsaSigner();
            ecDsaSigner.Init(true, parametersWithRandom);
            var signerResult = ecDsaSigner.GenerateSignature(message);
            signature.R = signerResult[0].ToByteArray();
            signature.S = signerResult[1].ToByteArray();
            return true;
        }

        internal static bool DoECDSAVerify(ECDSA_Public publicKey, byte[] message, ECDSA_SIG signature)
        {
            X9ECParameters curve = default;
            ECDomainParameters domain = default;
            GetCurveAndDomainParameters(publicKey.EC_OpenSSL_NID, ref curve, ref domain);
            var bigXCoord = new BigInteger(1, publicKey.X);
            var bigYCoord = new BigInteger(1, publicKey.Y);
            var ecPoint = curve.Curve.CreatePoint(bigXCoord, bigYCoord);
            var pubKeyParams = new ECPublicKeyParameters("ECDSA", ecPoint, domain);

            var sigR = new BigInteger(1, signature.R);
            var sigS = new BigInteger(1, signature.S);

            var ecDsaSigner = new ECDsaSigner();
            ecDsaSigner.Init(false, pubKeyParams);
            return ecDsaSigner.VerifySignature(message, sigR, sigS);
        }

        public static bool DoPascalCoinECIESEncrypt(ECDSA_Public publicKey, byte[] message,
            ref byte[] encryptedMessage)
        {
            X9ECParameters curve = default;
            ECDomainParameters domain = default;
            GetCurveAndDomainParameters(publicKey.EC_OpenSSL_NID, ref curve, ref domain);
            var bigXCoord = new BigInteger(1, publicKey.X);
            var bigYCoord = new BigInteger(1, publicKey.Y);
            var ecPoint = curve.Curve.CreatePoint(bigXCoord, bigYCoord);
            var pubKeyParams = new ECPublicKeyParameters("ECDSA", ecPoint, domain);
            // Encryption
            var iesCipher = new IesCipher(PascalCoinIesEngine);
            iesCipher.Init(true, pubKeyParams, IesParameterSpec, Random);
            encryptedMessage = iesCipher.DoFinal(message);
            return true;
        }

        public static bool DoPascalCoinECIESDecrypt(
            ushort ecOpensslNid, byte[] privateKey, byte[] encryptedMessage, ref byte[] decryptedMessage)
        {
            try
            {
                var domain = GetDomainParameters(ecOpensslNid);
                var d = new BigInteger(1, privateKey);
                var privateKeyParameters = new ECPrivateKeyParameters("ECDSA", d, domain);
                // Decryption
                var iesCipher = new IesCipher(PascalCoinIesEngine);
                iesCipher.Init(false, privateKeyParameters, IesParameterSpec, Random);
                decryptedMessage = iesCipher.DoFinal(encryptedMessage);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static byte[] DoPascalCoinAESEncrypt(byte[] message, byte[] password)
        {
            var salt = EVP_GetSalt();
            var key = Array.Empty<byte>();
            var iv = Array.Empty<byte>();
            EVP_GetKeyIV(password, salt, ref key, ref iv);
            var cipher = CipherUtilities.GetCipher("AES/CBC/PKCS7PADDING");
            var parametersWithIv = new ParametersWithIV
                (ParameterUtilities.CreateKeyParameter("AES", key), iv);

            cipher.Init(true, parametersWithIv); // init encryption cipher
            var blockSize = cipher.GetBlockSize();

            var buffer = new byte[message.Length + blockSize + SALT_MAGIC_LEN + PKCS5_SALT_LEN];

            var bufferStart = 0;

            var auxiliaryBuffer = ByteArrayUtils.FromString(SALT_MAGIC);
            Buffer.BlockCopy(auxiliaryBuffer, 0, buffer, bufferStart, SALT_MAGIC_LEN);
            bufferStart += SALT_MAGIC_LEN;
            Buffer.BlockCopy(salt, 0, buffer, bufferStart, PKCS5_SALT_LEN);
            bufferStart += PKCS5_SALT_LEN;

            var count = cipher.ProcessBytes(message, 0, message.Length, buffer, bufferStart);
            bufferStart += count;
            count = cipher.DoFinal(buffer, bufferStart);
            bufferStart += count;
            Array.Resize(ref buffer, bufferStart);
            return buffer;
        }

        public static bool DoPascalCoinAESDecrypt(byte[] encryptedMessage, byte[] password,
            ref byte[] decryptedMessage)
        {
            try
            {
                int srcStart;
                var salt = new byte[SALT_SIZE];
                var key = Array.Empty<byte>();
                var iv = Array.Empty<byte>();
                // First read the magic text and the salt - if any
                var chopped = new byte[SALT_MAGIC_LEN];
                Array.Copy(encryptedMessage, chopped, chopped.Length);
                if (encryptedMessage.Length >= SALT_MAGIC_LEN && ByteArrayUtils.ToString(chopped) == SALT_MAGIC)
                {
                    Buffer.BlockCopy(encryptedMessage, SALT_MAGIC_LEN, salt, 0, SALT_SIZE);
                    if (!EVP_GetKeyIV(password, salt, ref key, ref iv)) return false;
                    srcStart = SALT_MAGIC_LEN + SALT_SIZE;
                }
                else
                {
                    if (!EVP_GetKeyIV(password, null, ref key, ref iv)) return false;
                    srcStart = 0;
                }

                var cipher = CipherUtilities.GetCipher("AES/CBC/PKCS7PADDING");
                var parametersWithIv = new ParametersWithIV(ParameterUtilities.CreateKeyParameter("AES", key), iv);

                cipher.Init(false, parametersWithIv); // init decryption cipher

                var buffer = new byte[encryptedMessage.Length];
                var bufferStart = 0;
                var count = cipher.ProcessBytes(
                    encryptedMessage, srcStart, encryptedMessage.Length - srcStart, buffer,
                    bufferStart);
                bufferStart += count;
                count = cipher.DoFinal(buffer, bufferStart);
                bufferStart += count;
                Array.Resize(ref buffer, bufferStart);
                decryptedMessage = buffer;
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}