using System;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;
using Sphere10.Framework.CryptoEx.EC;
using System.Text;
using Tools;
using System.Reflection;
using System.IO;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Math;

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

        private static BigInteger[] DerSig_To_R_And_S(byte[] derSig)
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

        // TrickSig uses a given valid signature (r, s) over a message hash
        // to calculate another valid signature over the same message hash as (r, -s mod n)
        // where n is the curve order i.e. the order of the base point
        private static byte[] TrickSig(BigInteger order, byte[] sig)
        {
            var rs = DerSig_To_R_And_S(sig);
            rs[1] = rs[1].Negate().Mod(order);
            //rs[1] = order.Subtract(rs[1]);
            return R_And_S_To_DerSig(rs);
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
            var ecdsa = new ECDSA(keyType);
            var secret = new byte[] { 0, 1, 2, 3, 4 }; // deterministic secret
            var privateKey = ecdsa.GeneratePrivateKey(secret);
            var publicKey = ecdsa.DerivePublicKey(privateKey);
            var message = Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog");
            var sig = ecdsa.Sign(privateKey, message);
            Assert.IsTrue(ecdsa.Verify(sig, message, publicKey));
            var order = privateKey.Parameters.Value.Parameters.Curve.Order;
            var trickedSig = TrickSig(order, sig);
            Assert.IsFalse(ecdsa.Verify(trickedSig, message, publicKey));
        }
    }
}