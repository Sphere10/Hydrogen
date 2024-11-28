using NUnit.Framework;
using Org.BouncyCastle.Crypto;

namespace Hydrogen.CryptoEx.Tests;

/**
 * Test vectors for Haval5_256Digest
 *  
 */
[TestFixture]
public class Haval5_256DigestTest
	: DigestTest {
	private static readonly string[] Messages =
	{
			"",
			"a",
			"abc",
			"message digest",
			"abcdefghijklmnopqrstuvwxyz",
			"ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789",
			"12345678901234567890123456789012345678901234567890123456789012345678901234567890",
			"abcdbcdecdefdefgefghfghighijhijk"
		};

	private static readonly string[] Digests5_256 =
	{
			"BE417BB4DD5CFB76C7126F4F8EEB1553A449039307B1A3CD451DBFDC0FBBE330",
			"DE8FD5EE72A5E4265AF0A756F4E1A1F65C9B2B2F47CF17ECF0D1B88679A3E22F",
			"976CD6254C337969E5913B158392A2921AF16FCA51F5601D486E0A9DE01156E7",
			"7CCF22AF7F99ACD6AC84F176041329E2958FDE1419A259D5A4B89D8F4115AD74",
			"C9C7D8AFA159FD9E965CB83FF5EE6F58AEDA352C0EFF005548153A61551C38EE",
			"B45CB6E62F2B1320E4F8F1B0B273D45ADD47C321FD23999DCF403AC37636D963",
			"68E57A72AD513AF517469A96A0073CE212B42E772671687DE3DFCE4FF8CDE9BF",
			"56971D0DA32B1EE40492D5EC330F3A31668B91B84C60BE1076ABAD959804E83B"
		};

	protected override IDigest CloneDigest(IDigest digest) {
		return new Haval5_256Digest((Haval5_256Digest)digest);
	}

	public Haval5_256DigestTest()
		: base(new Haval5_256Digest(), Messages, Digests5_256) {
	}

	[Test]
	public void TestHaval5_256() {
		Haval5_256DigestTest digestTest = new Haval5_256DigestTest();
		digestTest.PerformTest();
	}
}
