using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization;
using NUnit.Framework;
using Sphere10.Framework.CryptoEx.EC;

namespace Sphere10.Framework.CryptoEx.Tests;

public class Bip340SchnorrTestData
{
    [JsonPropertyName("index")]
    public int Index { get; set; }
    [JsonPropertyName("secret key")]
    public string SecretKey { get; set; }
    [JsonPropertyName("public key")]
    public string PublicKey { get; set; }
    [JsonPropertyName("aux_rand")]
    public string AuxRand { get; set; }
    [JsonPropertyName("message")]
    public string Message { get; set; }
    [JsonPropertyName("signature")]
    public string Signature { get; set; }
    [JsonPropertyName("verification result")]
    public bool VerificationResult { get; set; }
    [JsonPropertyName("comment")]
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
        Span<byte> result = stackalloc byte [length];
        RandomNumberGenerator.Fill(result);
        return result.ToArray();
    }

    [OneTimeSetUp]
    public void Init() {
        // extract the needed Resource (bip340SchnorrVectors.json) and assign path to variable below
        _bip40SchnorrFolder = Tools.FileSystem.GetTempEmptyDirectory(true);
        Tools.FileSystem.AppendAllBytes(Path.Combine(_bip40SchnorrFolder, "bip340SchnorrVectors.json"), Properties.Resource.bip340SchnorrVectors_json);
        _bip40SchnorrFilePath = Path.Combine(_bip40SchnorrFolder, "bip340SchnorrVectors.json");
        var jsonContent = File.ReadAllText(_bip40SchnorrFilePath);
        _vectors = JsonSerializer.Deserialize<List<Bip340SchnorrTestData>>(jsonContent);
    }

    [OneTimeTearDown]
    public void Cleanup() {
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
            catch (Exception e)
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
    [TestCase(ECDSAKeyType.SECP256K1), Repeat(50)]
    // [TestCase(ECDSAKeyType.SECP384R1)]
    // [TestCase(ECDSAKeyType.SECP521R1)]
    // [TestCase(ECDSAKeyType.SECT283K1)]
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
    
    // TODO test parsing private and public keys
}