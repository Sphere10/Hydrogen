using NUnit.Framework;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Math;
using Sphere10.Framework.CryptoEx.EC;
using System;
using System.IO;
using System.Linq;
using System.Text;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace Sphere10.Framework.CryptoEx.Tests
{
    [TestFixture]
    [Parallelizable(ParallelScope.Children)]
    public class ECDSATests
    {
        private const string InvalidDerSignature = "Invalid DER Signature";
        private const string InvalidRAndSSignature = "Invalid R and S Signature";

        private static string RandomString(int length)
        {
            var rng = new Random(31337);
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[rng.Next(s.Length)]).ToArray());
        }

        private static bool IsLowS(BigInteger order, BigInteger s)
        {
            return s.CompareTo(order.ShiftRight(1)) <= 0;
        }

        public static BigInteger[] DerSig_To_R_And_S(byte[] derSig)
        {
            if (derSig == null)
            {
                throw new ArgumentNullException(nameof(derSig));
            }

            try
            {
                var decoder = new Asn1InputStream(derSig);
                var derSequence = decoder.ReadObject() as DerSequence;
                if (derSequence is not { Count: 2 })
                {
                    throw new FormatException(InvalidDerSignature);
                }

                var r = (derSequence[0] as DerInteger)?.Value;
                var s = (derSequence[1] as DerInteger)?.Value;
                return new[] { r, s };
            }
            catch (Exception ex)
            {
                throw new FormatException(InvalidDerSignature, ex);
            }
        }

        private static byte[] R_And_S_To_DerSig(BigInteger[] rs)
        {
            if (rs == null)
                throw new ArgumentNullException(nameof(rs));
            if (rs.Length != 2)
            {
                throw new ArgumentException(InvalidRAndSSignature, nameof(rs));
            }

            var outStream = new MemoryStream();
            var generator = new DerSequenceGenerator(outStream);
            generator.AddObject(new DerInteger(rs[0]));
            generator.AddObject(new DerInteger(rs[1]));
            generator.Close();
            return outStream.ToArray();
        }

        public static byte[] CanonicalizeSig(BigInteger order, byte[] sig)
        {
            var rs = DerSig_To_R_And_S(sig);
            if (!IsLowS(order, rs[1]))
            {
                rs[1] = order.Subtract(rs[1]);
                return R_And_S_To_DerSig(rs);
            }
            return sig;
        }

        [Test, Repeat(64)]
        [TestCase(ECDSAKeyType.SECP256K1)]
        [TestCase(ECDSAKeyType.SECP384R1)]
        [TestCase(ECDSAKeyType.SECP521R1)]
        [TestCase(ECDSAKeyType.SECT283K1)]
        public void SignVerify_Basic(ECDSAKeyType keyType)
        {
            var ecdsa = new ECDSA(keyType);
            var secret = new byte[] { 0, 1, 2, 3, 4 }; // deterministic secret
            var privateKey = ecdsa.GeneratePrivateKey(secret);
            var publicKey = ecdsa.DerivePublicKey(privateKey);
            var message = Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog");
            var sig = ecdsa.Sign(privateKey, message);
            Assert.IsTrue(ecdsa.Verify(sig, message, publicKey));
        }

        [Test]
        [TestCase(ECDSAKeyType.SECP256K1)]
        [TestCase(ECDSAKeyType.SECP384R1)]
        [TestCase(ECDSAKeyType.SECP521R1)]
        [TestCase(ECDSAKeyType.SECT283K1)]
        public void IsPublicKey(ECDSAKeyType keyType)
        {
            var ecdsa = new ECDSA(keyType);
            var secret = new byte[] { 0, 1, 2, 3, 4 }; // deterministic secret
            var privateKey = ecdsa.GeneratePrivateKey(secret);
            var publicKey = ecdsa.DerivePublicKey(privateKey);
            Assert.IsTrue(ecdsa.IsPublicKey(privateKey, publicKey.RawBytes));
        }

        [Test]
        [TestCase(new byte[] { }, ECDSAKeyType.SECP256K1)]
        [TestCase(new byte[] { 0, 0 }, ECDSAKeyType.SECP256K1)]
        [TestCase(new byte[] { }, ECDSAKeyType.SECP384R1)]
        [TestCase(new byte[] { 0, 0 }, ECDSAKeyType.SECP384R1)]
        [TestCase(new byte[] { }, ECDSAKeyType.SECP521R1)]
        [TestCase(new byte[] { 0, 0 }, ECDSAKeyType.SECP521R1)]
        [TestCase(new byte[] { }, ECDSAKeyType.SECT283K1)]
        [TestCase(new byte[] { 0, 0 }, ECDSAKeyType.SECT283K1)]
        public void VerifyThatTryParsePrivateKeyFailsEarlyForBadKeys(byte[] badRawKey, ECDSAKeyType keyType)
        {
            var ecdsa = new ECDSA(keyType);
            Assert.IsFalse(ecdsa.TryParsePrivateKey(badRawKey, out _));
            var order = keyType.GetAttribute<KeyTypeOrderAttribute>().Value;
            Assert.IsFalse(ecdsa.TryParsePrivateKey(order.ToByteArrayUnsigned(), out _));
        }

        [Test, Repeat(64)]
        [TestCase(ECDSAKeyType.SECP256K1)]
        [TestCase(ECDSAKeyType.SECP384R1)]
        [TestCase(ECDSAKeyType.SECP521R1)]
        [TestCase(ECDSAKeyType.SECT283K1)]
        public void TestSignatureMalleability(ECDSAKeyType keyType)
        {
            var ecdsaNoMalleability = new ECDSA(keyType);
            var secret = new byte[] { 0, 1, 2, 3, 4 }; // deterministic secret
            var privateKey = ecdsaNoMalleability.GeneratePrivateKey(secret);
            var publicKey = ecdsaNoMalleability.DerivePublicKey(privateKey);
            var messageDigest = Hashers.Hash(CHF.SHA2_256,
                Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog"));

            var order = privateKey.Parameters.Value.Parameters.Curve.Order;

            var ecdsaAllowMalleability = SignerUtilities.GetSigner("NONEwithECDSA");

            var secureRandom = new SecureRandom();
            byte[] ecdsaAllowMalleabilitySig;
            BigInteger[] sig;
            // generate a "High S" signature
            do
            {
                var parametersWithRandom = new ParametersWithRandom(privateKey.Parameters.Value, secureRandom);
                ecdsaAllowMalleability.Init(true, parametersWithRandom);
                ecdsaAllowMalleability.BlockUpdate(messageDigest, 0, messageDigest.Length);

                ecdsaAllowMalleabilitySig = ecdsaAllowMalleability.GenerateSignature();
                sig = DerSig_To_R_And_S(ecdsaAllowMalleabilitySig);
            } while (IsLowS(order, sig[1]));

            var canonicalSig = CanonicalizeSig(order, ecdsaAllowMalleabilitySig);

            // normal ECDSA should be able to verify both the OriginalSig and CanonicalSig
            ecdsaAllowMalleability.Init(false, publicKey.Parameters.Value);
            ecdsaAllowMalleability.BlockUpdate(messageDigest, 0, messageDigest.Length);
            Assert.IsTrue(ecdsaAllowMalleability.VerifySignature(ecdsaAllowMalleabilitySig));

            ecdsaAllowMalleability.Init(false, publicKey.Parameters.Value);
            ecdsaAllowMalleability.BlockUpdate(messageDigest, 0, messageDigest.Length);
            Assert.IsTrue(ecdsaAllowMalleability.VerifySignature(canonicalSig));

            // our LowS ECDSA should be able to verify only the CanonicalSig
            Assert.IsFalse(ecdsaNoMalleability.VerifyDigest(ecdsaAllowMalleabilitySig, messageDigest, publicKey));
            Assert.IsTrue(ecdsaNoMalleability.VerifyDigest(canonicalSig, messageDigest, publicKey));
        }
    }
}