using NUnit.Framework;

namespace Hydrogen.Tests;

internal class StandardHydrogenCHFValues : ValuesAttribute {
	public StandardHydrogenCHFValues()
		: base(CHF.ConcatBytes, CHF.SHA2_256, CHF.SHA2_384, CHF.SHA2_512, CHF.SHA1_160, CHF.Blake2b_512, CHF.Blake2b_384, CHF.Blake2b_256, CHF.Blake2b_160, CHF.Blake2b_128) {
	}
}
