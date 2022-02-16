using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Sphere10.Framework.CryptoEx.EC;
using Sphere10.Framework.CryptoEx.EC.MuSigBuilder;

namespace Sphere10.Framework.CryptoEx.Tests;

[TestFixture]
public class MuSigBuilderTest
{
    private static Schnorr.PublicKey GetPublicKey(MuSig muSig, Schnorr.PrivateKey privateKey)
    {
        return muSig.Schnorr.DerivePublicKey(privateKey);
    }

    private static Schnorr.PrivateKey GetPrivateKey(MuSig muSig, byte[] privateKeyBytes)
    {
        _ = muSig.Schnorr.TryParsePrivateKey(privateKeyBytes, out var privateKey);
        return privateKey;
    }

    [Test]
    // this test performs a muSig between 3 parties using known test vectors.
    public void TestFullMuSigBuilderWithKnownTestVectors()
    {
        var muSig = new MuSig(new Schnorr(ECDSAKeyType.SECP256K1));
        var messageDigest = "746869735F636F756C645F62655F7468655F686173685F6F665F615F6D736721".ToHexByteArray();
        var aggregatedSig =
            "ed7d22176b48817351b197be4ff6df813c938dfc3cd5c9823640c2303e22e80fa00235ceebac132159e4e15ef107dbb027ca7a4cd557be2266ba26b8bfeced58"
                .ToHexByteArray();

        var alicePrivateKey = GetPrivateKey(muSig,
            "3FC866534575FA473CA1FFAAFA6A64001B5B319A6928A138A82146C367BF699C".ToHexByteArray());
        var bobPrivateKey = GetPrivateKey(muSig,
            "14DAD4678588D866F50B084204A69C7ECAF0E3F34B12638DD25545ED153A8128".ToHexByteArray());
        var charliePrivateKey = GetPrivateKey(muSig,
            "16E31D4126EE7C217EF3A45D5C99DCCE2A5F0477E694AF948A771D6735903B0F".ToHexByteArray());

        var aliceSessionId = "7CB6E93BCF96AEE2BB31AB80AC880E108438FCECCD2E6132B49A2CF103991ED0".ToHexByteArray();
        var bobSessionId = "236851BDBB4E62E06D08DC228D4E83A0A0971816EED785F994D19B165952F38D".ToHexByteArray();
        var charlieSessionId = "BC1DCCF8BB20655E39ABB6279152D46AB999F4C36982DB3296986DC0368319EB".ToHexByteArray();

        var alicePublicKey = GetPublicKey(muSig, alicePrivateKey).RawBytes;
        var bobPublicKey = GetPublicKey(muSig, bobPrivateKey).RawBytes;
        var charliePublicKey = GetPublicKey(muSig, charliePrivateKey).RawBytes;


        var aliceMuSigBuilder = new MuSigBuilder(alicePrivateKey, messageDigest, aliceSessionId);
        var bobMuSigBuilder = new MuSigBuilder(bobPrivateKey, messageDigest, bobSessionId);
        var charlieMuSigBuilder = new MuSigBuilder(charliePrivateKey, messageDigest, charlieSessionId);

        // it is important to maintain same order when passing the public keys between builders as
        // the output of the KeyAgg algorithm depends on the order of the input public keys.
        aliceMuSigBuilder.AddPublicKey(alicePublicKey);
        aliceMuSigBuilder.AddPublicKey(bobPublicKey);
        aliceMuSigBuilder.AddPublicKey(charliePublicKey);

        bobMuSigBuilder.AddPublicKey(alicePublicKey);
        bobMuSigBuilder.AddPublicKey(bobPublicKey);
        bobMuSigBuilder.AddPublicKey(charliePublicKey);

        charlieMuSigBuilder.AddPublicKey(alicePublicKey);
        charlieMuSigBuilder.AddPublicKey(bobPublicKey);
        charlieMuSigBuilder.AddPublicKey(charliePublicKey);


        // add public nonce
        // order between builders doesn't matter in the case of nonce
        aliceMuSigBuilder.AddPublicNonce(aliceMuSigBuilder.PublicNonce);
        aliceMuSigBuilder.AddPublicNonce(bobMuSigBuilder.PublicNonce);
        aliceMuSigBuilder.AddPublicNonce(charlieMuSigBuilder.PublicNonce);

        bobMuSigBuilder.AddPublicNonce(bobMuSigBuilder.PublicNonce);
        bobMuSigBuilder.AddPublicNonce(charlieMuSigBuilder.PublicNonce);
        bobMuSigBuilder.AddPublicNonce(aliceMuSigBuilder.PublicNonce);

        charlieMuSigBuilder.AddPublicNonce(charlieMuSigBuilder.PublicNonce);
        charlieMuSigBuilder.AddPublicNonce(aliceMuSigBuilder.PublicNonce);
        charlieMuSigBuilder.AddPublicNonce(bobMuSigBuilder.PublicNonce);


        // add partial signature
        // order between builders doesn't matter in the case of partial signature
        aliceMuSigBuilder.AddPartialSignature(bobMuSigBuilder.PartialSignature);
        aliceMuSigBuilder.AddPartialSignature(charlieMuSigBuilder.PartialSignature);
        aliceMuSigBuilder.AddPartialSignature(aliceMuSigBuilder.PartialSignature);

        bobMuSigBuilder.AddPartialSignature(charlieMuSigBuilder.PartialSignature);
        bobMuSigBuilder.AddPartialSignature(aliceMuSigBuilder.PartialSignature);
        bobMuSigBuilder.AddPartialSignature(bobMuSigBuilder.PartialSignature);

        charlieMuSigBuilder.AddPartialSignature(aliceMuSigBuilder.PartialSignature);
        charlieMuSigBuilder.AddPartialSignature(bobMuSigBuilder.PartialSignature);
        charlieMuSigBuilder.AddPartialSignature(charlieMuSigBuilder.PartialSignature);

        var aliceAggregatedSignature = aliceMuSigBuilder.BuildAggregatedSignature();
        var bobAggregatedSignature = bobMuSigBuilder.BuildAggregatedSignature();
        var charlieAggregatedSignature = charlieMuSigBuilder.BuildAggregatedSignature();

        Assert.AreEqual(aliceAggregatedSignature.AggregatedSignature, bobAggregatedSignature.AggregatedSignature);
        Assert.AreEqual(bobAggregatedSignature.AggregatedSignature, charlieAggregatedSignature.AggregatedSignature);
        Assert.AreEqual(aliceAggregatedSignature.AggregatedSignature, aggregatedSig);
        // since all aggregated signatures are same from above check, we can just verify one.
        Assert.IsTrue(muSig.Schnorr.VerifyDigest(charlieAggregatedSignature.AggregatedSignature,
            messageDigest,
            charlieAggregatedSignature.AggregatedPublicKey));
    }

