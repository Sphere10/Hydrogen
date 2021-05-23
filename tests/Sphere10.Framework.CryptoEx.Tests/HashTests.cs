using NUnit.Framework;

namespace Sphere10.Framework.CryptoEx.Tests {


	[TestFixture]
	public class HashTests : HashTestBase {

		[Test]
		public void TestSHA2_224() {
			TestHash(x => Hashers.Hash(CHF.SHA2_224, x).ToHexString(), DATA_SHA2_224);
		}

		[Test]
		public void TestSHA2_256() {
			TestHash(x => Hashers.Hash(CHF.SHA2_256, x).ToHexString(), DATA_SHA2_256);
		}

		[Test]
		public void TestSHA2_384() {
			TestHash(x => Hashers.Hash(CHF.SHA2_384, x).ToHexString(), DATA_SHA2_384);
		}

		[Test]
		public void TestSHA2_512() {
			TestHash(x => Hashers.Hash(CHF.SHA2_512, x).ToHexString(), DATA_SHA2_512);
		}

		[Test]
		public void TestSHA2_512_224() {
			TestHash(x => Hashers.Hash(CHF.SHA2_512_224, x).ToHexString(), DATA_SHA2_512_224);
		}

		[Test]
		public void TestSHA2_512_256() {
			TestHash(x => Hashers.Hash(CHF.SHA2_512_256, x).ToHexString(), DATA_SHA2_512_256);
		}

		[Test]
		public void TestSHA0() {
			TestHash(x => Hashers.Hash(CHF.SHA0, x).ToHexString(), DATA_SHA0);
		}

		[Test]
		public void TestSHA1_160() {
			TestHash(x => Hashers.Hash(CHF.SHA1_160, x).ToHexString(), DATA_SHA1_160);
		}

		[Test]
		public void TestSHA3_224() {
			TestHash(x => Hashers.Hash(CHF.SHA3_224, x).ToHexString(), DATA_SHA3_224);
		}

		[Test]
		public void TestSHA3_256() {
			TestHash(x => Hashers.Hash(CHF.SHA3_256, x).ToHexString(), DATA_SHA3_256);
		}

		[Test]
		public void TestSHA3_384() {
			TestHash(x => Hashers.Hash(CHF.SHA3_384, x).ToHexString(), DATA_SHA3_384);
		}

		[Test]
		public void TestSHA3_512() {
			TestHash(x => Hashers.Hash(CHF.SHA3_512, x).ToHexString(), DATA_SHA3_512);
		}

		[Test]
		public void TestRIPEMD() {
			TestHash(x => Hashers.Hash(CHF.RIPEMD, x).ToHexString(), DATA_RIPEMD);
		}

		[Test]
		public void TestRIPEMD_160() {
			TestHash(x => Hashers.Hash(CHF.RIPEMD_160, x).ToHexString(), DATA_RIPEMD_160);
		}

		[Test]
		public void TestRIPEMD_256() {
			TestHash(x => Hashers.Hash(CHF.RIPEMD_256, x).ToHexString(), DATA_RIPEMD_256);
		}

		[Test]
		public void TestRIPEMD_320() {
			TestHash(x => Hashers.Hash(CHF.RIPEMD_320, x).ToHexString(), DATA_RIPEMD_320);
		}

		[Test]
		public void TestBlake2b_128() {
			TestHash(x => Hashers.Hash(CHF.Blake2b_128, x).ToHexString(), DATA_BLAKE2B_128);
		}

		[Test]
		public void TestBlake2b_160() {
			TestHash(x => Hashers.Hash(CHF.Blake2b_160, x).ToHexString(), DATA_BLAKE2B_160);
		}

		[Test]
		public void TestBlake2b_256() {
			TestHash(x => Hashers.Hash(CHF.Blake2b_256, x).ToHexString(), DATA_BLAKE2B_256);
		}

		[Test]
		public void TestBlake2b_384() {
			TestHash(x => Hashers.Hash(CHF.Blake2b_384, x).ToHexString(), DATA_BLAKE2B_384);
		}

		[Test]
		public void TestBlake2b_512() {
			TestHash(x => Hashers.Hash(CHF.Blake2b_512, x).ToHexString(), DATA_BLAKE2B_512);
		}

		[Test]
		public void TestBlake2s_128() {
			TestHash(x => Hashers.Hash(CHF.Blake2s_128, x).ToHexString(), DATA_BLAKE2S_128);
		}

		[Test]
		public void TestBlake2s_160() {
			TestHash(x => Hashers.Hash(CHF.Blake2s_160, x).ToHexString(), DATA_BLAKE2S_160);
		}

		[Test]
		public void TestBlake2s_224() {
			TestHash(x => Hashers.Hash(CHF.Blake2s_224, x).ToHexString(), DATA_BLAKE2S_224);
		}

		[Test]
		public void TestBlake2s_256() {
			TestHash(x => Hashers.Hash(CHF.Blake2s_256, x).ToHexString(), DATA_BLAKE2S_256);
		}

