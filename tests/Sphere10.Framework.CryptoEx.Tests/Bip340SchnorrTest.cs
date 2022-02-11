using Newtonsoft.Json;
using NUnit.Framework;
using Org.BouncyCastle.Math;
using Sphere10.Framework.CryptoEx.EC;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sphere10.Framework.CryptoEx.Tests;

public class Bip340SchnorrTestData
{
    [JsonProperty("index")]
    public int Index { get; set; }
    [JsonProperty("secret key")]
    public string SecretKey { get; set; }
    [JsonProperty("public key")]
    public string PublicKey { get; set; }
    [JsonProperty("aux_rand")]
    public string AuxRand { get; set; }
    [JsonProperty("message")]
    public string Message { get; set; }
    [JsonProperty("signature")]
    public string Signature { get; set; }
    [JsonProperty("verification result")]
    public bool VerificationResult { get; set; }
    [JsonProperty("comment")]
    public string Comment { get; set; }
}

[TestFixture]
public class Bip340SchnorrTest
{
    private string _bip40SchnorrFolder;
    private string _bip40SchnorrFilePath;
    private List<Bip340SchnorrTestData> _vectors;

    private static byte[] RandomBytes(int length)
    {
        return Tools.Crypto.GenerateCryptographicallyRandomBytes(length);
    }

    [OneTimeSetUp]
    public void Init()
    {
        // extract the needed Resource (bip340SchnorrVectors.json) and assign path to variable below
        _bip40SchnorrFolder = Tools.FileSystem.GetTempEmptyDirectory(true);
        Tools.FileSystem.AppendAllBytes(Path.Combine(_bip40SchnorrFolder, "bip340SchnorrVectors.json"), Properties.Resource.bip340SchnorrVectors_json);
        _bip40SchnorrFilePath = Path.Combine(_bip40SchnorrFolder, "bip340SchnorrVectors.json");
        var jsonContent = File.ReadAllText(_bip40SchnorrFilePath);
        _vectors = JsonConvert.DeserializeObject<List<Bip340SchnorrTestData>>(jsonContent);
    }

    [OneTimeTearDown]
    public void Cleanup()
    {
        // Delete the extracted resources here
        Tools.FileSystem.DeleteDirectory(_bip40SchnorrFolder);
    }

    [Test]
    public void TestThatSigningProducesSameSignatureAsTestVectors()
    {
        _vectors.Where(vec => !string.IsNullOrEmpty(vec.SecretKey)).ForEach(vec =>
        {
            var schnorr = new Schnorr(ECDSAKeyType.SECP256K1);

            Assert.IsTrue(schnorr.TryParsePrivateKey(vec.SecretKey.ToHexByteArray(),
                    out var d),
                $"error creating private key index = '{vec.Index}' private key = '{vec.SecretKey}'");
            var m = vec.Message.ToHexByteArray();
            var a = vec.AuxRand.ToHexByteArray();
            var expected = vec.Signature.ToHexByteArray();
            var actual = schnorr.SignDigestWithAuxRandomData(d, m, a);
            Assert.AreEqual(expected, actual, $"signature mismatch at index = '{vec.Index}'. expected = '{vec.Signature}' but got = '{actual.ToHexString()}'");
        });
    }

    [Test]
    public void TestCanVerifyTestVectorsSignatures()
    {
        _vectors.ForEach(vec =>
        {
            var schnorr = new Schnorr(ECDSAKeyType.SECP256K1);

            var m = vec.Message.ToHexByteArray();
            var pk = vec.PublicKey.ToHexByteArray();
            var sig = vec.Signature.ToHexByteArray();
            var expected = vec.VerificationResult;
            bool actual;
            try
            {
                actual = schnorr.VerifyDigest(sig, m, pk);
            }
            catch (Exception)
            {
                actual = false;
            }
            Assert.AreEqual(expected, actual, $"verification failure at index = '{vec.Index}'. expected = '{expected}' but got = '{actual}'");
        });
    }

    [Test]
    public void TestThatBatchVerifyVerifiesAllValidTestVectorsSignatures()
    {
        var positiveVectors = _vectors.Where(vec => vec.VerificationResult).ToArray();
        var messages = positiveVectors.Select(vec => vec.Message.ToHexByteArray()).ToArray();
        var pubKeys = positiveVectors.Select(vec => vec.PublicKey.ToHexByteArray()).ToArray();
        var signatures = positiveVectors.Select(vec => vec.Signature.ToHexByteArray()).ToArray();
        Exception exception = null;
        var schnorr = new Schnorr(ECDSAKeyType.SECP256K1);
        bool actual;
        try
        {
            actual = schnorr.BatchVerifyDigest(signatures, messages, pubKeys);
        }
        catch (Exception e)
        {
            actual = false;
            exception = e;
        }

        Assert.AreEqual(true, actual, $"batch verification failure. expected = '{true}' but got = '{actual}'");
        Assert.IsNull(exception);
    }

