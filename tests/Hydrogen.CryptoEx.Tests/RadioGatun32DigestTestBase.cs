using NUnit.Framework;
using Org.BouncyCastle.Crypto;

namespace Hydrogen.CryptoEx.Tests;

/**
 * Test vectors for RadioGatun32Digest
 *  
 */
[TestFixture]
public class RadioGatun32DigestTest
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

	private static readonly string[] Digests =
	{
			"F30028B54AFAB6B3E55355D277711109A19BEDA7091067E9A492FB5ED9F20117",
			"3B435C6FE708F65FFC3AEEF197BA5E8DD772040DA00D31D90DFF049ED87D794F",
			"6E3F3D756AF1579A03078E68E951EA4B3BAA64AFB2F09EE3E2E88393E31EBAEA",
			"A4C63643C55469E9F09438416DA3010F8B80B5091EE672CD5C4F6AFC5F9BC71F",
			"EA7514EC04D445C64BDAF302166868B3A8EFD2B227D7664D199739C734381B8A",
			"CDA5B66F705110A138D13605FE779CFA3ABE1BFD15651FDBDF42A86F0E4A8775",
			"4C4A821430EC6291DCFD30A4AAB6079F774CF5860335D863F067C0DD78A806EC",
			"D4A2D3A018C96720DB2BBC1CDC55CEB6492E2179F738F0CDF0D7705960B88B1C"
		};


	protected override IDigest CloneDigest(IDigest digest) {
		return new RadioGatun32Digest((RadioGatun32Digest)digest);
	}

	public RadioGatun32DigestTest()
		: base(new RadioGatun32Digest(), Messages, Digests) {
	}

	[Test]
	public void TestRadioGatun32() {
		RadioGatun32DigestTest digestTest = new RadioGatun32DigestTest();
		digestTest.PerformTest();
	}
}