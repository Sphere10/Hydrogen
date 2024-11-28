// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using NUnit.Framework;

namespace Hydrogen.CryptoEx.Tests;

public abstract class RandomHashTests : HashTestBase {

	protected readonly TestItem<int, string>[] DATA_RANDOMHASH =  {
 		    // NOTE: Input denotes the number of bytes to take from DATA_BYTES when executing test
 			new TestItem<int, string> {Input= 17,  Expected= "0x79ff7b9c9f325cbece87b8243ec6ebc69042147595c6919bd2cb8258dfc0d9a0"},
			 new TestItem<int, string> {Input= 31,  Expected= "0xd4ee93f7973c884574aefab26cae00b6b32e920324a52e13bb8f3d97e0b91763"},
			 new TestItem<int, string> {Input= 32,  Expected= "0x8ca859fdacfd9401d072b5d408d2d3dc0b1926a1ed15400e76e37c3445c808fa"},
			 new TestItem<int, string> {Input= 33,  Expected= "0x627fa9bf640cbae10b24d1bb265368b50692b116d371f8b5ff400830dbcd6be7"},
			 new TestItem<int, string> {Input= 34,  Expected= "0x1aafc18923eccbacfbd56fd716c2db6dbe3684bdaeebe3002feb97cfa52038eb"},
			 new TestItem<int, string> {Input= 63,  Expected= "0xc371d4eae34bb2d0fdd4ed83cdffc20334fc6c789b76aa0c0db175bd67037eb5"},
			 new TestItem<int, string> {Input= 64,  Expected= "0xb8be930b1fa4700ded05102eec33088d0cf891925686cf576603d4ab71da0617"},
			 new TestItem<int, string> {Input= 65,  Expected= "0xbcea73e7fa1477abf02e88a5a4f130a71bccea8f33ef61f15d7fb7e2a5da8326"},
			 new TestItem<int, string> {Input= 100, Expected= "0xc3178a3994159a36f536ea9c460ff74022b28157267f3afa1ad787abf66bc326"},
			 new TestItem<int, string> {Input= 117, Expected= "0xf5cc34efc10929fabde56a521fa172fb36c99a2cfd7b421b1cd55a6cde26ce99"},
			 new TestItem<int, string> {Input= 127, Expected= "0xf87bba49f2cfff37ab8c01ca3de9f48aa4b290339147e301a886d6a7778cc41a"},
			 new TestItem<int, string> {Input= 128, Expected= "0x5064ac09cb767c07a3ad538eee0cd0beb59ca0363df91d00b27667c71aaf0ae9"},
			 new TestItem<int, string> {Input= 129, Expected= "0x6b6119e5b826ae85b7b8a9dbba928679e47f08b03578a2fc11205731acec655c"},
			 new TestItem<int, string> {Input= 178, Expected= "0x78731c92386f6e3e024ef49096782812f0cfbbc2c179de9dbeec2c22faebce89"},
			 new TestItem<int, string> {Input= 199, Expected= "0x121f5286f7ddb462100556f142048c962839554b8cb55a648afb742f9d8d398f"},
			 new TestItem<int, string> {Input= 200, Expected= "0x488bdfcf90f4b5c0803691562bb30604e9c3b39ed37e8eea9957ed8ae12dec26"}
		 };

}

public class RandomHashFastTest : RandomHashTests {
	[Test]
	public void TestRandomHash() {
		TestHash(x => RandomHashFast.Compute(x).ToHexString(), DATA_RANDOMHASH);
	}
}
