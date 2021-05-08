// using NUnit.Framework;
// using Org.BouncyCastle.Crypto;
//
// namespace NPascalCoin.UnitTests.Crypto {
// 	/**
//      * Test vectors for Grindahl512Digest
//      *  
//      */
// 	[TestFixture]
// 	public class Grindahl512DigestTest
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
// 		private static readonly string[] Digests =
// 		{
// 			"EE0BA85F90B6D232430BA43DD0EDD008462591816962A355602ED214FAAE54A9A4607D6F577CE950421FF58AEA53F51A7A9F5CCA894C3776104D43568FEA1207",
// 			"A9381C925D46E25E6C8F3BF53EC00AD0F762626500A5E50D606447767D4172006542F1FE7F26A31A8D77AC5DEB620450B12DBE57C248292AE08A89049DB593FC",
// 			"521EEF5DB10E1F68E56F10FECDD00CAC7B1608C24A1BFBC876250691B793C133076322A9763D1035CF699BC33359CE448B46709FC06C9C15B696B4BF92CD07B0",
// 			"5CDC0EA9A52565C093A5FF96AB372BCD2A78E7BE79010170544019740DECDB522B297B92344AD2DD37A2A5AA403D239788718246C5795E24414F682DC8061E76",
// 			"AE5E89EC63191F622873FBF5C8AA8631757C4FC3F48B3E41121F3738FA31E36EEA3CD34344C0183E4C0A5DE27115A4D1B27BE6C7FFCA65A4205C087FD01653AB",
// 			"5C7560A629D3C68B6DFD162DC9FBAD1873707D15FDA5E6A061D7855C359E898510AA59E797F1695B5D40264702842E37AF37C9D922459038BFD32898C732F48F",
// 			"756B2F5D9B709F14D796E4B5E31932C3EE8714039C446EEF7CE6F9D4E04484CAF85C7727882CF2B1549BC90C0EA8D1C8BB7584BD51A844D1B1C9D8AFFB4896E1",
// 			"2BFDDA2278D02B93874BF60B3EC84B6227B1307E118BB332A287C2EB317ACEE97715D8CA1E8052EBC211169A79776F4F5B5DE54CE578E0332745D6A95D44F4A0"
// 		};
//
// 		protected override IDigest CloneDigest(IDigest digest) {
// 			return new Grindahl512Digest((Grindahl512Digest)digest);
// 		}
//
// 		public Grindahl512DigestTest()
// 			: base(new Grindahl512Digest(), Messages, Digests) {
// 		}
//
//
// 		[Test]
// 		public void TestGrindahl512() {
// 			Grindahl512DigestTest digestTest = new Grindahl512DigestTest();
// 			digestTest.PerformTest();
// 		}
// 	}
// }