		[Test]
		public void TestTiger_3_128() {
			TestHash(x => Hashers.Hash(CHF.Tiger_3_128, x).ToHexString(), DATA_TIGER_3_128);
		}

		[Test]
		public void TestTiger_3_160() {
			TestHash(x => Hashers.Hash(CHF.Tiger_3_160, x).ToHexString(), DATA_TIGER_3_160);
		}

		[Test]
		public void TestTiger_3_192() {
			TestHash(x => Hashers.Hash(CHF.Tiger_3_192, x).ToHexString(), DATA_TIGER_3_192);
		}

		[Test]
		public void TestTiger_4_128() {
			TestHash(x => Hashers.Hash(CHF.Tiger_4_128, x).ToHexString(), DATA_TIGER_4_128);
		}

		[Test]
		public void TestTiger_4_160() {
			TestHash(x => Hashers.Hash(CHF.Tiger_4_160, x).ToHexString(), DATA_TIGER_4_160);
		}

		[Test]
		public void TestTiger_4_192() {
			TestHash(x => Hashers.Hash(CHF.Tiger_4_192, x).ToHexString(), DATA_TIGER_4_192);
		}

		[Test]
		public void TestTiger_5_128() {
			TestHash(x => Hashers.Hash(CHF.Tiger_5_128, x).ToHexString(), DATA_TIGER_5_128);
		}

		[Test]
		public void TestTiger_5_160() {
			TestHash(x => Hashers.Hash(CHF.Tiger_5_160, x).ToHexString(), DATA_TIGER_5_160);
		}

		[Test]
		public void TestTiger_5_192() {
			TestHash(x => Hashers.Hash(CHF.Tiger_5_192, x).ToHexString(), DATA_TIGER_5_192);
		}

		[Test]
		public void TestTiger2_3_128() {
			TestHash(x => Hashers.Hash(CHF.Tiger2_3_128, x).ToHexString(), DATA_TIGER2_3_128);
		}

		[Test]
		public void TestTiger2_3_160() {
			TestHash(x => Hashers.Hash(CHF.Tiger2_3_160, x).ToHexString(), DATA_TIGER2_3_160);
		}

		[Test]
		public void TestTiger2_3_192() {
			TestHash(x => Hashers.Hash(CHF.Tiger2_3_192, x).ToHexString(), DATA_TIGER2_3_192);
		}

		[Test]
		public void TestTiger2_4_128() {
			TestHash(x => Hashers.Hash(CHF.Tiger2_4_128, x).ToHexString(), DATA_TIGER2_4_128);
		}

		[Test]
		public void TestTiger2_4_160() {
			TestHash(x => Hashers.Hash(CHF.Tiger2_4_160, x).ToHexString(), DATA_TIGER2_4_160);
		}

		[Test]
		public void TestTiger2_4_192() {
			TestHash(x => Hashers.Hash(CHF.Tiger2_4_192, x).ToHexString(), DATA_TIGER2_4_192);
		}

		[Test]
		public void TestTiger2_5_128() {
			TestHash(x => Hashers.Hash(CHF.Tiger2_5_128, x).ToHexString(), DATA_TIGER2_5_128);
		}

		[Test]
		public void TestTiger2_5_160() {
			TestHash(x => Hashers.Hash(CHF.Tiger2_5_160, x).ToHexString(), DATA_TIGER2_5_160);
		}

		[Test]
		public void TestTiger2_5_192() {
			TestHash(x => Hashers.Hash(CHF.Tiger2_5_192, x).ToHexString(), DATA_TIGER2_5_192);
		}

		[Test]
		public void TestSnefru8_128() {
			TestHash(x => Hashers.Hash(CHF.Snefru_8_128, x).ToHexString(), DATA_SNEFRU_8_128);
		}

		[Test]
		public void TestSnefru8_256() {
			TestHash(x => Hashers.Hash(CHF.Snefru_8_256, x).ToHexString(), DATA_SNEFRU_8_256);
		}

		[Test]
		public void TestGrindahl256() {
			TestHash(x => Hashers.Hash(CHF.Grindahl256, x).ToHexString(), DATA_GRINDAHL256);
		}

		[Test]
		public void TestGrindahl512() {
			TestHash(x => Hashers.Hash(CHF.Grindahl512, x).ToHexString(), DATA_GRINDAHL512);
		}

		[Test]
		public void TestHas160() {
			TestHash(x => Hashers.Hash(CHF.Has160, x).ToHexString(), DATA_HAS160);
		}

		[Test]
		public void TestHaval_3_128() {
			TestHash(x => Hashers.Hash(CHF.Haval_3_128, x).ToHexString(), DATA_HAVAL_3_128);
		}

		[Test]
		public void TestHaval_3_160() {
			TestHash(x => Hashers.Hash(CHF.Haval_3_160, x).ToHexString(), DATA_HAVAL_3_160);
		}