    [Test]
    // this test performs a muSig between random number of parties using random values.
    [TestCase(ECDSAKeyType.SECP256K1), Repeat(64)]
    public void TestFullMuSigBuilderWithRandomInputs(ECDSAKeyType keyType)
    {
        var muSig = new MuSig(new Schnorr(keyType));
        // messageDigest must be 32 bytes in length
        var messageDigest = Tools.Crypto.GenerateCryptographicallyRandomBytes(32);

        var numberOfSigners = new Random().Next(2, 16);
        var publicKeys = new List<byte[]>();
        var muSigBuilders = new List<MuSigBuilder>();

        for (var i = 0; i < numberOfSigners; i++)
        {
            var privateKey = GetPrivateKey(muSig, Tools.Crypto.GenerateCryptographicallyRandomBytes(muSig.KeySize));
            publicKeys.Add(GetPublicKey(muSig, privateKey).RawBytes);
            muSigBuilders.Add(new MuSigBuilder(privateKey, messageDigest));
        }

        for (var i = 0; i < numberOfSigners; i++)
        {
            for (var j = 0; j < numberOfSigners; j++)
            {
                muSigBuilders[i].AddPublicKey(publicKeys[j]);
            }
        }

        for (var i = 0; i < numberOfSigners; i++)
        {
            for (var j = 0; j < numberOfSigners; j++)
            {
                muSigBuilders[i].AddPublicNonce(muSigBuilders[j].PublicNonce);
            }
        }

        for (var i = 0; i < numberOfSigners; i++)
        {
            for (var j = 0; j < numberOfSigners; j++)
            {
                muSigBuilders[i].AddPartialSignature(muSigBuilders[j].PartialSignature);
            }
        }

        var aggregatedSignatures = new List<MuSigData>();
        for (var i = 0; i < numberOfSigners; i++)
        {
            aggregatedSignatures.Add(muSigBuilders[i].BuildAggregatedSignature());
        }

        Assert.IsTrue(aggregatedSignatures.All(bytes => bytes.AggregatedSignature.SequenceEqual(aggregatedSignatures
            .First()
            .AggregatedSignature)));
        Assert.IsTrue(muSig.Schnorr.VerifyDigest(aggregatedSignatures.Last().AggregatedSignature,
            messageDigest,
            aggregatedSignatures.Last().AggregatedPublicKey));
    }
}