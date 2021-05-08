using NUnit.Framework;
using Org.BouncyCastle.Crypto;

namespace NPascalCoin.UnitTests.Crypto {
	/**
     * Test vectors for Snefru8256Digest
     *  
     */
	[TestFixture]
	public class Snefru8_256DigestTest
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

		private static readonly string[] Digests8256 =
		{
			"8617F366566A011837F4FB4BA5BEDEA2B892F3ED8B894023D16AE344B2BE5881",
			"45161589AC317BE0CEBA70DB2573DDDA6E668A31984B39BF65E4B664B584C63D",
			"7D033205647A2AF3DC8339F6CB25643C33EBC622D32979C4B612B02C4903031B",
			"C5D4CE38DAA043BDD59ED15DB577500C071B917C1A46CD7B4D30B44A44C86DF8",
			"9304BB2F876D9C4F54546CF7EC59E0A006BEAD745F08C642F25A7C808E0BF86E",
			"83AA9193B62FFD269FAA43D31E6AC2678B340E2A85849470328BE9773A9E5728",
			"D5FCE38A152A2D9B83AB44C29306EE45AB0AED0E38C957EC431DAB6ED6BB71B8",
			"839D5F685685C6055C7A9679369BD14C9604CCB4FBD27FBD850E64F9A4430F46"
		};

		protected override IDigest CloneDigest(IDigest digest) {
			return new SnefruDigest((Snefru8_256Digest)digest);
		}

		public Snefru8_256DigestTest()
			: base(new Snefru8_256Digest(), Messages, Digests8256) {
		}

		[Test]
		public void TestSnefru8_256() {
			Snefru8_256DigestTest digestTest = new Snefru8_256DigestTest();
			digestTest.PerformTest();
		}
	}
}