		[Test]
		public void TestHaval_3_192() {
			TestHash(x => Hashers.Hash(CHF.Haval_3_192, x).ToHexString(), DATA_HAVAL_3_192);
		}

		[Test]
		public void TestHaval_3_224() {
			TestHash(x => Hashers.Hash(CHF.Haval_3_224, x).ToHexString(), DATA_HAVAL_3_224);
		}

		[Test]
		public void TestHaval_3_256() {
			TestHash(x => Hashers.Hash(CHF.Haval_3_256, x).ToHexString(), DATA_HAVAL_3_256);
		}

		[Test]
		public void TestHaval_4_128() {
			TestHash(x => Hashers.Hash(CHF.Haval_4_128, x).ToHexString(), DATA_HAVAL_4_128);
		}

		[Test]
		public void TestHaval_4_160() {
			TestHash(x => Hashers.Hash(CHF.Haval_4_160, x).ToHexString(), DATA_HAVAL_4_160);
		}

		[Test]
		public void TestHaval_4_192() {
			TestHash(x => Hashers.Hash(CHF.Haval_4_192, x).ToHexString(), DATA_HAVAL_4_192);
		}

		[Test]
		public void TestHaval_4_224() {
			TestHash(x => Hashers.Hash(CHF.Haval_4_224, x).ToHexString(), DATA_HAVAL_4_224);
		}

		[Test]
		public void TestHaval_4_256() {
			TestHash(x => Hashers.Hash(CHF.Haval_4_256, x).ToHexString(), DATA_HAVAL_4_256);
		}

		[Test]
		public void TestHaval_5_128() {
			TestHash(x => Hashers.Hash(CHF.Haval_5_128, x).ToHexString(), DATA_HAVAL_5_128);
		}

		[Test]
		public void TestHaval_5_160() {
			TestHash(x => Hashers.Hash(CHF.Haval_5_160, x).ToHexString(), DATA_HAVAL_5_160);
		}

		[Test]
		public void TestHaval_5_192() {
			TestHash(x => Hashers.Hash(CHF.Haval_5_192, x).ToHexString(), DATA_HAVAL_5_192);
		}

		[Test]
		public void TestHaval_5_224() {
			TestHash(x => Hashers.Hash(CHF.Haval_5_224, x).ToHexString(), DATA_HAVAL_5_224);
		}

		[Test]
		public void TestHaval_5_256() {
			TestHash(x => Hashers.Hash(CHF.Haval_5_256, x).ToHexString(), DATA_HAVAL_5_256);
		}

		[Test]
		public void TestKeccak_224() {
			TestHash(x => Hashers.Hash(CHF.Keccak_224, x).ToHexString(), DATA_KECCAK_224);
		}

		[Test]
		public void TestKeccak_256() {
			TestHash(x => Hashers.Hash(CHF.Keccak_256, x).ToHexString(), DATA_KECCAK_256);
		}

		[Test]
		public void TestKeccak_288() {
			TestHash(x => Hashers.Hash(CHF.Keccak_288, x).ToHexString(), DATA_KECCAK_288);
		}

		[Test]
		public void TestKeccak_384() {
			TestHash(x => Hashers.Hash(CHF.Keccak_384, x).ToHexString(), DATA_KECCAK_384);
		}

		[Test]
		public void TestKeccak_512() {
			TestHash(x => Hashers.Hash(CHF.Keccak_512, x).ToHexString(), DATA_KECCAK_512);
		}

		[Test]
		public void TestMD2() {
			TestHash(x => Hashers.Hash(CHF.MD2, x).ToHexString(), DATA_MD2);
		}

		[Test]
		public void TestMD4() {
			TestHash(x => Hashers.Hash(CHF.MD4, x).ToHexString(), DATA_MD4);
		}

		[Test]
		public void TestMD5() {
			TestHash(x => Hashers.Hash(CHF.MD5, x).ToHexString(), DATA_MD5);
		}

		[Test]
		public void TestPanama() {
			TestHash(x => Hashers.Hash(CHF.Panama, x).ToHexString(), DATA_PANAMA);
		}

		[Test]
		public void TestGost() {
			TestHash(x => Hashers.Hash(CHF.Gost, x).ToHexString(), DATA_GOST);
		}

		[Test]
		public void TestGost_3411_2012_256() {
			TestHash(x => Hashers.Hash(CHF.Gost3411_2012_256, x).ToHexString(), DATA_GOST_3411_2012_256);
		}

		[Test]
		public void TestGost_3411_2012_512() {
			TestHash(x => Hashers.Hash(CHF.Gost3411_2012_512, x).ToHexString(), DATA_GOST_3411_2012_512);
		}

		[Test]
		public void TestWhirlPool() {
			TestHash(x => Hashers.Hash(CHF.WhirlPool, x).ToHexString(), DATA_WHIRLPOOL);
		}

		[Test]
		public void TestRadioGatun32() {
			TestHash(x => Hashers.Hash(CHF.RadioGatun32, x).ToHexString(), DATA_RADIOGATUN32);
		}

	}

}
