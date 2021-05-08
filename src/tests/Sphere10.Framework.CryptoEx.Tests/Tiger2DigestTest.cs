// using NUnit.Framework;
// using Org.BouncyCastle.Crypto;
//
// namespace NPascalCoin.UnitTests.Crypto {
// 	/**
//      * Test vectors for Tiger2_5_192Digest
//      *  
//      */
// 	[TestFixture]
// 	public class Tiger2_5_192DigestTest
// 		: DigestTest {
// 		private static readonly string[] Messages =
// 		{
// 			"",
// 			"a",
// 			"abc",
// 			"message digest",
// 			"abcdefghijklmnopqrstuvwxyz",
// 			"ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789",
// 			"12345678901234567890123456789012345678901234567890123456789012345678901234567890",
// 			"abcdbcdecdefdefgefghfghighijhijk"
// 		};
//
// 		private static readonly string[] Digests5192 =
// 		{
// 			"61C657CC0C3C147ED90779B36A1E811F1D27F406E3F37010",
// 			"82B16983EC681F4742D1437A78E6F2E12D2DF70260E7EA98",
// 			"053C86A84303A4644D25FCD578642426477171DB33965647",
// 			"0B3C00DC652CD5E75FCCDF8154FC387F7EE00D06F9CA1429",
// 			"FF7B61F6093ABF5021C55713CD066F302D00935920F7C63B",
// 			"142AB036AED5217511985D33517D30002D35AE7DCADB72F9",
// 			"D824F7D871597092F42C256EFE827662B91D706627853EBE",
// 			"249F838DD22222FF2D50EC56589AE28AFD6B93EF47DBCECA"
// 		};
//
// 		protected override IDigest CloneDigest(IDigest digest) {
// 			return new Tiger2Digest((Tiger2_5_192Digest)digest);
// 		}
//
// 		public Tiger2_5_192DigestTest()
// 			: base(new Tiger2_5_192Digest(), Messages, Digests5192) {
// 		}
//
// 		[Test]
// 		public void TestSnefru8_256() {
// 			Tiger2_5_192DigestTest digestTest = new Tiger2_5_192DigestTest();
// 			digestTest.PerformTest();
// 		}
// 	}
// }