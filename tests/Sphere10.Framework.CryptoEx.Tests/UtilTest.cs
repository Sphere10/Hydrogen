// using NUnit.Framework;
//
// namespace Crypto.UnitTests {
// 	[TestFixture]
// 	public class UtilTest {
// 		[Test]
// 		[TestCase("", true)]
// 		[TestCase("AE", true)]
// 		[TestCase("09FA2E", true)]
// 		[TestCase("ZA", false)]
// 		[TestCase("09SA2E", false)]
// 		public void TestHexString(string stringToParse, bool pass) =>
// 			Assert.AreEqual(ECCrypto.IsHexString(stringToParse), pass);
//
// 		[Test]
// 		[TestCase("")]
// 		[TestCase("AE")]
// 		[TestCase("09FA2E")]
// 		public void TestEncodeAndDecodeHexString(string stringToParse) =>
// 			Assert.AreEqual(ECCrypto.RawToHex(ECCrypto.HexaToRaw(stringToParse)).ToUpperInvariant(), stringToParse.ToUpperInvariant());
//
// 		[Test]
// 		[TestCase(new byte[] { 56, 127, 128 }, false)]
// 		[TestCase(new byte[] { 56, 126, 122 }, true)]
// 		public void TestIsHumanReadable(byte[] bytesToParse, bool pass) =>
// 			Assert.AreEqual(ECCrypto.IsHumanReadable(bytesToParse), pass);
// 	}
// }