    [Test]
    public void TestThatBatchVerifyFailsOnOneInvalidSignature()
    {
        var positiveVectors = _vectors.Where(vec => vec.VerificationResult).ToList();
        var messages = positiveVectors.Select(vec => vec.Message.ToHexByteArray()).ToList();
        var pubKeys = positiveVectors.Select(vec => vec.PublicKey.ToHexByteArray()).ToList();
        var signatures = positiveVectors.Select(vec => vec.Signature.ToHexByteArray()).ToList();
        var negativeVector = _vectors.First(vec => !vec.VerificationResult);
        pubKeys.Add(negativeVector.PublicKey.ToHexByteArray());
        messages.Add(negativeVector.Message.ToHexByteArray());
        signatures.Add(negativeVector.Signature.ToHexByteArray());

        Exception exception = null;
        var schnorr = new Schnorr(ECDSAKeyType.SECP256K1);
        bool actual;
        try
        {
            actual = schnorr.BatchVerifyDigest(signatures.ToArray(), messages.ToArray(), pubKeys.ToArray());
        }
        catch (Exception e)
        {
            actual = false;
            exception = e;
        }

        Assert.AreEqual(false, actual, $"batch verification failure. expected = '{false}' but got = '{actual}'");
        Assert.AreEqual("c is not equal to y^2", exception?.Message);
    }

    [Test]
    [TestCase(ECDSAKeyType.SECP256K1), Repeat(64)]
    public void TestRandomSignAndVerify(ECDSAKeyType keyType)
    {
        var messageDigest = Hashers.Hash(CHF.SHA2_256, RandomBytes(new Random().Next(0, 65536)));
        var schnorr = new Schnorr(keyType);
        var sk = schnorr.GeneratePrivateKey();
        var pk = schnorr.DerivePublicKey(sk);
        var sig = schnorr.SignDigest(sk, messageDigest);
        var actual = schnorr.VerifyDigest(sig, messageDigest, pk);
        Assert.IsTrue(actual);
    }

    [Test]
    [TestCase(ECDSAKeyType.SECP256K1)]
    public void IsPublicKey(ECDSAKeyType keyType)
    {
        var schnorr = new Schnorr(keyType);
        var privateKey = schnorr.GeneratePrivateKey();
        var publicKey = schnorr.DerivePublicKey(privateKey);
        Assert.IsTrue(schnorr.IsPublicKey(privateKey, publicKey.RawBytes));
    }

    // test parsing private and public keys

    [Test]
    [TestCase(new byte[] { }, ECDSAKeyType.SECP256K1)]
    [TestCase(new byte[] { 0, 0 }, ECDSAKeyType.SECP256K1)]
    public void VerifyThatTryParsePrivateKeyFailsEarlyForBadKeys(byte[] badRawKey, ECDSAKeyType keyType)
    {
        var schnorr = new Schnorr(keyType);
        Assert.IsFalse(schnorr.TryParsePrivateKey(badRawKey, out _));
    }

    [Test]
    [TestCase(ECDSAKeyType.SECP256K1)]
    public void VerifyThatTryParsePrivateKeyFailsForValuesNotInBetweenZeroToCurveOrderMinusOne(ECDSAKeyType keyType)
    {
        var schnorr = new Schnorr(keyType);
        var negativeOne = BigInteger.One.Negate();
        Assert.IsFalse(schnorr.TryParsePrivateKey(negativeOne.ToByteArray(), out _));
        var order = keyType.GetAttribute<KeyTypeOrderAttribute>().Value;
        Assert.IsFalse(schnorr.TryParsePrivateKey(order.ToByteArrayUnsigned(), out _));
    }

    [Test]
    [TestCase(ECDSAKeyType.SECP256K1)]
    [TestCase(ECDSAKeyType.SECP256K1)]
    public void VerifyThatTryParsePrivateKeyPassForGoodKeys(ECDSAKeyType keyType)
    {
        var schnorr = new Schnorr(keyType);
        var privateKeyBytes = schnorr.GeneratePrivateKey().RawBytes;
        Assert.IsTrue(schnorr.TryParsePrivateKey(privateKeyBytes, out var privateKey));
        Assert.AreEqual(privateKeyBytes, privateKey.RawBytes);
    }

    [Test]
    [TestCase(new byte[] { }, ECDSAKeyType.SECP256K1)]
    [TestCase(new byte[] { 0, 0 }, ECDSAKeyType.SECP256K1)]
    public void VerifyThatTryParsePublicKeyFailsEarlyForBadKeys(byte[] badRawKey, ECDSAKeyType keyType)
    {
        var schnorr = new Schnorr(keyType);
        Assert.IsFalse(schnorr.TryParsePublicKey(badRawKey, out _));
    }

    [Test]
    [TestCase(ECDSAKeyType.SECP256K1)]
    public void VerifyThatTryParsePublicKeyFailsForValuesNotInBetweenZeroToPrimeFieldMinusOne(ECDSAKeyType keyType)
    {
        var schnorr = new Schnorr(keyType);
        var negativeOne = BigInteger.One.Negate();
        Assert.IsFalse(schnorr.TryParsePublicKey(negativeOne.ToByteArray(), out _));
        var primeField = keyType.GetAttribute<KeyTypePrimeFieldAttribute>().Value;
        Assert.IsFalse(schnorr.TryParsePublicKey(primeField.ToByteArrayUnsigned(), out _));
    }

    [Test]
    [TestCase(ECDSAKeyType.SECP256K1)]
    [TestCase(ECDSAKeyType.SECP256K1)]
    public void VerifyThatTryParsePublicKeyPassForGoodKeys(ECDSAKeyType keyType)
    {
        var schnorr = new Schnorr(keyType);
        var privateKey = schnorr.GeneratePrivateKey();
        var publicKeyBytes = schnorr.DerivePublicKey(privateKey).RawBytes;
        Assert.IsTrue(schnorr.TryParsePublicKey(publicKeyBytes, out var publicKey));
        Assert.AreEqual(publicKeyBytes, publicKey.RawBytes);
    }
}