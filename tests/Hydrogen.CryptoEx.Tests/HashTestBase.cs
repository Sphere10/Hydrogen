// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Hydrogen.CryptoEx.Tests;

public abstract class HashTestBase {

	protected struct TestItem<TInput, TExpected> {
		public TInput Input;
		public TExpected Expected;
	}


	protected static void TestHash<TResult>(Func<byte[], TResult> hasher, IEnumerable<TestItem<int, TResult>> testCases) {
		foreach (var testCase in testCases) {
			var input = HexEncoding.Decode(DATA_BYTES).Take(testCase.Input).ToArray();
			var result = hasher(input);
			ClassicAssert.AreEqual(testCase.Expected, result);
		}
	}

	#region TestData

	// General purpose byte array for testing		
	const string DATA_BYTES =
		"0x4f550200ca022000bb718b4b00d6f74478c332f5fb310507e55a9ef9b38551f63858e3f7c86dbd00200006f69afae8a6b0735b6acfcc58b7865fc8418897c530211f19140c9f95f24532102700000000000003000300a297fd17506f6c796d696e65722e506f6c796d696e65722e506f6c796d6939303030303030302184d63666eb166619e925cef2a306549bbc4d6f4da3bdf28b4393d5c1856f0ee3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855000000006d68295b00000000";

	protected static readonly TestItem<int, string>[] DATA_SHA2_224 = {
		// NOTE: Input denotes the number of bytes to take from DATA_BYTES when executing test
		new() { Input = 17, Expected = "0x869cfeac56d43d426e0bd2785b8c20126ae7c983250a812131904b77" },
		new() { Input = 31, Expected = "0xa6b11c2eb7179530827899bcb80cd527723bb11bcef1d39aca6e5891" },
		new() { Input = 32, Expected = "0xa87194e797f003637206b54a60c79980ace0718f033ccc6b8b6bbba3" },
		new() { Input = 33, Expected = "0xea9caa4460b7156e9d552ed04ebf5717484af73ca8c4b2e92a625bc3" },
		new() { Input = 34, Expected = "0x95fca03fba53a6ec3276aee6ff7feb9a39cb444d734fc57d191c7bd5" },
		new() { Input = 63, Expected = "0x867ab82bf05d985a1d94b52cad97c9b0391288aaded4e4725ddc4445" },
		new() { Input = 64, Expected = "0x3622dc46f016fea7aeeefc2e93c81cc42944658e1c9d20b2f366c322" },
		new() { Input = 65, Expected = "0xa500a02d8a062490b3acc41185045df59b5a4e60ffc4a76419ae2e9f" },
		new() { Input = 117, Expected = "0x584ff177b0c24d989e5eb8f769024696ffc2e930f5b0200ed8035e25" },
		new() { Input = 100, Expected = "0x6ac04f82d86fd241bed3fe0621ac191957520251f654241a3eabf1fd" },
		new() { Input = 127, Expected = "0x3eceec70509157838b88bc107d625d2aeb6484c7ef5f92b8f240d51d" },
		new() { Input = 128, Expected = "0x9ca3d82bc27f56d5c1bdd1f90ddf3b823b86133c9d6c00dd1379f933" },
		new() { Input = 129, Expected = "0x3bce24c01916e2eb0a7bb0c9cfe9aa86fa64055f49ab0e85e655aa7b" },
		new() { Input = 178, Expected = "0x554eb0b30d67d9e6465b4d98377750b495f8c265f6aae8ddb2983460" },
		new() { Input = 199, Expected = "0xcee26fccb1f901675ebe31780778230806fc8fbc2c25131766ca5c83" },
		new() { Input = 200, Expected = "0x8d9dc116544184744c67ff62335090bdf46bc03e9be2e7180f97c1bb" }
	};

	protected readonly TestItem<int, string>[] DATA_SHA2_256 = {
		// NOTE: Input denotes the number of bytes to take from DATA_BYTES when executing test
		new() { Input = 17, Expected = "0x0fd3f87ae8963c1ac8aabc0706d2ad5a66c2d88b50f57821b864b093263a7a05" },
		new() { Input = 31, Expected = "0x209ef563d4ac7d51968cced180be0145dbd4d4c9688bdbdd8fcdb171029bff35" },
		new() { Input = 32, Expected = "0xa910d364190b6aed1c0a4198688a1a5ac4b37205c542d665be0f5aa558ad483e" },
		new() { Input = 33, Expected = "0x8f2d5d44ca1a2f534253a600c4e95f315133f775127a11bcb22db928efbd638d" },
		new() { Input = 34, Expected = "0xda8f41e9f2ac0effa4815a50f599b0791f210cb85f056672404639c960f56fe8" },
		new() { Input = 63, Expected = "0xb06a88f708c40510cc132a5108c6f26a9a3f7f6d42e0143baaacaf96aec16952" },
		new() { Input = 64, Expected = "0x3725408cbe6e81f8a05bd2f1b4618a356235b7262eb809608bc4e3dc38e4fa1f" },
		new() { Input = 65, Expected = "0xaf29a07c4c9ca57aa087a3c6134573615ec8b54706c75361cfd23fba38d8a5d0" },
		new() { Input = 100, Expected = "0x30cb592bdaf02c26fcba00c055059d9c3cf74f10a7eb49e2fcd4926c86c85e00" },
		new() { Input = 117, Expected = "0x1e34859b3591e50f8522d707a554725591603b95725d8d16f9dc728f901091d4" },
		new() { Input = 127, Expected = "0x6b3e56f2349c09aa0a814a0c5a9dfb72e13b79c57d3dd5bf802ab00c5040164b" },
		new() { Input = 128, Expected = "0x75b01600de565f4138151f345028a91a8471385509dfe27e2d07096b4c82136b" },
		new() { Input = 129, Expected = "0x5536bf5cdf0739e4ff259eb79a4276a009717e371057a3b8afe4ba79a03a884a" },
		new() { Input = 178, Expected = "0xad69c11f5d88dc4b047174218e843fdb29dbfb8dd2697f017bc8cd98a6a7b7fd" },
		new() { Input = 199, Expected = "0xcafebf56cdeaec6505b97a0f52369a79fa441d4d2e5a034d16ab0df00172b907" },
		new() { Input = 200, Expected = "0xd20e764994f9a21ca01a3e9247bc70618f39663773c3a7a839d8a2e1072f182d" }
	};

	protected readonly TestItem<int, string>[] DATA_SHA2_384 = {
		// NOTE: Input denotes the number of bytes to take from DATA_BYTES when executing test
		new() { Input = 17, Expected = "0x86b2d0189776966214f3469254c4a2e9d4fadbb81aab5d9ef8d67f085301a5128758c8f3b9b89d8d4460c684fe181a58" },
		new() { Input = 31, Expected = "0xf19c9457db4e320f0a795dd911f46e4def8e57f567b0e058eba7ea7de7277e0e0cf9467d567f3913af7bd3812a999901" },
		new() { Input = 32, Expected = "0x60e13c214f9ecc37ab48c67beda727612a635d9e67114c83b34ed44753a65d00a424fbc812f1ec16f93079d7ae97a939" },
		new() { Input = 33, Expected = "0xdcc50f12c899f09c44901c549aae1d3d7341b2c6b78f2e566c671631d8df1e74ebf5b74f5230b92401ba9b74e75a4e67" },
		new() { Input = 34, Expected = "0xf8a0491ef325a3af1ed02eac4e9bfd7ef645a1312318e0b5189300850ead5016194c39af296643dd5230c3b5cfa15479" },
		new() { Input = 63, Expected = "0x2adbfe51413f5d3458581dc9b9ce713b6e96ff6208fa4716cd012710e6a2d834681d32b1915e661ebfcf8dedecc08c85" },
		new() { Input = 64, Expected = "0x483f8d2065879e98c9640230d85cfffdcbf99543d7a2f24c045cf08ef8f53cb5472c93c1cd3655f35903ac91926ed2b8" },
		new() { Input = 65, Expected = "0xc4397852b5944238dc167821e2f51e80ff736c0050b1abbd0400c8db1eeb4dc17e1fdc0ed9a0d61d2e2bc29ebbb583b9" },
		new() { Input = 100, Expected = "0x5526d6e720647cc23e1ab86a51c8e8601579b6952e5d610c4b450e41292e6acb073439b91fcdd75041f475530c033323" },
		new() { Input = 117, Expected = "0x7ade74e0a89e7ad77e76e9a35c04f67c933d8f4cab485d1628b0ced9ccc17f447ba38f81ebac28a4618abc006af4e5b4" },
		new() { Input = 127, Expected = "0x6e23e9d0dc3ee1ccb08f1f9568e8fc5d8d85b8b5a01afe63946894b39d68691330a63bbeaccc4fd6bac141c452feaa0e" },
		new() { Input = 128, Expected = "0x3b9d1126768bc0e16c6484a0025f492893a92927eb42cc645c23c22a6a5252bcb7b82ac748f0a99a49ce2ccdaafa723a" },
		new() { Input = 129, Expected = "0x2703c12554db5b80ef25b7d2dc4f0233b7b7064e69d57eff39b12aa77ad3c8b2e5d8014506179fc76399da952b2ed985" },
		new() { Input = 178, Expected = "0xc21fe026e7ba3c8e845512d39c592beddf903e6df81fb8ec0637464c279618b1f10a91b5291f1ab698d9354b61a3b2d6" },
		new() { Input = 199, Expected = "0x83843225d4dbfd455676885ea3b923ba2e0fa536a53c713365b5335623897840588d30260a4ed4d392c18efb6c96d946" },
		new() { Input = 200, Expected = "0xccfe1529f08bad44c42cf6bb96497f3474fe69631a33b58b4a28833e30dc7a404d63f5573dd81654e0430d92034b2b8b" }
	};

	protected readonly TestItem<int, string>[] DATA_SHA2_512 = {
		// NOTE: Input denotes the number of bytes to take from DATA_BYTES when executing test
		new() { Input = 17, Expected = "0xf729f844e23dadbfcb53c046407f03e790a7a9ec6004c570feea461f76b066353dfc5cca95629360d5ea310719bf6f0251a56e9c515b62b863206d6ff64b6784" },
		new() { Input = 31, Expected = "0x526be8f0afbc7ffe77f62456f8d47b2e60bdad5ff1955841d9bcf82d9a2c71a9a2bdf4288d025154ff43ba65b4d4adb97ac24f47c27a28af7af0b2d831c9c7a2" },
		new() { Input = 32, Expected = "0x3bbcc5f450e9b6708c22ed0ba40b5265d3b32130b9ffdcd06bfc61c49452aaabc8bf08df544f55935952c80d0e266f27f3f66ab4aa1b2f3e7b58ee0708200d79" },
		new() { Input = 33, Expected = "0x10279e84bf5f4debae99ebb1c2186a3b5a510da642c99cb77ab981f39fbf55d20ef70fcb19880b86929dd7db3a4b2259b4b86d82a38b200933d550c42d729a57" },
		new() { Input = 34, Expected = "0xb5c4f53ee9d151543fdb42640650e4ff930d2f145ce1986d6a8b3b1860a0136ec889e4f02675a99e0118430c9c8357f974ee99d0e52b62b92016ac2c6833af5b" },
		new() { Input = 63, Expected = "0xa35de82665a3c12424e5a11acc356b329a56b15bee61c2332ec04fee142ad7699f9834800e127c0146827d8b84ad1ce0b57f2c5ed30afc0768e098a5d621dd97" },
		new() { Input = 64, Expected = "0x6dd15a36cb5ae97d7ba0c74e19adea2bb4c243839f58aeef83cd8527e87c43069d0a02804dbcb281636b8712f6e546f31946318a709019ed11f3816642eba77b" },
		new() { Input = 65, Expected = "0xa2433136dc3bd4f0e2d4d14b6033e1002f675c4ce842d7baeee78b95193030c647af66f0e54ff94ae3b60e46a88314a4a145f30267f3fd0990c6ebc2970b9fbf" },
		new() { Input = 117, Expected = "0xb4647f67deb7347a18d43d87a4143853855fd81602baab1edd8a08b32a74268adb12fc03b6d1a05d81e67dc75fa93386749dc1d40d988a685ed1550a5849b527" },
		new() { Input = 100, Expected = "0xa55acfa8808e502b5f02e23f6f824b56fbf6e8bba3f032d7ffd5b254200de521299a4e8f593c453c1483773cc78332d54f1016af2cbddac68ae7fef7aa399219" },
		new() { Input = 127, Expected = "0xd33bc6775743bd1110f51b84c0ebbdc57c622890b20d53b754ad9a1937e2761a1747d9adcdc2ec685549e418eb6ec3943c1e88d8e4a698389542547256522fe7" },
		new() { Input = 128, Expected = "0xf03557fc390333279816513d69a4e389ab51df3bf1a06b666c816c18f98c8dedaf338eea98e3063cd728ebcafe7d59dd19eca2bef4327a3421eb1e921af5d223" },
		new() { Input = 129, Expected = "0x5af2f48f25c994054c624afd99c5c9a59e91c492facdb65068cc1a15497f65ba0f6c5d15dc2f176f10ea6130c2894339a02fb99696b39b6c634066acc590427c" },
		new() { Input = 178, Expected = "0x8dc7dbc6d4b1ccd92948804c6474e5f94acaf59f4d908f86603abd3c7d96f18dc1d1723a22cef7b6e0ef9a6c1c33f390c4c85a9e1fd4c4fd4db3c867564f1d81" },
		new() { Input = 199, Expected = "0xf239e971dfa284808c7e95a9726e1f42942e431e2c942e84d020c580a7a4a8c1a7ca35af44f2efafee6d3d929c01c30f0588c01e8e6813649fb86b22f0369cb1" },
		new() { Input = 200, Expected = "0x5a9aee4aed39dd405980b29984dccc6b520b685c6beb6e42c3450b858e1cc45de9d235849fa743738a06514b30522180d06f98185a49919191e86374a79df3b9" }
	};

	protected static readonly TestItem<int, string>[] DATA_SHA2_512_224 = {
		// NOTE: Input denotes the number of bytes to take from DATA_BYTES when executing test
		new() { Input = 17, Expected = "0x4846bb2631c431152f50e9d953879d2e8dd0b5c269cf3adf2fabda1c" },
		new() { Input = 31, Expected = "0x0ca9ed8697815c00879139b2f33d41ec0a847883db5f5b5e7eeab54f" },
		new() { Input = 32, Expected = "0xc2f367157013a6258bd0753972ce564b897ddadde28e00e73eaabfe6" },
		new() { Input = 33, Expected = "0x16969daf39bc1000dbea1b75fffad0041d5701a49d89afd59b4c8aaa" },
		new() { Input = 34, Expected = "0xda2a0e540a2c766df7f6947f386cbf7e8cb3973a0ad121f00305cd62" },
		new() { Input = 63, Expected = "0x1fa0dd121ea2ec2bf42b1129be65d77429155235a1508ea099e5184b" },
		new() { Input = 64, Expected = "0xd58f565a4167fea1c806929f9b721989c06c839e966c8bdc9dbbe4f6" },
		new() { Input = 65, Expected = "0x67ab11a7aa72ce3bfefa7ea4a8a4fb33be011bfbca9508a226597f5a" },
		new() { Input = 117, Expected = "0xa5aaf3fdbd5e45368fa4641cb833da3b613705b94733a40ff152c738" },
		new() { Input = 100, Expected = "0x3ede9bba66ecb29f4559e6f81901090ef5cd0f222e0268be446d1bef" },
		new() { Input = 127, Expected = "0x8274a43a288313f4bbdb4143b618c840377f097e77e5f05520c509f7" },
		new() { Input = 128, Expected = "0xd83e6f8db0fbe873ab1df76db481bb4c3c88ceb38308a7c3376c7824" },
		new() { Input = 129, Expected = "0xf65099cf64497bc93719e9ffe933d1a4501e106f0f75575f8c44486e" },
		new() { Input = 178, Expected = "0xbc5a3540c403d93ab1669b4ad75da366a100f9063131c7f9a17250ee" },
		new() { Input = 199, Expected = "0x2c58eb005441bc5dc5e4dd869dbbc705e9fe8490a4798c3c60fe1b0d" },
		new() { Input = 200, Expected = "0x1a7e56a2bdaff2335068ab04dc3780a7892754422ad37a28b08c659e" }
	};

	protected static readonly TestItem<int, string>[] DATA_SHA2_512_256 = {
		// NOTE: Input denotes the number of bytes to take from DATA_BYTES when executing test
		new() { Input = 17, Expected = "0x04130f0c464373fcb0bc93d112ab700f21c2d444ee0a6b40768c38b2149b2433" },
		new() { Input = 31, Expected = "0x7572a6bbfbae6df08915f5a0b51dd4725118920a3c604fa860bd811c72e38c49" },
		new() { Input = 32, Expected = "0x75c693f7ba26912024b3ec751140112bd07d5092b3b8b231e2ba7491abdeceff" },
		new() { Input = 33, Expected = "0x8ad6db5fa09f8c51d7366d6222d7f847aefc66c6eaa27f06f365ba90507cf814" },
		new() { Input = 34, Expected = "0x1df568b6cc787bb3ca2fbc4cb87a7ca8fb0a241eac24c40ac3814a19b12c6be1" },
		new() { Input = 63, Expected = "0x18fc0c80956a8ad32f64262d29fc6003b2327db14f2c36087cbfc7e6789f2545" },
		new() { Input = 64, Expected = "0xfa80e2986700af9c2872bb18a580c65a10456842c82df19c38761e475b38b355" },
		new() { Input = 65, Expected = "0xcf5421cf033b3e739bf46f7ac4408661ec45f850b527e06ca82f60629ca1caf2" },
		new() { Input = 117, Expected = "0x24e7bb55dc160fb0cf6cf5b70d19db9f52eb3b0a85f41da693b2b10c8ef4dfab" },
		new() { Input = 100, Expected = "0x6ccd786cce564faee1f4bedad4fb07e66834d6b814ae2fa2c56effa02a8bb662" },
		new() { Input = 127, Expected = "0x2b4fee91d9f575419513199df156b1746d9040d7c37d0d16db0f795e85aeddc1" },
		new() { Input = 128, Expected = "0xe62bc7de3bd602dab41e571d660d41d0c200c05270af77c64a58bfb74e1a73a2" },
		new() { Input = 129, Expected = "0xa199d5defea788fe69a47119d0a5a1246ed922144769dd3ff45f6b6063298609" },
		new() { Input = 178, Expected = "0xa36521aebb17176d82c9ad7ca21ad983ccc84705fdb338cb4eb9aaa8e9cc3030" },
		new() { Input = 199, Expected = "0x8c0c7a127b552dcfbe5afd3491c44d3a2e844a503c3c40bf5174a3fd8cb6a185" },
		new() { Input = 200, Expected = "0xdb6933549c8e47196fa38c1371b98a3da217ede8845b46b8f431ed24d1d41a98" }
	};

	protected static readonly TestItem<int, string>[] DATA_SHA0 = {
		// NOTE: Input denotes the number of bytes to take from DATA_BYTES when executing test
		new() { Input = 17, Expected = "0x58d746a49b7c66182d2b0a3de6b3cea9570478bf" },
		new() { Input = 31, Expected = "0x0c77f16ad91b6bf9205b7d5c17932db903cb65de" },
		new() { Input = 32, Expected = "0x9ba2e43c9380328723f9ae4117e0c97e7cc47873" },
		new() { Input = 33, Expected = "0xb0a9818e4736af5cc1383d4d0ebd9ef51db79ac5" },
		new() { Input = 34, Expected = "0xe4788dfcba1f4c44863d1cfa1df8f730881f2ed6" },
		new() { Input = 63, Expected = "0xb4ec5d267abed1b4f01e3a16e35c1d1c95ba5364" },
		new() { Input = 64, Expected = "0xb1c666df938f8960f94b346b32c29a29bef38583" },
		new() { Input = 65, Expected = "0xb8fbd2f077c0ecaf3d9fb2880262458c6e73f47b" },
		new() { Input = 117, Expected = "0x17f0802abd5a546b633ac001f597dd2eddfff749" },
		new() { Input = 100, Expected = "0x96bb42741cbae4679e0d35870358e746eae2ae79" },
		new() { Input = 127, Expected = "0x704e7554ccf7443fbb6b42aa7f02c4ff7004f48a" },
		new() { Input = 128, Expected = "0x5bf6422be153fa9fbf7458d912a064ad0243c9e6" },
		new() { Input = 129, Expected = "0xc0e3e2956e24dd5ce94908b777037284487be358" },
		new() { Input = 178, Expected = "0xa3ec09bcf28e3eef2dd841816b1aab4d290f8315" },
		new() { Input = 199, Expected = "0xce56f5d8b0641be06c6baf1e4da20e22afd461d5" },
		new() { Input = 200, Expected = "0x4d071fd25625c9e0cd464a464544d11462e42037" }
	};


	protected readonly TestItem<int, string>[] DATA_SHA1_160 = {
		// NOTE: Input denotes the number of bytes to take from DATA_BYTES when executing test
		new() { Input = 17, Expected = "0xa96776b1ebac72819ed9460a79ca95fee5157ef4" },
		new() { Input = 31, Expected = "0x12a27fbb27caa2aa543f532387112de2c02c22f5" },
		new() { Input = 32, Expected = "0xb07e26220aa339c876a9d7447f1f6e007605fe4e" },
		new() { Input = 33, Expected = "0xf0b72467af9526525713107c95adfd06a940e216" },
		new() { Input = 34, Expected = "0xd48d3738ee1951b16b353a5899082ea722ec4cac" },
		new() { Input = 63, Expected = "0x9b3dce9e98dad9e8ffa08125010b606fcfde925b" },
		new() { Input = 64, Expected = "0x9864ce90af0ea8aaaf2e2f5fa280d5d7508f1d51" },
		new() { Input = 65, Expected = "0x52063ffd94354bf08221e9d7e51f4280eb41cfc9" },
		new() { Input = 117, Expected = "0x447fe81cb06fa0d1742b160ec262e1c41be6aa3e" },
		new() { Input = 100, Expected = "0x9a8af7b7861bd8d6edc41d620138586016a23abf" },
		new() { Input = 127, Expected = "0x7cad901b4dc525fb74aab345b61f507c34518e53" },
		new() { Input = 128, Expected = "0x55d8e062b1cd400c57c3f1a799c4ec0aae75a0f9" },
		new() { Input = 129, Expected = "0xaef5a27d651680bd9befc52bba81a9b56e03211b" },
		new() { Input = 178, Expected = "0xb910353e0c40f90294cbdc6d89c8dab2fccb2325" },
		new() { Input = 199, Expected = "0x48d1a8132ce4352b5c14ab8c08d7d7c15e5d8a79" },
		new() { Input = 200, Expected = "0x368fcfea12ac5db4964ed7897caf3dfaf6d2ae38" }
	};

	protected readonly TestItem<int, string>[] DATA_SHA3_224 = {
		// NOTE: Input denotes the number of bytes to take from DATA_BYTES when executing test
		new() { Input = 17, Expected = "0x7730f0ef02ba83614514808ae2728fb6d089e5cc60440f568836b216" },
		new() { Input = 31, Expected = "0x00d0b20e7b22756c0603201e58c8df8d7a5b7532ccaaa11dbe41ffee" },
		new() { Input = 32, Expected = "0x98210936602200db9c058cef6dae33314b2952c5769c93c19e692149" },
		new() { Input = 33, Expected = "0x17e3bbe10a6325b88e7dd15539f08597eeba629ed6a915616021e071" },
		new() { Input = 34, Expected = "0xa1a0db03f757dfd84a62864556988801e328cc449748ea6985daadcc" },
		new() { Input = 63, Expected = "0x20b2b6e3aec9110ca2b0de08db01b6517d40131b424860c844c5e93b" },
		new() { Input = 64, Expected = "0x7a771ffc0003021bd275b2fdd9579fc4ae14348420b01ef7e4d6bafb" },
		new() { Input = 65, Expected = "0x0226f4c1ea8b5fe23c79d3f17e249de8cd2023d9c7b3117f270e2894" },
		new() { Input = 117, Expected = "0x661c35c558e4c5bf3596cc50145ef880e17cedcdab37237b6b39eef6" },
		new() { Input = 100, Expected = "0x8843e5e01af499d6af98c25b7c26fe1e0c9a880ea37d11f4ec93d1ca" },
		new() { Input = 127, Expected = "0x0a1160c16e94fb1bc4018308fd1774b9a4b1efaa4fb6b12dc1edfcc3" },
		new() { Input = 128, Expected = "0x466ca95ec19554fb6ec249f2bcd1fefb4c81933047f3783a3867cc88" },
		new() { Input = 129, Expected = "0x739196e95833dfcdb5cb15691816c39412d9388dc9b61f95384a01dc" },
		new() { Input = 178, Expected = "0x21115bb24ba5f07295bda0b91fc63aaacc5c8bae90fe79bb87b9c168" },
		new() { Input = 199, Expected = "0x2ef98a1711d88db9c915c80c2450342ed90c570334baa298df1f1e17" },
		new() { Input = 200, Expected = "0xd53776471a002f4f8d132a896d2a7dd7577c3029d3468fa28f94e8f2" }
	};


	protected readonly TestItem<int, string>[] DATA_SHA3_256 = {
		// NOTE: Input denotes the number of bytes to take from DATA_BYTES when executing test
		new() { Input = 17, Expected = "0x84b6a1cf6df74b3a54da73cf2ae3bca8426fba94908199bba45ba1ccc8f680d8" },
		new() { Input = 31, Expected = "0x49128a80ce9b14b46c310adcdfc0be99266ecd0728b4a12a7fdaa000d49c4106" },
		new() { Input = 32, Expected = "0x60c394688e6a2eba3d14edcebf6b13c95eea80a458bf3f557e55df0dd710bebe" },
		new() { Input = 33, Expected = "0xfeb0146e6af5c99e7dc931f28fa2c965c1e16a9360bb7fc5eacbd6658115b114" },
		new() { Input = 34, Expected = "0xc247d6b3649e736004601810655ba1e7041c40a73ee5fd5d408e891a90f38dbb" },
		new() { Input = 63, Expected = "0xd3e6fd4abf153070e11446c6dd1cfe748064239a9f680437a4b1d51c5c64fa2c" },
		new() { Input = 64, Expected = "0xc5d9eea9c7d04746dde6e94cee94105a5d1f173809849c2d2953e31b3af5d556" },
		new() { Input = 65, Expected = "0x81bd225df0d6dd4ed5347dbf688b4940b9a0f085db9a5efd8fa4dddf5bea2e9d" },
		new() { Input = 100, Expected = "0x5746f720dab78746407d4c594fda4a2539949183a0208553c8aee1d578b72898" },
		new() { Input = 127, Expected = "0x4230dbf66b2e324d321fcbd6ffbfeb0156e3070af672dc0c743b5001d6e530ac" },
		new() { Input = 117, Expected = "0xade65df24b483b5d51e8620dd05966dd89b96c90b69322c19d67c3a968f5514d" },
		new() { Input = 128, Expected = "0xc19c584bb6969ba83731d2f21025d556b9cf08a9e598cc97cdc5f021675e7a90" },
		new() { Input = 129, Expected = "0x82ea34a1f09ebaf85ad11efa05f81e9e7a8d6fbb62e04cfed2e5f26c4d1f09b5" },
		new() { Input = 178, Expected = "0x471ea99294ac57486166be9a3e3da3cbf588adc0c6606c290dddd513632931ac" },
		new() { Input = 199, Expected = "0xaf6df45fdc24388fba66baa4484ace35cdd01aa6a0f9a635f564c1ba5b1fefd3" },
		new() { Input = 200, Expected = "0xcd31079dc52963c7753ff9b8640ce60404fd44fe4464af475229aa704cb5de4f" }
	};

	protected readonly TestItem<int, string>[] DATA_SHA3_384 = {
		// NOTE: Input denotes the number of bytes to take from DATA_BYTES when executing test
		new() { Input = 17, Expected = "0xee2621cc2dc6f234c8976a1ac76a1eb8724213c67af5a704ba56a7bc92f09e146e1a1d7d0a5a4ae5405e8b9295fdf216" },
		new() { Input = 31, Expected = "0xcac5638f7c264b72d01942b8109667b44142293cd1ad7bae06bcca65d82a5f72daf27070b17702415e9c3d501658ce57" },
		new() { Input = 32, Expected = "0x509a74fbecb9d7cb23838a31bcd8447d73ae0893d2a60c53d6327467a2861e07b39ce800c01329ae2e06d1b3ecc905e3" },
		new() { Input = 33, Expected = "0xcd6c73588fce7db1f3d59bdef9f544b6f08b2c50ec0b01dd012700d4274b80f4d0ff20ca774b27f04b31ef9f19bf0cc9" },
		new() { Input = 34, Expected = "0xad76006715dd48f0138420ae2c3bd7d5e64ba735a307323c00192acbe837cec5cbe04312a1602ea757de41f18d0fde7f" },
		new() { Input = 63, Expected = "0xddc1e64c8420ff5579eceac10844684d08cb769cf578925e59d98c79f5be736524ff44738a16543bba47d70b1ebcc36e" },
		new() { Input = 64, Expected = "0xf29ec08d00ae2072137288e31990f2858629e23d2365a84a079cc5986dbcff1b16a19216aceb079e240e89626644bb3e" },
		new() { Input = 65, Expected = "0x9a0bd293ed9ea460387266b65773bd73cd8c5c6ccadc0d1b901f35d1e82571a10b63bb90beeac3e1a0fc29786da0beb1" },
		new() { Input = 100, Expected = "0xaddb1229b53c3a35d1f974cfe7a1c3a6f6803996d72cbc13bf50376b85105b86b1fdeacdbe51525928e39e38ff23b1fc" },
		new() { Input = 117, Expected = "0x5c142da18a1e2b0f66f396e07cc102106227638a93d9cc5230b2c8ade550fab096049acb53fb5b357039983b77193460" },
		new() { Input = 127, Expected = "0xd3a0c04b1350044d29a099cb5d95175539e93e1144f471d27bbcae555864a3e7c87bbaf7107e8335206aebb2067c6e1d" },
		new() { Input = 128, Expected = "0x7538b4cc1d1fc9eb921f5bea8dda949b43e1f2e8fb7dbfd2f1e7b01f843dc5914fe7983cc29f53ea52c91da5e0e38a7c" },
		new() { Input = 129, Expected = "0x699fc858bf267ab42444dc5888f53e55c8bd7f195cda1bee192d9471fced05a25370f98d1e8a20127e57422fb226e499" },
		new() { Input = 178, Expected = "0x03c546e8f629538bdfe523e4776b9c4fce59b2c523a57482fcf212d617e63a7677b98ded0878b317e1514de278c58aec" },
		new() { Input = 199, Expected = "0xa2e0626bec9c34d571ec7079d0186b0235c45cc2faa165ca619c0ebd290f0292e7c565ee77fce106af58e0d30e7b673b" },
		new() { Input = 200, Expected = "0xaf0f60050d97927fa2becfd3b7938e31c20ff3576bc3adde5d51428e91de10102e3c49c24ae7e515838952e53709a67a" }
	};

	protected readonly TestItem<int, string>[] DATA_SHA3_512 = {
		// NOTE: Input denotes the number of bytes to take from DATA_BYTES when executing test
		new() { Input = 17, Expected = "0x36b8e099d4afb54a9aadb5c76154be673a96967a73e462fb401c21282a2c4554b832f323415c047156e3452e77070a085d14543b123b473ed93d03248514898c" },
		new() { Input = 31, Expected = "0xdfc10d8ec28d43efe3cbba1c1e1edcb6f71c14d9057941afc590469350402e8fe1298de2ba20eaa8280dea009668d5dde5f7001b65fb9237284c8b60e6bf4e8f" },
		new() { Input = 32, Expected = "0xe4290dafe0838e10c8752074731d7fdb76c4d5d632f75f2c508b357d344c622b8e5aa9ba1d58f4c859bb49b4b81a25c1faecbc08317ceafc00e1c3a9945295a4" },
		new() { Input = 33, Expected = "0xdde23aa602cca8efcfa9b026cf067ada1b8bc5487b4dc029b31621294d5be3954e402ddfb4e5f9a0401648e6e649a0f05f647e61457289f705ee167c86f6c3db" },
		new() { Input = 34, Expected = "0xa54f15ec275b53cb618ca462bb0de1776e1038f2cbc40df2da6a7e5e1333ba475fcead9e0c55e357547feca9a973f781bc9e601c7570a0f510414e27167be834" },
		new() { Input = 63, Expected = "0x6971211bc158034f3420850303953d8845f9657871af4d35d71f75eb086e69c07f4e63eb173962d53279400688ae3637d2fd742255b93e3ab6bbe1b203243586" },
		new() { Input = 64, Expected = "0xdec734f489aefcd5ad355134ef6fd1ebb18c8f741d16e0fedb201dd801905a7f39c2824b67b2b995679c8266530b527e2dd2af59f044cc5d034d93bc7c35efdd" },
		new() { Input = 65, Expected = "0x40b460c3f18d2c0aa076db67af63c3d22a6c3d29853ca642204d3ff5b0649b394f2e10beaf78be0929cb499b24323462ad7242a3e3e9c7b7a89a58da4358d1c5" },
		new() { Input = 100, Expected = "0x480d6ea46a25eeb45a2eaa1a23304d68dba624635772d26a21fe8fe56376de8d298bcb5f5d48e59aa6193a55170ae5a1d15f4f8dfe7fdef7706c0686eb39862f" },
		new() { Input = 117, Expected = "0x5b7e1c31bf4358a77f1afb7f2c181cde1bf87b3d9e94fed09d82a996364998ee3e46b9e7ab94337ad967878741475b2d11061de00d06e1db3026e2859ca2af32" },
		new() { Input = 127, Expected = "0x73b63d13c3e4e9dcd9fcce0adaeba4423ec201aa7e13e33faba2b6fbc35efd76302148fc964f7647b24d770ae897c9d5ca0211e4b1e27a81fb769ecbfefb1511" },
		new() { Input = 128, Expected = "0xd5ec5de877ef0a39eefe294f6183b63adb91d2a0ba1ec1fd576db515ed78f8220442c2347bdeb8a0f77cdc46d97e5b96d4189fec1f5cd2e8b5de3d467684ad73" },
		new() { Input = 129, Expected = "0x461f2ef3ddb3101d2ae5b1edea9178bc431225a9e5bec7c04e446a70db25f2e8e9b24547733667f0794286a330297d11215f21da7b5eea03adf063193f5f49bf" },
		new() { Input = 178, Expected = "0x3f71cc9ed5acf47e4b994fb36bdc306c7e777a400532e0c0ec7e2ac1796c4471d39a09d7e32473e7bf804e4b342813a87f8f11c85da3b08f50cfe8af3f690d12" },
		new() { Input = 199, Expected = "0xe2e4d8eadf49edf7c0b81c97e0c115064a6788eda531df390b88d09586dd2f33f551c6fe4f930caaf3e6d24e7f3dce49c9ecfedb5ceeef796c1afa1776157736" },
		new() { Input = 200, Expected = "0xd62ed867af9fee338bc1cc712fdbc0da15afa40b4a5dcc3e76d74f1770c5a7ca88638f0cc8bce685cae8d68a2aa8717c84bc3e146100aff25c3326355b1735aa" }
	};

	protected static readonly TestItem<int, string>[] DATA_RIPEMD = {
		// NOTE: Input denotes the number of bytes to take from DATA_BYTES when executing test
		new() { Input = 17, Expected = "0xbfdc1db3335fd40c6fcc745196095daa" },
		new() { Input = 31, Expected = "0x407c88ce6996902f3c0494fb12e6f997" },
		new() { Input = 32, Expected = "0x0140e0c8caaecb37a6b85b11be585876" },
		new() { Input = 33, Expected = "0x1993f0152058eafef8e714435e9a25f1" },
		new() { Input = 34, Expected = "0x7b081119355014700e8862bb5a6fe154" },
		new() { Input = 63, Expected = "0x985584ca8e46e4760d81a8a59e6489a7" },
		new() { Input = 64, Expected = "0xe7d34cd24ef82c527cab3d8ef2b72571" },
		new() { Input = 65, Expected = "0x3b67767584326eb649bfa58fed154900" },
		new() { Input = 117, Expected = "0xc89a71c44530331d76d7ccf61ea01b39" },
		new() { Input = 100, Expected = "0x85263382da7bab1e7ded9eafefd83d99" },
		new() { Input = 127, Expected = "0xeb53d82d2028a94cf2abebda49cc3988" },
		new() { Input = 128, Expected = "0x0d33e8b984fda75f525449ff706675c5" },
		new() { Input = 129, Expected = "0xd7e7e34f26a717a82dd09184c336965e" },
		new() { Input = 178, Expected = "0x66f56cdfa628da7ad1e9c9fa129edaa9" },
		new() { Input = 199, Expected = "0x0c475e8a48a90ffd93dfe1549e36c129" },
		new() { Input = 200, Expected = "0x78f3f537566884f84e157a5ad7062463" }
	};

	protected readonly TestItem<int, string>[] DATA_RIPEMD_160 = {
		// NOTE: Input denotes the number of bytes to take from DATA_BYTES when executing test
		new() { Input = 17, Expected = "0x734191cffedbbe96f14865d2eebe3650e54c6de6" },
		new() { Input = 31, Expected = "0xf5c19350c4a7a79f1597b7172ff52205864c92e7" },
		new() { Input = 32, Expected = "0x29c74325055e81d14d7165c28599e311c9b63c6a" },
		new() { Input = 33, Expected = "0x1f54c3702f8dff024a6fae7ceb017a64f71b15a6" },
		new() { Input = 34, Expected = "0x1068b29dd5bd6aec7cf04ffc1ef671cf83e7f239" },
		new() { Input = 63, Expected = "0x5de126808d8b2656c8f91796eb2dd86a9fe65ad1" },
		new() { Input = 64, Expected = "0xbf4c1c78a8e75584c6697fc2f1706e0c41c9df59" },
		new() { Input = 65, Expected = "0x79123df7d67e2a3c3cdf3f1529deac143d44ca8c" },
		new() { Input = 100, Expected = "0xef7cdf0a7ded768b4675a743ac7ab64c3bc5fad3" },
		new() { Input = 117, Expected = "0xfb82dbfbb359e2f5fd3bc0a00a9bb7e873bda70d" },
		new() { Input = 127, Expected = "0x67073f8cb7f372f93bd57f289cf3829d801e78d6" },
		new() { Input = 128, Expected = "0xc923752f5fbb9721a48c5f1dbcfbc70865577869" },
		new() { Input = 129, Expected = "0x6ada1e777ecaacc07922cf839e1259d1f2b8afce" },
		new() { Input = 178, Expected = "0xabc2c368a457d10bc300954a4036b3a33eae7128" },
		new() { Input = 199, Expected = "0x31ed25a6a35ba860abc0804c6e8c3e3e6174099d" },
		new() { Input = 200, Expected = "0x1105e599abaea1b0f8d51c3878729ad0ca619a4e" }
	};

	protected readonly TestItem<int, string>[] DATA_RIPEMD_256 = {
		// NOTE: Input denotes the number of bytes to take from DATA_BYTES when executing test
		new() { Input = 17, Expected = "0xb242099231d61f0d6c83044d360524b499a434d0ff12407296d1061e017bd023" },
		new() { Input = 31, Expected = "0xe71778fbcc7b32156c66e244a6a07d10e463bb20cc35ed98c8cf35191ec013d3" },
		new() { Input = 32, Expected = "0xd538e7bdd392ee4ec094a2a50cb6edec45537a87fd8f4a72a7fc573cd5ce43c7" },
		new() { Input = 33, Expected = "0x7e1bbb5611223834cb1cee497b700c70cc27bbb042c2431fccd4ec67965567ee" },
		new() { Input = 34, Expected = "0xa73d52f35585f3d4dd34850bf3e8de4697ad1f94cba71321d6784785f29ed905" },
		new() { Input = 63, Expected = "0x48d647a2e1dc581b675daf26f0d08a11fff402a42c47d132f52133bb8a6895f4" },
		new() { Input = 64, Expected = "0x2cefa11f6ea8dddd1d0c935b4f04f36c1631b1589eea6082ed53b3e9b54cfc72" },
		new() { Input = 65, Expected = "0x5a2a91bab4ca44664ef1d16fb8f8cde48ba2dca1cc0c0faa636812b86b98fe3f" },
		new() { Input = 100, Expected = "0xa5fbe1faca66cc5d5f5dcea2550811254f221fb8761c4b5a3caf31f2f0534ad0" },
		new() { Input = 117, Expected = "0xf8cace5bd4fc6711706e6c3cfe9713234d40e4fafeb37b5dbe97c13c37f6ebc9" },
		new() { Input = 127, Expected = "0xd14265c897b77caa18c77c77c7f46f1a07faca209a16d997af794c15b145bb05" },
		new() { Input = 128, Expected = "0xb286ca27b0ae4f6c18886879f9713cd959fff512535bcd379943c95dcde7773f" },
		new() { Input = 129, Expected = "0xccfc63b15e2e810a36f3d26ed3b1bd49f456d1af97c3d46c0683833d37ce359f" },
		new() { Input = 178, Expected = "0xe6178a33180fdcad7cc503f5ed90b66610db900dee7326696cb4e10d1234caa7" },
		new() { Input = 199, Expected = "0x2a1c9d07ce2174a6a09a246c6edbdc4f0fd0514f0179984cb44c06b8b3c573b1" },
		new() { Input = 200, Expected = "0x29a962414ab1f46a2013178f831d66559a46d709fd3604b4b435ec4d8b536619" }
	};

	protected readonly TestItem<int, string>[] DATA_RIPEMD_320 = {
		// NOTE: Input denotes the number of bytes to take from DATA_BYTES when executing test
		new() { Input = 17, Expected = "0x3d9cd1561f939f3aa80ee5339fa11140e68f3dbcfdd928d4d31f6932a268bba329595cc1e347d06e" },
		new() { Input = 31, Expected = "0x62dabb157501ee8aee1e7364942774f5741ed806f87f31d3754e956cda45c3423d31d5675cd7fcdd" },
		new() { Input = 32, Expected = "0x286cbc2d0bd027673fdb6165c0281f3beabeafa2936d0d2b651010b473faa68fbad54c663c9d0fa2" },
		new() { Input = 33, Expected = "0x921c28a7318df3bfca84091eb48ae54808fe79e9a24d716b641c61108272114a7c3e21614b316eb3" },
		new() { Input = 34, Expected = "0xd569a0217a6bbbbd99e6f54899f14078adccc06b56be014bf3f25493763c7f6ebdb76fb0d187d0ba" },
		new() { Input = 63, Expected = "0xffac8eacef53c8e9c9b9628ae080dbf8b50d9ccef6beaf0fc318f0921aeaa4624e478b48dff801fa" },
		new() { Input = 64, Expected = "0x47f9c63000e89707be545cdf37e3697128b6ca013ea59ce576437125a35b94a1fc12b4568c2b42f7" },
		new() { Input = 65, Expected = "0x0047b303eeff27dd6d3fd9ad838cb3eaac2d06b9f909729d449052bfb648c522e17f23beef18e14f" },
		new() { Input = 100, Expected = "0xd9eaaa5d3dbe16e6d2d06b1fdae8f5a6893303f82cf7ec838ee1b94a37ba2ecb8ccb008c149586bf" },
		new() { Input = 117, Expected = "0xdeff952e2a54873158c0cb880eb8c813f03716649006b9026dd9ba1556b9be4058ac4091c36693ac" },
		new() { Input = 127, Expected = "0xd5d5fbe5fb496f65ecc8f65b114bc498bad886b826e593fe0c66b0b03b868002be71c3219a992b61" },
		new() { Input = 128, Expected = "0x39bb7f49c9be805d4ff51210d6e64fc5b48a87ad4795e1c17deef630d4ab5f93bcee15b999fd81de" },
		new() { Input = 129, Expected = "0x1d18806ff98659458e4095e0acac282c1af2815cf5967402dad2c688afa4c10b16b6d1996415bf86" },
		new() { Input = 178, Expected = "0x957de878669f2a162f50a2c8bb07ae835b857985ef68f6c77d590b89861358698ed10fe59503b454" },
		new() { Input = 199, Expected = "0x527b23083ca9c12fe6f3e9936310f7b71c594113efaaeb58c195b657406a45a70f6d918e714ba450" },
		new() { Input = 200, Expected = "0xc56063dd1fb318af5a0910ed3993c3ea3f746be8ef65661af0fb4c7451f44dfcabfe7e5db469d9b3" }
	};

	protected static readonly TestItem<int, string>[] DATA_BLAKE2B_128 = {
		// NOTE: Input denotes the number of bytes to take from DATA_BYTES when executing test
		new() { Input = 17, Expected = "0x49a48254e908ccda9ba18ccad8db5b65" },
		new() { Input = 31, Expected = "0xad02b381eedd0dcaa786b6c909c54058" },
		new() { Input = 32, Expected = "0x3eca051ea634a54e0bb85cba642e76a5" },
		new() { Input = 33, Expected = "0x785a9bb07259445048a3b7bc91dd4e6f" },
		new() { Input = 34, Expected = "0x88b51265c8e06e3d89507d71d0dd022b" },
		new() { Input = 63, Expected = "0x16d2ab174a37539fef72e687913d674b" },
		new() { Input = 64, Expected = "0xbff3913f314c7e06b1940df82890a46b" },
		new() { Input = 65, Expected = "0xcc8e88bcbca0345789e9181e469003c9" },
		new() { Input = 117, Expected = "0xb34ed87577f9f0f588e88b27c30b032f" },
		new() { Input = 100, Expected = "0xf5e0dc00a2d19abbf2345d3ba9141777" },
		new() { Input = 127, Expected = "0x2688275b5d937e3c3f4d89d3c5608ddb" },
		new() { Input = 128, Expected = "0x643cb87592cb195b31a23de152e368b5" },
		new() { Input = 129, Expected = "0xf9de2bd56e1ef7c0d1f63c84bbf5dc1a" },
		new() { Input = 178, Expected = "0xc9d17d0bc9633a267d3024c40558d109" },
		new() { Input = 199, Expected = "0xffd123448e81f72b79e009d982d1c3a0" },
		new() { Input = 200, Expected = "0x9fb45b39900680e48ff3b112d30463b6" }
	};


	protected static readonly TestItem<int, string>[] DATA_BLAKE2B_160 = {
		// NOTE: Input denotes the number of bytes to take from DATA_BYTES when executing test
		new() { Input = 17, Expected = "0xe2fb03613eb98655c07b1c7562f551576e4b7fa3" },
		new() { Input = 31, Expected = "0xe4c219445e5d612d2ad8b5f671f028c60793edc1" },
		new() { Input = 32, Expected = "0x8b2449771869d52ff7f12d83abb75286b0282f30" },
		new() { Input = 33, Expected = "0x1ff4862a308b42145d3cf1c03811e5ea07c32e04" },
		new() { Input = 34, Expected = "0x8bec0ddea1b2eb142d8048b2d39b7a251bf50e28" },
		new() { Input = 63, Expected = "0x522e511c8069bc751de1903e4cab1e4a636ee3dc" },
		new() { Input = 64, Expected = "0x16b24086bf23aef29d4acf21f2be775edc476399" },
		new() { Input = 65, Expected = "0xa934178370b82563fe2d9a90a61f680e1fdeb4a6" },
		new() { Input = 117, Expected = "0x169bee3bd993d8213e035b13c26d545ebefa3316" },
		new() { Input = 100, Expected = "0xb35a97cd753f23ae99ae1bb1d0d6fea5bd01b4d0" },
		new() { Input = 127, Expected = "0x51e2b6f22cfa20492fb5fad409209d78e1263b27" },
		new() { Input = 128, Expected = "0x693c44aee624f04c29f0302b859f18c936dd0ab4" },
		new() { Input = 129, Expected = "0xe4c520f9cb52076ac58c62c24553405fc080ed8f" },
		new() { Input = 178, Expected = "0x8a173499f67a3161298e8dbdd28619425c733461" },
		new() { Input = 199, Expected = "0x95dcb3eee502de3e925dfcf9f54c81ad55cc8a3e" },
		new() { Input = 200, Expected = "0xed3bf2f3da87df37ca1aa0d1453d275156f5b0a9" }
	};

	protected static readonly TestItem<int, string>[] DATA_BLAKE2B_256 = {
		// NOTE: Input denotes the number of bytes to take from DATA_BYTES when executing test
		new() { Input = 17, Expected = "0x2d27da7ef41e06ac1bc860203acdb4d5590f8e1cbd4ebde7173993319b96942f" },
		new() { Input = 31, Expected = "0x22016ccee234cf60a8edddd56be2601e0d9991e2c0809b4d6a7bd7e3e0e4b77e" },
		new() { Input = 32, Expected = "0x57f5cf50f21cf5b467d3ab35383dda675c669928cadea7e28ace0c99490ed1a0" },
		new() { Input = 33, Expected = "0x569019bd21af6737c4223259c95bf21c2b64a451995ad2823c287636c72bde51" },
		new() { Input = 34, Expected = "0x6b6a9e9ab4491f7a3223f99c4759158fcfc1ac3ceff9643e5357a5b6aba51cb6" },
		new() { Input = 63, Expected = "0xba36d6a19fd75a24b6408f1235d7d070cacd0ad06eeeddc6aa84d529d9d77d42" },
		new() { Input = 64, Expected = "0x1741b27f5c6a50c3cee00ce8e6e70c1362ae18665a57c9d5454e8b6266e84de5" },
		new() { Input = 65, Expected = "0xa741f85ac3e389e47b099554c1de21be49a2a1eb80691bc56fdf943fc5c4ef18" },
		new() { Input = 117, Expected = "0x3e50481ac95b202f5b03bbd5b5544acb0e1dcc9ef0c42e58cf2e31448ff5c45f" },
		new() { Input = 100, Expected = "0xeee72bedf3cb104e8b832ca03cd1b3bc640024f9398f658187cac64549d3e544" },
		new() { Input = 127, Expected = "0x7b10d0d1e7da93c0a8476e1369276a7f589641a762119a7807891fa404440e59" },
		new() { Input = 128, Expected = "0xa4b538b61e04f65302648fe3f432f21a225a038c010e66e8a079bb054fd362d2" },
		new() { Input = 129, Expected = "0x2c9c22d727eeec63c6b509849bf61526588a00f99687a5914a8152e767c1e6ac" },
		new() { Input = 178, Expected = "0xa74e225cf53d645319f7cc3af025788e227ce02eef8fbea8dbe11b645e5a11c6" },
		new() { Input = 199, Expected = "0x24c97ac78bc75efc9ac72af9e3415ab46e7e60ee49d9de8f40b995b2bcacb0d4" },
		new() { Input = 200, Expected = "0xb34964c00d9b467409239b3e190fcf16c9109b8084b42be6f4e1cf36136a7d57" }
	};

	protected static readonly TestItem<int, string>[] DATA_BLAKE2B_384 = {
		// NOTE: Input denotes the number of bytes to take from DATA_BYTES when executing test
		new() { Input = 17, Expected = "0x3a08da334a4b328d75f123417a05022cac73535f4aa4bea3f31d86f862250f37425281120c97d687a95bece75c28a0d2" },
		new() { Input = 31, Expected = "0x26ce23c0dcf7b0e5dc93f815a86f2cdcc7daee8a5a216937f4a8d32b5cf1072d4c9462f761084f5c1638b50b86ce11e7" },
		new() { Input = 32, Expected = "0x8f7993befdce4f8212b33dffdd4ea347eb6fd964a909e3b3431cc6b372da31d8eeae54ab04f6d9410f279d7bee217905" },
		new() { Input = 33, Expected = "0xe4e5fc14e25f7a2767ca428672a2e51ebbda7f578ebc0ee35d1d2cf553a65bcd8e638a638f27d77e125913feaacd95e7" },
		new() { Input = 34, Expected = "0x993ba14777ab02a9299f78351bf5a0f21765f58c3e33b918f7818fa1fd50a81fd344207e767290379fc300ac4a113732" },
		new() { Input = 63, Expected = "0x094c5aafc4ec999e561ae81a61c996fc2ac96994125f02997f7737822e6250a173fbc8f3843285745e9c6c0102c1ecdd" },
		new() { Input = 64, Expected = "0xca0a50ca7b449eabfc2b2a914fe7ef949e0c7fed344e05d4608f1ae09eb47584b837649875b2cf0e7994b2a2d27a2607" },
		new() { Input = 65, Expected = "0xdae08237f92b7209c3e1020383bee9aa59f63daaea278978068716313fde91aa590a74c670a8dce4f25511252e7f680b" },
		new() { Input = 117, Expected = "0xcba5beca96118dbfd3868a6e6ffacdced14546f6fb522028c1ce4e44131c1d690ce247da550578b05ce7825533fe919f" },
		new() { Input = 100, Expected = "0xe5ec9bccbc2d0f0f8cc2e096c0728ab87179846ca568611a1e546beccbab002ac22eb227965e64501417b8e9e8093f81" },
		new() { Input = 127, Expected = "0xdeb4c6603653dc82e1372aa5f7291e9ae298011a4fab1f1590ca7c638757b5683607901bb814a80317440f7ca895357b" },
		new() { Input = 128, Expected = "0x91aa66364949c079711591c2efce3d02a87ee366e6d3aabacd04760ea58b9aed3809800de69cf44c90aea96eb37ffc70" },
		new() { Input = 129, Expected = "0xf38dd8a035d9bfaf681ffe6350719734e35fc8d6ca087f801afdeb802a9eb6357ae3efecb67237e9105972436f1d01a6" },
		new() { Input = 178, Expected = "0x92146005b2c0d64af849f9a2f714f0febf1700b6d5ab8a87af46483b90b8fde9abbd09e578c96368af10e069257e2cdd" },
		new() { Input = 199, Expected = "0x5435eb9cf7541bf264ca320880abf31a6edda9bb9ecad48e70115a650cf10cccec0f70fbbbda0844bbb8c2707547eb5c" },
		new() { Input = 200, Expected = "0xf01dc700d658aff83165888cfba018d930f1b30063e1143e02ab7b39b1219af952a36b3de8161ee25cc0f9134b1e1ebe" }
	};

	protected readonly TestItem<int, string>[] DATA_BLAKE2B_512 = {
		// NOTE: Input denotes the number of bytes to take from DATA_BYTES when executing test
		new() { Input = 17, Expected = "0x8f8a1cf77aad3d0421db8ae7b2a4752b811059d3a3a5cc3b00454ecd918f39936e2f8e23c5a96c6f4519f76e73981da24d2f8c4d3ef4e7002a17eef80e2a9514" },
		new() { Input = 31, Expected = "0x4e074ec035707651726210950e241346aec8f6c6aaa504f416cd0ec92fa4c08340cca3827fb990d74b8f837c0bbafccb2d5739f2b59ff49cce5cfa4f285e083f" },
		new() { Input = 32, Expected = "0xcb2c167bbad4d529cbdc48645756cf61b3838d6c0af14a9596dd105a172053e198c22c3669a792949274ff1ed687e80e4ae3b85ec70154a6f62d2cf13231b083" },
		new() { Input = 33, Expected = "0x9a8a4ea7bbaf058c07a62a9f13de219abb2bd99738a7997bfaa373d61ce54c6a0ede112cb652d40682ff804552f9db4247de5858c45ccb9a8ac064881f05b92c" },
		new() { Input = 34, Expected = "0xa7651109edfa702d76471ad0c4ffaaed200f5ed783a4ad834ced1b37bf4038af8472d767a7b0d08e146e079c4467468df30d89f14ae59fc75ecf927717abecdc" },
		new() { Input = 63, Expected = "0xe24f626b1a12d956231a7bf17d7f976925cc186776da91543eb9b244454bc0b71956bce4e514bf1095fc61097eb39d67dc78ec6c78e640bcfb18fd110adaecfd" },
		new() { Input = 64, Expected = "0x55de09270df2b8f2b8c35f082ae45acd55fca556fb4c7614a61531888e7d5502a2015b0c936fbddf4f6ccfcdba4d4e69139be2062c42a6b1acc03638b035d55e" },
		new() { Input = 65, Expected = "0x51a424024d3eb88e2cf09e14e512a6ce27b1a95a087afe07c5138e191cbf8079fd740a262e47e6dffad44355548eebd2c1ebc24c8b7bbf266573b838a6b70ef8" },
		new() { Input = 100, Expected = "0x03cab91f85e1ffc286a297538200b80b39681f5fe06108557c354264127db6aaa271399af25c2cb240554921b3d878675f875dd244a7af22187015945b105558" },
		new() { Input = 117, Expected = "0xe9ca855bf340229a4446f46cb0b0e3cffaf1942b8a8b6e296d5b35621be9e6c40217a76d1461380d062e9f0ac8cee8e15b70b7762a6de367463ac84c4d56b49a" },
		new() { Input = 127, Expected = "0xab24840d31c5c19a8c5c0729e8bc327cb1b48088b135de8f04428985a0ef71d366388973625cb77d558f6dc4dcbe93c5d5327aedb83b0cbee34e656fde2962ee" },
		new() { Input = 128, Expected = "0xef4618e9126f6c54931e8f2ab5e12737ed4722932e107d05768ba59f484e0858b6b189ce0b1db3e18eea5355eb60dec5826be26cac759b7f2eab3a97ec111f10" },
		new() { Input = 129, Expected = "0xad7f01517787bfa75ceceab92d96f94f04600786a83cabe190e3b503af1d184d9db27577bdddb78fa052d8a086147add8ecc385b3f26c37180408311664bf9af" },
		new() { Input = 178, Expected = "0x99ae6a63885847e5b45ff4d2d2b0eb43e9fd722a0c7254eb4bcf706a484df9e300c61e6aa7c6620ddf2dabcc9b51257715f396f713606dbcd09f14c833becdb6" },
		new() { Input = 199, Expected = "0x33074a6aa23c6117037b426d16211bc41a29e38bf94bba4c2dce6659b0c4e5b63555a8b08a214905e1f795282a0a427cb90de7d3967d7ba975b58a7eb550eb3c" },
		new() { Input = 200, Expected = "0x6c5117105a9cf47347e5e59aeeacf833e503c3e537e75020c9363cdebafeab00dd478e96c3a0e11e4c2615284fddf47a079c2b49d650f0bbc167ba10f5bf25e8" }
	};

	protected static readonly TestItem<int, string>[] DATA_BLAKE2S_128 = {
		// NOTE: Input denotes the number of bytes to take from DATA_BYTES when executing test
		new() { Input = 17, Expected = "0x772747b25c0b178250c2c25f34ba4180" },
		new() { Input = 31, Expected = "0x8a5f58395b7234260e78f0a037521516" },
		new() { Input = 32, Expected = "0x6376fa4f07891623a403467aea22a31b" },
		new() { Input = 33, Expected = "0x88b4a975fc6641bb468add2147a43fb4" },
		new() { Input = 34, Expected = "0xe85a4e2b095e2a197a1967c103260ff4" },
		new() { Input = 63, Expected = "0x22cb94bbcdcc849f43fd1c0961333b9e" },
		new() { Input = 64, Expected = "0xd5ef1298f40486c99be7d9d65e153a23" },
		new() { Input = 65, Expected = "0x88968a23d93f07fafeccc8fea9b91ed9" },
		new() { Input = 117, Expected = "0xd2d25839c72fe40b76f93c6c5918685b" },
		new() { Input = 100, Expected = "0x55175439e7ab7478450e0b8c823e73ac" },
		new() { Input = 127, Expected = "0xa6ba2f65874f99865a0a2e00eff25b57" },
		new() { Input = 128, Expected = "0xe16f48c3d7d0c931e26de2f507e8ee82" },
		new() { Input = 129, Expected = "0xeb1d515fcb44ddd0b7527fd21ec63be6" },
		new() { Input = 178, Expected = "0x1dfce172b5c4cf063a415d63513a7025" },
		new() { Input = 199, Expected = "0xe8c9db1d1b769eece915b9ddc7b0b68a" },
		new() { Input = 200, Expected = "0xb0af3e10726f90cd07b71fc103f2b556" }
	};

	protected static readonly TestItem<int, string>[] DATA_BLAKE2S_160 = {
		// NOTE: Input denotes the number of bytes to take from DATA_BYTES when executing test
		new() { Input = 17, Expected = "0xf6ba42985394bab8bd51ce760a39b3a298bd3a11" },
		new() { Input = 31, Expected = "0xb90703b03ef5f531535633ea1151c82cf9660353" },
		new() { Input = 32, Expected = "0x88dc5f24a3c4f34ec08a00565a9209507209df78" },
		new() { Input = 33, Expected = "0x27c495c0e3aa90bfa7aaf20d27afc275560d9f9f" },
		new() { Input = 34, Expected = "0xe67f177a3a5e1b7b1cd15dd8a00da63d6b344b33" },
		new() { Input = 63, Expected = "0xb162319a5bcb2733409d97375e18ce059771f2be" },
		new() { Input = 64, Expected = "0x17a263a124a1740011d0639832c6125f16d0687e" },
		new() { Input = 65, Expected = "0x75819b8ecd75e87ade0e349d55ce59ddb91847b3" },
		new() { Input = 117, Expected = "0x31ae1311494728fff3c5e7f1a405b4cbd0657b1f" },
		new() { Input = 100, Expected = "0xf36c72419513a7cfec06a00c12beeace98364f40" },
		new() { Input = 127, Expected = "0x5a5b2da33b993b2aec06afa2bbed3950efacc317" },
		new() { Input = 128, Expected = "0x6e20a41da31f44b34456ce482a97845d9ff3bc95" },
		new() { Input = 129, Expected = "0xe2e3decb7017b4814507176f011cd4d5048a3ec6" },
		new() { Input = 178, Expected = "0xfc822079244ce74fa6b2e2e23e7ca83ba058da46" },
		new() { Input = 199, Expected = "0x601a8bb6597717cf97abec19230713424504f160" },
		new() { Input = 200, Expected = "0xc3f2b5858a230ac4876733cce7b9211539c0c1ab" }
	};

	protected static readonly TestItem<int, string>[] DATA_BLAKE2S_224 = {
		// NOTE: Input denotes the number of bytes to take from DATA_BYTES when executing test
		new() { Input = 17, Expected = "0x234e1c6a3df37a27d74cff1df04ce21a22366937fa2063ea1e750da1" },
		new() { Input = 31, Expected = "0xfd958963a6cff7aa4a7306a3d896333b32c65039dacb79b3aac58d0c" },
		new() { Input = 32, Expected = "0xf72aed3ea7c81e26769c9b130db582b81cc5e59a19df551541d93f09" },
		new() { Input = 33, Expected = "0x761642484e4db018890f9560885142137c7d51ad5c05797d60c4dd04" },
		new() { Input = 34, Expected = "0x74f3a6c0d6cc0a618ed081a5804e5d4b5c23f980ab24595ec07b404d" },
		new() { Input = 63, Expected = "0x69c95720ff5aa3de8ae0bad02ec493661de32890159b443e87de34ea" },
		new() { Input = 64, Expected = "0xa7caa1321d083fc62c4f5505c272015a087e44d412698d62f6d23ad1" },
		new() { Input = 65, Expected = "0x950077d6f8630b82900c9843a72da552e95db0da1d8718ca64b7d462" },
		new() { Input = 117, Expected = "0x822ed98f12194c0857940c85641864f22867ca2b2b110c6167d8c916" },
		new() { Input = 100, Expected = "0x0e71c342a698f9af3c9584c5c3522245013883788367debdf864ad0c" },
		new() { Input = 127, Expected = "0x19e9915753c42292e38a258b40f68f72955ffcf9ac335ae8208980d1" },
		new() { Input = 128, Expected = "0x27c72565492e38e1fb96ff646e2f8ae0710d5ee633984272bd70053b" },
		new() { Input = 129, Expected = "0x1d01481a6f01ae00b9cfa7a7db627e2145140024d651d510d8a75e7e" },
		new() { Input = 178, Expected = "0xd5018b2b23da0a7ea6b742a78a81fa84ec3a3a3a751a3619c9300251" },
		new() { Input = 199, Expected = "0x2fe0ce44cc1ae3608606d5b38f8d5ca0471c792855220b5a70fbf2f7" },
		new() { Input = 200, Expected = "0x1b8211ad5a170d6e2fb831b0355b461d85ee62155cd1796d439f627f" }
	};


	protected readonly TestItem<int, string>[] DATA_BLAKE2S_256 = {
		// NOTE: Input denotes the number of bytes to take from DATA_BYTES when executing test
		new() { Input = 17, Expected = "0xc6d5f10d213cfa97b3317f115f6eae29419051524f14f29b39c4f620a6e4758d" },
		new() { Input = 31, Expected = "0x2c82e8af7b3db4a4737546616f34026c0acdf0c2037ba138861af29e34b2eaff" },
		new() { Input = 32, Expected = "0xc661e40d5ef223343c2513b19b0ba5a69c91e076be875c854830345de2741517" },
		new() { Input = 33, Expected = "0xfae74bb4a48f325c4380ab694ed91ed6b0bb5d8eac825ae8ade73d4b7d7d1cb7" },
		new() { Input = 34, Expected = "0xd6010f74459a82f459604a044fb2d21d93904427c44ebb22bd76694110fbf9df" },
		new() { Input = 63, Expected = "0x0707f52c9629e5d926d19aaac0e31f96273627ddfbb85519f4d2abdda8107459" },
		new() { Input = 64, Expected = "0xc55f4dc5612258bd600c4b078128919dca82a4f98022b9762826d596356dda14" },
		new() { Input = 65, Expected = "0x7ce4f4e9e7357f74f15903f273a285e02d7fa976e94ae900d9a14b131f397aec" },
		new() { Input = 100, Expected = "0x4be6010f72c375b685dd57d66585b8c5f86eb1ac27b80ca20f041d44533a7005" },
		new() { Input = 117, Expected = "0xa37bc13f537b8800fc61170dd714cb938c0e62047a7c9d0061bd8a407fe29a13" },
		new() { Input = 127, Expected = "0xef42bf26aeae6d85c8c1a0d4304da676444a7c57944efc0496c300b391048b01" },
		new() { Input = 128, Expected = "0x54afb0c19b2fc2ed628d379f819a79ad940add19296099acabe26bdc67c9bd05" },
		new() { Input = 129, Expected = "0x31e1c3e9ce27f992329d933a02dafe206b856f90057803d1e537304e97f80885" },
		new() { Input = 178, Expected = "0xf95e620f0335c83afa8eda36b853a739158cd4f8910fa2aa30d0794352c65510" },
		new() { Input = 199, Expected = "0xbd881d0cac02bd2300d41dfd8936570ed940d8cef9632731f28ea472d43c4199" },
		new() { Input = 200, Expected = "0xc83b8ea4503d8a8d470c0ba7f977c2ea773e844d36d9a9e866a953c1338259ee" }
	};

	protected static readonly TestItem<int, string>[] DATA_TIGER_3_128 = {
		// NOTE: Input denotes the number of bytes to take from DATA_BYTES when executing test
		new() { Input = 17, Expected = "0xbd564f151427e8377f9065fdcda99861" },
		new() { Input = 31, Expected = "0x21440858538e69caaf978163edacb045" },
		new() { Input = 32, Expected = "0x39046c6e2891f723547a5bbf2d0e114a" },
		new() { Input = 33, Expected = "0x966b716020a4b06b6359da36c8c611fd" },
		new() { Input = 34, Expected = "0x7362b78770ee021ad562f003fcbb7558" },
		new() { Input = 63, Expected = "0x97c9942645a6bada29dbcd0a6cf0eab1" },
		new() { Input = 64, Expected = "0x47aac7a78a9f808e45e499d6a9d17b2c" },
		new() { Input = 65, Expected = "0x984c30cd9df19bd9fa76429a59a32983" },
		new() { Input = 117, Expected = "0x785090a0e9a16a7c79a05be1675a02b5" },
		new() { Input = 100, Expected = "0x782a571f065b3ad553699dd09634c4a3" },
		new() { Input = 127, Expected = "0xecb4f404eefc8a4ba2d70a412a7f55ac" },
		new() { Input = 128, Expected = "0x0fd6b2ed965ba12c64944e96c383356b" },
		new() { Input = 129, Expected = "0x717767568da81f68406c53867f288a93" },
		new() { Input = 178, Expected = "0x772d7f6e54cdb791041d35bad1ae8bd7" },
		new() { Input = 199, Expected = "0x2f7642269536cc6418f303a41b2db98e" },
		new() { Input = 200, Expected = "0x0274d4de9b94787fd4d7d350ecc8551b" }
	};

	protected static readonly TestItem<int, string>[] DATA_TIGER_3_160 = {
		// NOTE: Input denotes the number of bytes to take from DATA_BYTES when executing test
		new() { Input = 17, Expected = "0xbd564f151427e8377f9065fdcda99861404a29c8" },
		new() { Input = 31, Expected = "0x21440858538e69caaf978163edacb0453de24844" },
		new() { Input = 32, Expected = "0x39046c6e2891f723547a5bbf2d0e114a34770e30" },
		new() { Input = 33, Expected = "0x966b716020a4b06b6359da36c8c611fdb9f32110" },
		new() { Input = 34, Expected = "0x7362b78770ee021ad562f003fcbb755885cd6e58" },
		new() { Input = 63, Expected = "0x97c9942645a6bada29dbcd0a6cf0eab16fc8fa0e" },
		new() { Input = 64, Expected = "0x47aac7a78a9f808e45e499d6a9d17b2ca5fc6544" },
		new() { Input = 65, Expected = "0x984c30cd9df19bd9fa76429a59a329833d4fc487" },
		new() { Input = 117, Expected = "0x785090a0e9a16a7c79a05be1675a02b5359dde15" },
		new() { Input = 100, Expected = "0x782a571f065b3ad553699dd09634c4a37c289c20" },
		new() { Input = 127, Expected = "0xecb4f404eefc8a4ba2d70a412a7f55ac795beb03" },
		new() { Input = 128, Expected = "0x0fd6b2ed965ba12c64944e96c383356be4f76194" },
		new() { Input = 129, Expected = "0x717767568da81f68406c53867f288a9370e8819a" },
		new() { Input = 178, Expected = "0x772d7f6e54cdb791041d35bad1ae8bd75d51c87c" },
		new() { Input = 199, Expected = "0x2f7642269536cc6418f303a41b2db98e3e37a479" },
		new() { Input = 200, Expected = "0x0274d4de9b94787fd4d7d350ecc8551b5a2dbdd9" }
	};


	protected static readonly TestItem<int, string>[] DATA_TIGER_3_192 = {
		// NOTE: Input denotes the number of bytes to take from DATA_BYTES when executing test
		new() { Input = 17, Expected = "0xbd564f151427e8377f9065fdcda99861404a29c8b7866940" },
		new() { Input = 31, Expected = "0x21440858538e69caaf978163edacb0453de24844c6f65f46" },
		new() { Input = 32, Expected = "0x39046c6e2891f723547a5bbf2d0e114a34770e3024047a84" },
		new() { Input = 33, Expected = "0x966b716020a4b06b6359da36c8c611fdb9f32110c9781d1c" },
		new() { Input = 34, Expected = "0x7362b78770ee021ad562f003fcbb755885cd6e586d4ab383" },
		new() { Input = 63, Expected = "0x97c9942645a6bada29dbcd0a6cf0eab16fc8fa0ef33bb651" },
		new() { Input = 64, Expected = "0x47aac7a78a9f808e45e499d6a9d17b2ca5fc654468a5e2b3" },
		new() { Input = 65, Expected = "0x984c30cd9df19bd9fa76429a59a329833d4fc487f6436b0b" },
		new() { Input = 117, Expected = "0x785090a0e9a16a7c79a05be1675a02b5359dde15233ced5f" },
		new() { Input = 100, Expected = "0x782a571f065b3ad553699dd09634c4a37c289c20ae075fa8" },
		new() { Input = 127, Expected = "0xecb4f404eefc8a4ba2d70a412a7f55ac795beb03c764b7c4" },
		new() { Input = 128, Expected = "0x0fd6b2ed965ba12c64944e96c383356be4f7619431a59a77" },
		new() { Input = 129, Expected = "0x717767568da81f68406c53867f288a9370e8819a7c390de4" },
		new() { Input = 178, Expected = "0x772d7f6e54cdb791041d35bad1ae8bd75d51c87cd7094695" },
		new() { Input = 199, Expected = "0x2f7642269536cc6418f303a41b2db98e3e37a479f45f8176" },
		new() { Input = 200, Expected = "0x0274d4de9b94787fd4d7d350ecc8551b5a2dbdd9e5ed13ce" }
	};

	protected static readonly TestItem<int, string>[] DATA_TIGER_4_128 = {
		// NOTE: Input denotes the number of bytes to take from DATA_BYTES when executing test
		new() { Input = 17, Expected = "0x89dd708bf4c224634e4a7f72a3d04fb0" },
		new() { Input = 31, Expected = "0xdc079dab739165a4caf8ab9e11d7f76c" },
		new() { Input = 32, Expected = "0xcc84e85beac7cf301fbf9b7f9ad498c6" },
		new() { Input = 33, Expected = "0x317048229fdb6bc6bb38e3c94e10fbcf" },
		new() { Input = 34, Expected = "0xc2157572f48736eab9095bc2549c4ecf" },
		new() { Input = 63, Expected = "0x3c930cda98a84c66eaeb7eaa4f778a0f" },
		new() { Input = 64, Expected = "0x131c4cc96ffcb8e860c01473e0349cc8" },
		new() { Input = 65, Expected = "0x2fb969ae2d36061d514d0f9fc72649e4" },
		new() { Input = 117, Expected = "0x13dd0222acafbf6ded56f355fe80fbed" },
		new() { Input = 100, Expected = "0x856a967796045e1ce28d1b3456f2f118" },
		new() { Input = 127, Expected = "0x0db0247178c24838f03272978efaf875" },
		new() { Input = 128, Expected = "0x55565869f15d4806533e4bd2f2251fc5" },
		new() { Input = 129, Expected = "0x834f7aa5d8ac5c91dc95e40ff78d4212" },
		new() { Input = 178, Expected = "0xf7f4737536721e49c488b4bb837abc82" },
		new() { Input = 199, Expected = "0x39b4c65c3a4338e29a5a159d66881bdf" },
		new() { Input = 200, Expected = "0x7e1d42ab76af98cceec74c3ff11953f5" }
	};


	protected static readonly TestItem<int, string>[] DATA_TIGER_4_160 = {
		// NOTE: Input denotes the number of bytes to take from DATA_BYTES when executing test
		new() { Input = 17, Expected = "0x89dd708bf4c224634e4a7f72a3d04fb05b7c2568" },
		new() { Input = 31, Expected = "0xdc079dab739165a4caf8ab9e11d7f76c223dc0d8" },
		new() { Input = 32, Expected = "0xcc84e85beac7cf301fbf9b7f9ad498c68e4e3e03" },
		new() { Input = 33, Expected = "0x317048229fdb6bc6bb38e3c94e10fbcf8bd0fcf7" },
		new() { Input = 34, Expected = "0xc2157572f48736eab9095bc2549c4ecf87e84692" },
		new() { Input = 63, Expected = "0x3c930cda98a84c66eaeb7eaa4f778a0f0e818cd7" },
		new() { Input = 64, Expected = "0x131c4cc96ffcb8e860c01473e0349cc83e70630a" },
		new() { Input = 65, Expected = "0x2fb969ae2d36061d514d0f9fc72649e499a2809c" },
		new() { Input = 117, Expected = "0x13dd0222acafbf6ded56f355fe80fbed35d52098" },
		new() { Input = 100, Expected = "0x856a967796045e1ce28d1b3456f2f1188cde6164" },
		new() { Input = 127, Expected = "0x0db0247178c24838f03272978efaf87552a0c334" },
		new() { Input = 128, Expected = "0x55565869f15d4806533e4bd2f2251fc539f52067" },
		new() { Input = 129, Expected = "0x834f7aa5d8ac5c91dc95e40ff78d42122d0f5229" },
		new() { Input = 178, Expected = "0xf7f4737536721e49c488b4bb837abc82c0d93072" },
		new() { Input = 199, Expected = "0x39b4c65c3a4338e29a5a159d66881bdfc31cd9d4" },
		new() { Input = 200, Expected = "0x7e1d42ab76af98cceec74c3ff11953f550652fb0" }
	};


	protected static readonly TestItem<int, string>[] DATA_TIGER_4_192 = {
		// NOTE: Input denotes the number of bytes to take from DATA_BYTES when executing test
		new() { Input = 17, Expected = "0x89dd708bf4c224634e4a7f72a3d04fb05b7c25684e8985a7" },
		new() { Input = 31, Expected = "0xdc079dab739165a4caf8ab9e11d7f76c223dc0d8a5f5e262" },
		new() { Input = 32, Expected = "0xcc84e85beac7cf301fbf9b7f9ad498c68e4e3e0306fec93f" },
		new() { Input = 33, Expected = "0x317048229fdb6bc6bb38e3c94e10fbcf8bd0fcf78efc44a9" },
		new() { Input = 34, Expected = "0xc2157572f48736eab9095bc2549c4ecf87e84692bb9aaf03" },
		new() { Input = 63, Expected = "0x3c930cda98a84c66eaeb7eaa4f778a0f0e818cd7db309afa" },
		new() { Input = 64, Expected = "0x131c4cc96ffcb8e860c01473e0349cc83e70630a43df4a30" },
		new() { Input = 65, Expected = "0x2fb969ae2d36061d514d0f9fc72649e499a2809c115fc1b3" },
		new() { Input = 117, Expected = "0x13dd0222acafbf6ded56f355fe80fbed35d52098cfe2558e" },
		new() { Input = 100, Expected = "0x856a967796045e1ce28d1b3456f2f1188cde61645647f0d5" },
		new() { Input = 127, Expected = "0x0db0247178c24838f03272978efaf87552a0c334c17eea01" },
		new() { Input = 128, Expected = "0x55565869f15d4806533e4bd2f2251fc539f5206772ea441a" },
		new() { Input = 129, Expected = "0x834f7aa5d8ac5c91dc95e40ff78d42122d0f5229f1a787e2" },
		new() { Input = 178, Expected = "0xf7f4737536721e49c488b4bb837abc82c0d930723e5d981e" },
		new() { Input = 199, Expected = "0x39b4c65c3a4338e29a5a159d66881bdfc31cd9d42dbb1d88" },
		new() { Input = 200, Expected = "0x7e1d42ab76af98cceec74c3ff11953f550652fb097e5355c" }
	};

	protected static readonly TestItem<int, string>[] DATA_TIGER_5_128 = {
		// NOTE: Input denotes the number of bytes to take from DATA_BYTES when executing test
		new() { Input = 17, Expected = "0xead95fb917754d70e6ad081b4802477d" },
		new() { Input = 31, Expected = "0x7a9251c37ab59076647ead0a4fd72487" },
		new() { Input = 32, Expected = "0xa4ed370415226b714761f69dbb3b1c32" },
		new() { Input = 33, Expected = "0xc8fdb6752d5ea086f561c1812707ce12" },
		new() { Input = 34, Expected = "0x0c4a7a3cb849fcffdf07cee11925e03a" },
		new() { Input = 63, Expected = "0x7ac1d39986e15d22ea188bd0250e7443" },
		new() { Input = 64, Expected = "0xfeb688b42fc0f91731da3f5aa2c0652c" },
		new() { Input = 65, Expected = "0x3af3ab878d66b31b8805362d3ff05182" },
		new() { Input = 117, Expected = "0x794e8635105d8e8a1074faca1f384566" },
		new() { Input = 100, Expected = "0xf4cd888c82b7fe90c176f05ca805f560" },
		new() { Input = 127, Expected = "0x08301d132f09cb1eaadce79c579ca682" },
		new() { Input = 128, Expected = "0xa27992166ac827beb1f13960e8f8c6e5" },
		new() { Input = 129, Expected = "0x0fde0fc4da784bf268672a9d94eaa610" },
		new() { Input = 178, Expected = "0xb9e4e14555c5bd887db0e9183c336200" },
		new() { Input = 199, Expected = "0xf9c2bffc2cffb1b742bda3e11b77f140" },
		new() { Input = 200, Expected = "0x3a4734eeef2faedcba1e726360a6e706" }
	};


	protected static readonly TestItem<int, string>[] DATA_TIGER_5_160 = {
		// NOTE: Input denotes the number of bytes to take from DATA_BYTES when executing test
		new() { Input = 17, Expected = "0xead95fb917754d70e6ad081b4802477d8af8a949" },
		new() { Input = 31, Expected = "0x7a9251c37ab59076647ead0a4fd724877fd6c4c2" },
		new() { Input = 32, Expected = "0xa4ed370415226b714761f69dbb3b1c32c11c5ef1" },
		new() { Input = 33, Expected = "0xc8fdb6752d5ea086f561c1812707ce127b193d3f" },
		new() { Input = 34, Expected = "0x0c4a7a3cb849fcffdf07cee11925e03aea3db15a" },
		new() { Input = 63, Expected = "0x7ac1d39986e15d22ea188bd0250e744386921d33" },
		new() { Input = 64, Expected = "0xfeb688b42fc0f91731da3f5aa2c0652cc36bb2b4" },
		new() { Input = 65, Expected = "0x3af3ab878d66b31b8805362d3ff05182ef8e9eb2" },
		new() { Input = 117, Expected = "0x794e8635105d8e8a1074faca1f3845666b774540" },
		new() { Input = 100, Expected = "0xf4cd888c82b7fe90c176f05ca805f560d6e0a956" },
		new() { Input = 127, Expected = "0x08301d132f09cb1eaadce79c579ca68250091237" },
		new() { Input = 128, Expected = "0xa27992166ac827beb1f13960e8f8c6e5d854591d" },
		new() { Input = 129, Expected = "0x0fde0fc4da784bf268672a9d94eaa61044f5abe2" },
		new() { Input = 178, Expected = "0xb9e4e14555c5bd887db0e9183c336200fe749eff" },
		new() { Input = 199, Expected = "0xf9c2bffc2cffb1b742bda3e11b77f140a3b685d7" },
		new() { Input = 200, Expected = "0x3a4734eeef2faedcba1e726360a6e706ee658012" }
	};


	protected static readonly TestItem<int, string>[] DATA_TIGER_5_192 = {
		// NOTE: Input denotes the number of bytes to take from DATA_BYTES when executing test
		new() { Input = 17, Expected = "0xead95fb917754d70e6ad081b4802477d8af8a949b5e119ea" },
		new() { Input = 31, Expected = "0x7a9251c37ab59076647ead0a4fd724877fd6c4c2e0ef4a25" },
		new() { Input = 32, Expected = "0xa4ed370415226b714761f69dbb3b1c32c11c5ef166b1b745" },
		new() { Input = 33, Expected = "0xc8fdb6752d5ea086f561c1812707ce127b193d3f39c8a885" },
		new() { Input = 34, Expected = "0x0c4a7a3cb849fcffdf07cee11925e03aea3db15a9ff25896" },
		new() { Input = 63, Expected = "0x7ac1d39986e15d22ea188bd0250e744386921d33b6c1a285" },
		new() { Input = 64, Expected = "0xfeb688b42fc0f91731da3f5aa2c0652cc36bb2b4e69de154" },
		new() { Input = 65, Expected = "0x3af3ab878d66b31b8805362d3ff05182ef8e9eb298394fb1" },
		new() { Input = 117, Expected = "0x794e8635105d8e8a1074faca1f3845666b7745402769fad9" },
		new() { Input = 100, Expected = "0xf4cd888c82b7fe90c176f05ca805f560d6e0a956e20814a6" },
		new() { Input = 127, Expected = "0x08301d132f09cb1eaadce79c579ca68250091237e9767384" },
		new() { Input = 128, Expected = "0xa27992166ac827beb1f13960e8f8c6e5d854591dcf0a8ef3" },
		new() { Input = 129, Expected = "0x0fde0fc4da784bf268672a9d94eaa61044f5abe29c1cdd3c" },
		new() { Input = 178, Expected = "0xb9e4e14555c5bd887db0e9183c336200fe749eff4471b6f7" },
		new() { Input = 199, Expected = "0xf9c2bffc2cffb1b742bda3e11b77f140a3b685d77c8120c2" },
		new() { Input = 200, Expected = "0x3a4734eeef2faedcba1e726360a6e706ee6580124bf8ed26" }
	};

	protected readonly TestItem<int, string>[] DATA_TIGER2_3_128 = {
		// NOTE: Input denotes the number of bytes to take from DATA_BYTES when executing test
		new() { Input = 17, Expected = "0x194e85c692547aedf4f3ea99f789e0dc" },
		new() { Input = 31, Expected = "0xd7818bb5ec794ea95623b8a0e756b276" },
		new() { Input = 32, Expected = "0x6406da069393867ab46916e9a5863c45" },
		new() { Input = 33, Expected = "0xca710c7054a81bd33a3a4ee28b74b528" },
		new() { Input = 34, Expected = "0x78040c5a25d4d620f68f47528708adda" },
		new() { Input = 63, Expected = "0x69cdffa520350ca8fa856a03799bc74d" },
		new() { Input = 64, Expected = "0x4aaa17f4ed086ea108a74922b1a136ac" },
		new() { Input = 65, Expected = "0xf3780b12da9f5e69c720c4be5558481f" },
		new() { Input = 117, Expected = "0x3d8970a9f4030895057b6d28aaf13f59" },
		new() { Input = 100, Expected = "0x47b8fbe40794378ce5641aaaefb8f74d" },
		new() { Input = 127, Expected = "0xd699f861340ff17c8acbaf682597366e" },
		new() { Input = 128, Expected = "0x176524e67b5919fd609ac749be84bace" },
		new() { Input = 129, Expected = "0x97f7db1c52ff20d7ff43248460a211ca" },
		new() { Input = 178, Expected = "0x6aa13076eef9215ce43631502f65a5f5" },
		new() { Input = 199, Expected = "0x9111ec922a45107511588a78edc585e7" },
		new() { Input = 200, Expected = "0x687f368248f5cd5ad9c0a412877b92be" }
	};


	protected readonly TestItem<int, string>[] DATA_TIGER2_3_160 = {
		// NOTE: Input denotes the number of bytes to take from DATA_BYTES when executing test
		new() { Input = 17, Expected = "0x194e85c692547aedf4f3ea99f789e0dc394f15d5" },
		new() { Input = 31, Expected = "0xd7818bb5ec794ea95623b8a0e756b2769ce83ec2" },
		new() { Input = 32, Expected = "0x6406da069393867ab46916e9a5863c45f6d91abf" },
		new() { Input = 33, Expected = "0xca710c7054a81bd33a3a4ee28b74b5284a81a9ac" },
		new() { Input = 34, Expected = "0x78040c5a25d4d620f68f47528708addad9f44b2f" },
		new() { Input = 63, Expected = "0x69cdffa520350ca8fa856a03799bc74df3a05a25" },
		new() { Input = 64, Expected = "0x4aaa17f4ed086ea108a74922b1a136acc61236d9" },
		new() { Input = 65, Expected = "0xf3780b12da9f5e69c720c4be5558481f8ecfe1b2" },
		new() { Input = 117, Expected = "0x3d8970a9f4030895057b6d28aaf13f5945a7427b" },
		new() { Input = 100, Expected = "0x47b8fbe40794378ce5641aaaefb8f74d8d6b17ea" },
		new() { Input = 127, Expected = "0xd699f861340ff17c8acbaf682597366e0b51e1bd" },
		new() { Input = 128, Expected = "0x176524e67b5919fd609ac749be84bace82c5ca17" },
		new() { Input = 129, Expected = "0x97f7db1c52ff20d7ff43248460a211ca98a231ad" },
		new() { Input = 178, Expected = "0x6aa13076eef9215ce43631502f65a5f5019d9149" },
		new() { Input = 199, Expected = "0x9111ec922a45107511588a78edc585e7f5516717" },
		new() { Input = 200, Expected = "0x687f368248f5cd5ad9c0a412877b92bebc6b8a9c" }
	};


	protected readonly TestItem<int, string>[] DATA_TIGER2_3_192 = {
		// NOTE: Input denotes the number of bytes to take from DATA_BYTES when executing test
		new() { Input = 17, Expected = "0x194e85c692547aedf4f3ea99f789e0dc394f15d52dbf0cac" },
		new() { Input = 31, Expected = "0xd7818bb5ec794ea95623b8a0e756b2769ce83ec2f78733cb" },
		new() { Input = 32, Expected = "0x6406da069393867ab46916e9a5863c45f6d91abf2adf2ebf" },
		new() { Input = 33, Expected = "0xca710c7054a81bd33a3a4ee28b74b5284a81a9aca7622586" },
		new() { Input = 34, Expected = "0x78040c5a25d4d620f68f47528708addad9f44b2f38f0e469" },
		new() { Input = 63, Expected = "0x69cdffa520350ca8fa856a03799bc74df3a05a25cbbb21af" },
		new() { Input = 64, Expected = "0x4aaa17f4ed086ea108a74922b1a136acc61236d9083d6935" },
		new() { Input = 65, Expected = "0xf3780b12da9f5e69c720c4be5558481f8ecfe1b27d7d89f9" },
		new() { Input = 117, Expected = "0x3d8970a9f4030895057b6d28aaf13f5945a7427b7ba31dbe" },
		new() { Input = 100, Expected = "0x47b8fbe40794378ce5641aaaefb8f74d8d6b17eae94f9dea" },
		new() { Input = 127, Expected = "0xd699f861340ff17c8acbaf682597366e0b51e1bd80d9020c" },
		new() { Input = 128, Expected = "0x176524e67b5919fd609ac749be84bace82c5ca177edb7eac" },
		new() { Input = 129, Expected = "0x97f7db1c52ff20d7ff43248460a211ca98a231adf111d9a2" },
		new() { Input = 178, Expected = "0x6aa13076eef9215ce43631502f65a5f5019d914995c51428" },
		new() { Input = 199, Expected = "0x9111ec922a45107511588a78edc585e7f5516717313539e4" },
		new() { Input = 200, Expected = "0x687f368248f5cd5ad9c0a412877b92bebc6b8a9c772cba93" }
	};

	protected readonly TestItem<int, string>[] DATA_TIGER2_4_128 = {
		// NOTE: Input denotes the number of bytes to take from DATA_BYTES when executing test
		new() { Input = 17, Expected = "0x585ed93d3356ad51eaa81fb9f593fa60" },
		new() { Input = 31, Expected = "0x8e6c6ca98cc24271944bbeb901fa9a2c" },
		new() { Input = 32, Expected = "0x741a642e6d04d7d305a80853ac0bb6b5" },
		new() { Input = 33, Expected = "0x3f1a5b2219f9fbb4709e3274aa739a17" },
		new() { Input = 34, Expected = "0xbe6b53f8c9f3778c7fc37f3713a4603e" },
		new() { Input = 63, Expected = "0x88f2e1b9d26d830b155b20c8ec565b96" },
		new() { Input = 64, Expected = "0x0aefe4707e1453b3db2dbef66735e74a" },
		new() { Input = 65, Expected = "0x38f60e320b275e40c6e4e92c42aa0d16" },
		new() { Input = 117, Expected = "0xc77b3db4100910fe03768313a13ab54e" },
		new() { Input = 100, Expected = "0x32788806b1e1176ccf3dd1a47265c038" },
		new() { Input = 127, Expected = "0x66588aa446630c43281fa7eaa662c2e2" },
		new() { Input = 128, Expected = "0x7b81b41483b630070180a275f352cdf4" },
		new() { Input = 129, Expected = "0x5c50e0745fcf53dc512f7dc56c815224" },
		new() { Input = 178, Expected = "0xeeb062c6c9f87c91fe4865a5b73c8172" },
		new() { Input = 199, Expected = "0x668679ce5ac3d88a72a5bdb18ca33d44" },
		new() { Input = 200, Expected = "0x127130828d6278ac9e47b6c20babd0b4" }
	};


	protected readonly TestItem<int, string>[] DATA_TIGER2_4_160 = {
		// NOTE: Input denotes the number of bytes to take from DATA_BYTES when executing test
		new() { Input = 17, Expected = "0x585ed93d3356ad51eaa81fb9f593fa60a6484208" },
		new() { Input = 31, Expected = "0x8e6c6ca98cc24271944bbeb901fa9a2ceb7ab361" },
		new() { Input = 32, Expected = "0x741a642e6d04d7d305a80853ac0bb6b5bbf12ba5" },
		new() { Input = 33, Expected = "0x3f1a5b2219f9fbb4709e3274aa739a177b709e39" },
		new() { Input = 34, Expected = "0xbe6b53f8c9f3778c7fc37f3713a4603ed6c72fef" },
		new() { Input = 63, Expected = "0x88f2e1b9d26d830b155b20c8ec565b9691f8475c" },
		new() { Input = 64, Expected = "0x0aefe4707e1453b3db2dbef66735e74a409a9384" },
		new() { Input = 65, Expected = "0x38f60e320b275e40c6e4e92c42aa0d161a3299c6" },
		new() { Input = 117, Expected = "0xc77b3db4100910fe03768313a13ab54e54d8cc87" },
		new() { Input = 100, Expected = "0x32788806b1e1176ccf3dd1a47265c038c761ce33" },
		new() { Input = 127, Expected = "0x66588aa446630c43281fa7eaa662c2e2a3abb825" },
		new() { Input = 128, Expected = "0x7b81b41483b630070180a275f352cdf4f915fec5" },
		new() { Input = 129, Expected = "0x5c50e0745fcf53dc512f7dc56c8152246703b7a0" },
		new() { Input = 178, Expected = "0xeeb062c6c9f87c91fe4865a5b73c8172ea2080ac" },
		new() { Input = 199, Expected = "0x668679ce5ac3d88a72a5bdb18ca33d44bdb4c42d" },
		new() { Input = 200, Expected = "0x127130828d6278ac9e47b6c20babd0b4b3dca071" }
	};

	protected readonly TestItem<int, string>[] DATA_TIGER2_4_192 = {
		// NOTE: Input denotes the number of bytes to take from DATA_BYTES when executing test
		new() { Input = 17, Expected = "0x585ed93d3356ad51eaa81fb9f593fa60a64842084f0063d5" },
		new() { Input = 31, Expected = "0x8e6c6ca98cc24271944bbeb901fa9a2ceb7ab361e682faab" },
		new() { Input = 32, Expected = "0x741a642e6d04d7d305a80853ac0bb6b5bbf12ba519e0abcc" },
		new() { Input = 33, Expected = "0x3f1a5b2219f9fbb4709e3274aa739a177b709e397124e3a0" },
		new() { Input = 34, Expected = "0xbe6b53f8c9f3778c7fc37f3713a4603ed6c72fefedbe4c0f" },
		new() { Input = 63, Expected = "0x88f2e1b9d26d830b155b20c8ec565b9691f8475cf5a36ac5" },
		new() { Input = 64, Expected = "0x0aefe4707e1453b3db2dbef66735e74a409a9384710dee14" },
		new() { Input = 65, Expected = "0x38f60e320b275e40c6e4e92c42aa0d161a3299c6cdf33b3f" },
		new() { Input = 117, Expected = "0xc77b3db4100910fe03768313a13ab54e54d8cc870c1a2699" },
		new() { Input = 100, Expected = "0x32788806b1e1176ccf3dd1a47265c038c761ce336d5086e4" },
		new() { Input = 127, Expected = "0x66588aa446630c43281fa7eaa662c2e2a3abb8255684d419" },
		new() { Input = 128, Expected = "0x7b81b41483b630070180a275f352cdf4f915fec59acf6e5b" },
		new() { Input = 129, Expected = "0x5c50e0745fcf53dc512f7dc56c8152246703b7a039b28925" },
		new() { Input = 178, Expected = "0xeeb062c6c9f87c91fe4865a5b73c8172ea2080ac0be31d6a" },
		new() { Input = 199, Expected = "0x668679ce5ac3d88a72a5bdb18ca33d44bdb4c42dcb1fa08a" },
		new() { Input = 200, Expected = "0x127130828d6278ac9e47b6c20babd0b4b3dca0716e0713b4" }
	};

	protected readonly TestItem<int, string>[] DATA_TIGER2_5_128 = {
		// NOTE: Input denotes the number of bytes to take from DATA_BYTES when executing test
		new() { Input = 17, Expected = "0x31f8163acae71a73f662828258b8506f" },
		new() { Input = 31, Expected = "0x4c3a22c2d96ab29ad12100b1f2cf6c52" },
		new() { Input = 32, Expected = "0x5072d1575f95f75eb22169647a0f5b77" },
		new() { Input = 33, Expected = "0x3fb8ab4e655028dbf2aab6ebeee5996a" },
		new() { Input = 34, Expected = "0x026780bd79297995ef4b5e0d9cbdb1fd" },
		new() { Input = 63, Expected = "0xc45fc6510ee3ff3503c4c8795d3d27da" },
		new() { Input = 64, Expected = "0x7e056bc56de5385d47eb3e3a218b5cab" },
		new() { Input = 65, Expected = "0x6b6e1f82c0ea6b6a4b40678c8fd1d8eb" },
		new() { Input = 117, Expected = "0xc15eba0aa26d3668b97f9abfa4bfa051" },
		new() { Input = 100, Expected = "0xcf38de0d363bb17ee67f510900a48f15" },
		new() { Input = 127, Expected = "0x24b3fec9a6235309ae17ee5a972503b6" },
		new() { Input = 128, Expected = "0x61485315bdca303a54a23b3fdf5ab410" },
		new() { Input = 129, Expected = "0x742c2dc251630e13016a4f968e640156" },
		new() { Input = 178, Expected = "0xeac22e2e763c29b07346c531917a0fcb" },
		new() { Input = 199, Expected = "0x2b173dfd8256085aa6b8336b5ce6fbb3" },
		new() { Input = 200, Expected = "0xc2eee732fdbcdc4b0c8f57187a69b701" }
	};


	protected readonly TestItem<int, string>[] DATA_TIGER2_5_160 = {
		// NOTE: Input denotes the number of bytes to take from DATA_BYTES when executing test
		new() { Input = 17, Expected = "0x31f8163acae71a73f662828258b8506f2d8d6506" },
		new() { Input = 31, Expected = "0x4c3a22c2d96ab29ad12100b1f2cf6c52b0f75f4c" },
		new() { Input = 32, Expected = "0x5072d1575f95f75eb22169647a0f5b774bdc21dd" },
		new() { Input = 33, Expected = "0x3fb8ab4e655028dbf2aab6ebeee5996a93fe0b4b" },
		new() { Input = 34, Expected = "0x026780bd79297995ef4b5e0d9cbdb1fdb4f6df4a" },
		new() { Input = 63, Expected = "0xc45fc6510ee3ff3503c4c8795d3d27da2fd4f81e" },
		new() { Input = 64, Expected = "0x7e056bc56de5385d47eb3e3a218b5cab1894449b" },
		new() { Input = 65, Expected = "0x6b6e1f82c0ea6b6a4b40678c8fd1d8ebdd49f3dc" },
		new() { Input = 117, Expected = "0xc15eba0aa26d3668b97f9abfa4bfa0513057f358" },
		new() { Input = 100, Expected = "0xcf38de0d363bb17ee67f510900a48f156fc9e842" },
		new() { Input = 127, Expected = "0x24b3fec9a6235309ae17ee5a972503b60a3e8017" },
		new() { Input = 128, Expected = "0x61485315bdca303a54a23b3fdf5ab410092824c0" },
		new() { Input = 129, Expected = "0x742c2dc251630e13016a4f968e640156e44bf3c6" },
		new() { Input = 178, Expected = "0xeac22e2e763c29b07346c531917a0fcb93fbc72d" },
		new() { Input = 199, Expected = "0x2b173dfd8256085aa6b8336b5ce6fbb3d383c595" },
		new() { Input = 200, Expected = "0xc2eee732fdbcdc4b0c8f57187a69b7017f9ad877" }
	};


	protected readonly TestItem<int, string>[] DATA_TIGER2_5_192 = {
		// NOTE: Input denotes the number of bytes to take from DATA_BYTES when executing test
		new() { Input = 17, Expected = "0x31f8163acae71a73f662828258b8506f2d8d65062b550d71" },
		new() { Input = 31, Expected = "0x4c3a22c2d96ab29ad12100b1f2cf6c52b0f75f4c75f049d3" },
		new() { Input = 32, Expected = "0x5072d1575f95f75eb22169647a0f5b774bdc21dd8896528f" },
		new() { Input = 33, Expected = "0x3fb8ab4e655028dbf2aab6ebeee5996a93fe0b4bb250fb6f" },
		new() { Input = 34, Expected = "0x026780bd79297995ef4b5e0d9cbdb1fdb4f6df4aa94abee6" },
		new() { Input = 63, Expected = "0xc45fc6510ee3ff3503c4c8795d3d27da2fd4f81e5edef179" },
		new() { Input = 64, Expected = "0x7e056bc56de5385d47eb3e3a218b5cab1894449b8e0b55fa" },
		new() { Input = 65, Expected = "0x6b6e1f82c0ea6b6a4b40678c8fd1d8ebdd49f3dc657ebc6a" },
		new() { Input = 100, Expected = "0xcf38de0d363bb17ee67f510900a48f156fc9e8429097509f" },
		new() { Input = 117, Expected = "0xc15eba0aa26d3668b97f9abfa4bfa0513057f35874f50ab0" },
		new() { Input = 127, Expected = "0x24b3fec9a6235309ae17ee5a972503b60a3e8017b66cdf12" },
		new() { Input = 128, Expected = "0x61485315bdca303a54a23b3fdf5ab410092824c0bd8b177a" },
		new() { Input = 129, Expected = "0x742c2dc251630e13016a4f968e640156e44bf3c6fc307665" },
		new() { Input = 178, Expected = "0xeac22e2e763c29b07346c531917a0fcb93fbc72daab36681" },
		new() { Input = 199, Expected = "0x2b173dfd8256085aa6b8336b5ce6fbb3d383c59547e5547c" },
		new() { Input = 200, Expected = "0xc2eee732fdbcdc4b0c8f57187a69b7017f9ad8771fc5ae36" }
	};


	protected static readonly TestItem<int, string>[] DATA_SNEFRU_8_128 = {
		// NOTE: Input denotes the number of bytes to take from DATA_BYTES when executing test
		new() { Input = 17, Expected = "0xa6fbc846e328931dc88705fb073ea10c" },
		new() { Input = 31, Expected = "0xb836d32d1d3c5ed7e2de5d3a87ac7f32" },
		new() { Input = 32, Expected = "0xd79dfce45f664afbbb9b133e14d965d8" },
		new() { Input = 33, Expected = "0xf7456483726d2fc1336b19b71424ce90" },
		new() { Input = 34, Expected = "0x4b9581a6ebd925eb7f25b8786c323418" },
		new() { Input = 63, Expected = "0x58ac92660f7aecb7089afeeb64007403" },
		new() { Input = 64, Expected = "0x574a2dc17d079111c4d8cffdbc280034" },
		new() { Input = 65, Expected = "0xab3371a260714517f2d3f207d355efec" },
		new() { Input = 117, Expected = "0x6d531a02a716e0cbffcac3bd9feb0f02" },
		new() { Input = 100, Expected = "0x2ebc34caf07c26ce6c6b8328c494a41d" },
		new() { Input = 127, Expected = "0x462b8ab20fdcf40ec008c77057d49558" },
		new() { Input = 128, Expected = "0x533d112dfe6131b7fd77974173fa9eb0" },
		new() { Input = 129, Expected = "0x39d7eade9df9220e6d000bbc5ec4751d" },
		new() { Input = 178, Expected = "0x14aab77f4365e07e130496f9630ffe67" },
		new() { Input = 199, Expected = "0xacf0b01a6d8a144ac809a2b48b081b76" },
		new() { Input = 200, Expected = "0x771de333337d9171141b01eb93305d2d" }
	};

	protected readonly TestItem<int, string>[] DATA_SNEFRU_8_256 = {
		// NOTE: Input denotes the number of bytes to take from DATA_BYTES when executing test
		new() { Input = 17, Expected = "0x93fdb3c044cf11b551b7527a59c9eb9cfb1716adc8fc0e1926b246038677968c" },
		new() { Input = 31, Expected = "0xfb7c3d09e37f3388d9a90ca09c87cea58c6efbb8462562f7a4572a3eea194ed8" },
		new() { Input = 32, Expected = "0xc5c78eba6dae1f3c9aefbe8e6608c60889dd8c648efc7b02befccd8bab46c54f" },
		new() { Input = 33, Expected = "0x448c94af0ceddab0a6c2d06eda05f3ad6484512cccc61fa32f902a8e9021b851" },
		new() { Input = 34, Expected = "0x99bd6565cf0c34bb93b74c81e68c5c096731e927c04eb374032e5507ce20175f" },
		new() { Input = 63, Expected = "0x81a91002867a3e930493d9c833655165c63062ea66d65c45f2b1b29fec0d245f" },
		new() { Input = 64, Expected = "0x565e627a7ac890df042565377b1413b30ff2fc1bafa861fa9070526375936299" },
		new() { Input = 65, Expected = "0x0116ec1a605e1c56137427e06599be0bfc243a191988a4ced8a5b461b6f9bf67" },
		new() { Input = 100, Expected = "0x2d5fc09951112a362dc542262351087594e3643160cf87733ef6bc48d9cbe673" },
		new() { Input = 117, Expected = "0x3278279bc38c7483c3c072a892702a9ba0ea909b8a3412a4b48f333c99735433" },
		new() { Input = 127, Expected = "0xb22280ba8e1c973424ddf5be20497e1191634f7c72f46cb0757eb46dac168839" },
		new() { Input = 128, Expected = "0xf456475f82364ff1c5b4d14509b2a06d5fc8512378ec4d909fa9c57c336d2bdb" },
		new() { Input = 129, Expected = "0xc3b087f29c8237981b10227dbed68b203408df8aeb1805089a7a723f02b51992" },
		new() { Input = 178, Expected = "0x0e13d6fc033f4de4e9db360292e7a8c02514534e2cdff6fd69cbdcb515c8760b" },
		new() { Input = 199, Expected = "0x72e8f1ef4c8425356593a9ce4be37181911bcff9d9f426c93aa1622348a2c6e7" },
		new() { Input = 200, Expected = "0x8ba028b1ad51b06d8a92cf3541c817a22c483fb8aa9c4341345faddb8e166867" }
	};

	protected static readonly TestItem<int, string>[] DATA_GRINDAHL256 = {
		// NOTE: Input denotes the number of bytes to take from DATA_BYTES when executing test
		new() { Input = 17, Expected = "0xa8bbfc051b52541e458334bc7f3c947270558b3bc9ba916095dddea6759b7f71" },
		new() { Input = 31, Expected = "0x184a3132f23468a76be67cd1a22cdc8dba62a07fdca77aaec1226c277472581d" },
		new() { Input = 32, Expected = "0x2f994998d00236fb3c9aef653fe88265480f2b2d1ff2d18083f1d2276a31bf59" },
		new() { Input = 33, Expected = "0x69dba37bd0878b57dd717c3be6c38a0791b7794c5df31ccbca4765aa1dcfdb01" },
		new() { Input = 34, Expected = "0x0a47bb3ad99b342e78b4c310aae5f2788b99ecd4c8f2b1f6602b8403a565aeb2" },
		new() { Input = 63, Expected = "0xb4786a17ee4f43ebf426b012834ed534227ae6a3e437a65c6f2ec144e71c3a7f" },
		new() { Input = 64, Expected = "0xda816d00d496dc5b51d344e8688446b437406dc5edac71adebe3677055e48f3c" },
		new() { Input = 65, Expected = "0x81c5dbf4e7e9e75aec198e2f8c0d349c41e497ad111925ef686c051af22bbec8" },
		new() { Input = 117, Expected = "0x68f327d9c4d7bd73b4e5cf1d690dbb13f2f90e52c40eb8b9ee5adc8178aae9bf" },
		new() { Input = 100, Expected = "0x017fdf33b5ff4119f4c4fdd168ceff5374e1389e79b7cbfa1ddea030379b9725" },
		new() { Input = 127, Expected = "0x458f0fe1fb3032dbef2cbce09bdcec534d4ca7410578a0be936c0eea110a65e2" },
		new() { Input = 128, Expected = "0xfcb6b55ebae2fa2b30192ceb744266109974a622da4f90d77359c095ce3e3521" },
		new() { Input = 129, Expected = "0xd468965f4fdb02af048b2acda9171117ef1f66baddddf5020026968172ef3a67" },
		new() { Input = 178, Expected = "0xac5bfdeb639d86c47a02ad1474fc8be022e0859ae89fda0a0d228aae015e460a" },
		new() { Input = 199, Expected = "0x19841ef667fbc743e2b9436c249598b63b2fcdcf80329a93bab531077d4ff09a" },
		new() { Input = 200, Expected = "0xabab0cce36096acf5eae869fbe13e0427d5953664439e81578f1d406ed878364" }
	};


	protected readonly TestItem<int, string>[] DATA_GRINDAHL512 = {
		// NOTE: Input denotes the number of bytes to take from DATA_BYTES when executing test
		new() { Input = 17, Expected = "0x0b25b53c3812cb38fee71eae043331d5486154d4277d63f571ed7621ba1f38816163c16e6445568cde5dd4926249a2293b4c96f1e99d7f0697e9b0be24987fd9" },
		new() { Input = 31, Expected = "0x9c8b6c9737348ea89adf7d3742344c416ca80e70d0c1a574b66d03c3a51fc363645a09b07e6804705726cbb0fda30ad755713f10b1dcd4bbc71d8d975401766b" },
		new() { Input = 32, Expected = "0x04790923e624227751ada31cb344e77ba8cfabea22b9d09fd2f0a867d679e8cbb70665be0fe81554d1a2add1b69bcfd59c8fa452dd7847461c688da80a22df5f" },
		new() { Input = 33, Expected = "0xd0af72a4c6ba8d8a690405f09c794030ae8c134df8ca60af5de4cc71458c0accba769abcb7d1c1b833921d52d44bec149d35110a98d03776ab9fc576f44044cf" },
		new() { Input = 34, Expected = "0xfcfd8e4226478060980bc67a6191f55e772f44327897ea518ed092277112de8e8df8780c630f712a4ee2b4387d945e20e9d1628c5d513ea5ae61f9f2ea476cba" },
		new() { Input = 63, Expected = "0x24b3f7df2ac9e96aa9ce2245e77a3b96a5c1c3c9d070f6806340f65ea9478d4b92ad48b0289d2540a4dc62fa511243eb7ca9808b59425ecc12343b8aff83d4a2" },
		new() { Input = 64, Expected = "0x8830b562ce16b7afaf42dcb1af79624856cdea734b88f7b9f26b147f6e8c716aa0bb48b329ffee5ba8d0a37f205de2dcc0d9359e7e133aae14a201d22e82e60e" },
		new() { Input = 65, Expected = "0xcca9753de1a1a717c1dfb06a1b9fd3bc7bb01ef228d2b10ddbb8e36fcfd30ee2ce6fb4b63c091506cef5c5458f89dd11991b829a817870fa25253697d369265e" },
		new() { Input = 100, Expected = "0xead32ea9bb5b7db55c19895cf6b9ea82bd17ee4a56ac508f3bbeb69a0e5f4df8cf492a02ea5db195f74e6101314ae4917758e0642e8981d947c1dfa16cf651b0" },
		new() { Input = 117, Expected = "0x4447132202fd4a94ae31af19bf454d2c46e4e8a1f82ab214f3eadd9d02eb9d7ebc72ddbf04bab2e0e3a553f4e6ec5b7c8724f20c887c8394b2f970524a3b845f" },
		new() { Input = 127, Expected = "0xf924af50f7cdc77b9199d1af7f1f7fcd454b8b670df3a1d22ec634a502f509f47ff0d6ede8eb26afb94ee45ef819acdd522680a5a6394aee34704f9f08e1a37c" },
		new() { Input = 128, Expected = "0xc86054fb58498874529532408a05101ad0d1753639716f96f56468c015880d7adfc2db4b94edcb50af6e66f87a0d595f7e29a5829edba17c2d039141aec90724" },
		new() { Input = 129, Expected = "0x3db136d934e5c22fbbe614fb7420d9cd70d74d1e868e078bbab97939039124543b0909de500b72114a110b1a94a6dcab623b3f0ac9eb102176023719a8243561" },
		new() { Input = 178, Expected = "0xbe7c6b085ccfa21344e46415a3bb139ee2ac1b87ae569e3f751a563280e879cc7910c357416101495cca5442d6260bf993e11ba1d5aedccad75afd130d4346fa" },
		new() { Input = 199, Expected = "0xeeefc607804883a8e4e24d349297380a7be6789f877d6edfd017b054d6dff6a7fcb1386c5695b76ff9997332125a2e7aadb9533761a2d9fd960f6be4646fbaf3" },
		new() { Input = 200, Expected = "0x13771dd2bd4e1d046acd57457b0cddd6c535d91923677315ad89f7bf2fd3573b31d5eff98eb88798a5383b90d36efabc5b4127eb6e592adceb6a0749bae01869" }
	};

	protected static readonly TestItem<int, string>[] DATA_HAS160 = {
		// NOTE: Input denotes the number of bytes to take from DATA_BYTES when executing test
		new() { Input = 17, Expected = "0x0de9c9930e0c81ffee14d2abdfd5e8dc1060cc1a" },
		new() { Input = 31, Expected = "0x5dcb6006f815009bbe8cfddd87da6fe7983e9af0" },
		new() { Input = 32, Expected = "0xa7677c37c73e855702f19c6d56890244cbbb9973" },
		new() { Input = 33, Expected = "0xf81874f5136d7110b2e7e6ba22bc2f7c332abace" },
		new() { Input = 34, Expected = "0xf4a72a1ea4450b9b6c7e4f9998b4ecfafcea3c2d" },
		new() { Input = 63, Expected = "0x10dd4268805f64d0648e8a2e43a3d245c3ba098e" },
		new() { Input = 64, Expected = "0x07489b7925ca7f7b6c82eb5e0699533c1cd10e1c" },
		new() { Input = 65, Expected = "0x9f5f5b31ecec7dfb56b4abddf4b2fa069cb829d6" },
		new() { Input = 117, Expected = "0x51482c56b4fc39f7d3c0f2e062b1ed033301882e" },
		new() { Input = 100, Expected = "0x70bb6cd7c1dfcf3ec6af4b730b173b2c63c1c266" },
		new() { Input = 127, Expected = "0xae4386955b6177101a4128319eb882f406ab9c84" },
		new() { Input = 128, Expected = "0x766d4a08848c1edd6b97b46c0e017241d3500093" },
		new() { Input = 129, Expected = "0xca9a79ffefadc5f7b0b72e672bd0910212d4e3b4" },
		new() { Input = 178, Expected = "0x98ebda126d905ecb3632a0ba8e194282965b2f15" },
		new() { Input = 199, Expected = "0xae48dd3f1ab7cbe77c4491e8df070e44afbc8408" },
		new() { Input = 200, Expected = "0x0d36b4643f321bff1151ff9493a1870f5bbd57c0" }
	};

	protected static readonly TestItem<int, string>[] DATA_HAVAL_3_128 = {
		// NOTE: Input denotes the number of bytes to take from DATA_BYTES when executing test
		new() { Input = 17, Expected = "0x120b5fe0e400b4fbc3fabf2b80e5372a" },
		new() { Input = 31, Expected = "0x20bc70c3a272cd97de3cb1743a851502" },
		new() { Input = 32, Expected = "0x5a903d96ef73c8fa5cd6a0adf7ee0b67" },
		new() { Input = 33, Expected = "0x898873919b823e4bd68c0cc0263f4044" },
		new() { Input = 34, Expected = "0xf1358e2e87e9d920545980c7c6da79c8" },
		new() { Input = 63, Expected = "0xa7d84433f80946d79c73082b08985f5f" },
		new() { Input = 64, Expected = "0x04cf0e38ecc38c4c78e37f4cdadee762" },
		new() { Input = 65, Expected = "0x71a69f39c02d7de696a8f81ff9db2232" },
		new() { Input = 117, Expected = "0xf63939bfc4c7b4e49fc7c390ba9e3d36" },
		new() { Input = 100, Expected = "0x147e0c350e847be8634b4bdf1264959c" },
		new() { Input = 127, Expected = "0xdbfa321e640b26c794513b06e8cd52d2" },
		new() { Input = 128, Expected = "0xc14ebd6b36ec68ef80418bd2bb550093" },
		new() { Input = 129, Expected = "0xcf2411c1e150346f57a69f9ec1e4e9bd" },
		new() { Input = 178, Expected = "0x082cd44fd909ec66b9638d84912419ac" },
		new() { Input = 199, Expected = "0x5ddd90bd9fc9cd7c6e5ab0d0860b3f3e" },
		new() { Input = 200, Expected = "0x2ab953630cc93f74d0ec5ed61b54337a" }
	};

	protected static readonly TestItem<int, string>[] DATA_HAVAL_3_160 = {
		// NOTE: Input denotes the number of bytes to take from DATA_BYTES when executing test
		new() { Input = 17, Expected = "0x3f751d4ff295f00fe0843524efe86fe19c82c30c" },
		new() { Input = 31, Expected = "0xeb4cfd6634af3e7afbb3c29baf3b8e8d25782760" },
		new() { Input = 32, Expected = "0xb3cb0658582cf3beef824bd8512ce017df84da9d" },
		new() { Input = 33, Expected = "0x88a67d89609fcb0e6c3bc65b48ca501bea9589a0" },
		new() { Input = 34, Expected = "0xba1e3ea228f2ae5a1661dd480a8749dc51589575" },
		new() { Input = 63, Expected = "0x7a3372fb4b6e168b03c47664b8b1eeec3b39c5a5" },
		new() { Input = 64, Expected = "0x3d45e785e65d1865acf898848bdfd711acc4cbe7" },
		new() { Input = 65, Expected = "0xde074ad40f7968415a1e91c70e39b66d5641b3cc" },
		new() { Input = 117, Expected = "0x79e28b419ba7d965569ba46f9a85ba7a3b28c5da" },
		new() { Input = 100, Expected = "0xfbfb171f344b0b329b58123eaca450d53f6c879f" },
		new() { Input = 127, Expected = "0xf6318c15b3da5e97a5b62872860dcc779b1fec64" },
		new() { Input = 128, Expected = "0x5bf49c0977f083f33d0e7f533ed6206bfc25a37f" },
		new() { Input = 129, Expected = "0x34778872661cceb2be324ebfbc8d499b1c19899e" },
		new() { Input = 178, Expected = "0xc666076d77b23cea30d3f831bb2b4c32bb5a6980" },
		new() { Input = 199, Expected = "0xb5fbd50ab784e393d002a4c6143f0b070566d996" },
		new() { Input = 200, Expected = "0xc23e38081df498e07ef09691ae249155ee12e66b" }
	};

	protected static readonly TestItem<int, string>[] DATA_HAVAL_3_192 = {
		// NOTE: Input denotes the number of bytes to take from DATA_BYTES when executing test
		new() { Input = 17, Expected = "0x84d6e8f7bf3e4b816980313f8a5a7e959d07a3f9450b8226" },
		new() { Input = 31, Expected = "0xe3269d7a0523ac9b3c30c563ce1c417ac5cbc11f2cfff4a8" },
		new() { Input = 32, Expected = "0x584d2a34508319b6f1dbfa3f7ecabb5daca4a174d95cf0c4" },
		new() { Input = 33, Expected = "0x88d1a648e1f50d94a690759c9f8e22030f755eacc98c4c77" },
		new() { Input = 34, Expected = "0x951c12c281405cdbaaf4591a4968eeea63e84c25cbcf9f10" },
		new() { Input = 63, Expected = "0xfcd96c38e35ecee836a3c5eb27d408f2d0f2cf2f6e097289" },
		new() { Input = 64, Expected = "0x5908688556e3906cb950a3b6ad6717e12830a8fa1124c3b6" },
		new() { Input = 65, Expected = "0xf55462ad249e96ef343221227148ec32dd575c20edf9846a" },
		new() { Input = 117, Expected = "0x06eb6b7aa258c657b717b489de9373d694e0eea1927a28b0" },
		new() { Input = 100, Expected = "0xa46860969eee93313ff917a315f0273c7add402aa8de1c50" },
		new() { Input = 127, Expected = "0xd45ebf883ecdb41cfad58fa9ab0ce7ccbe44f8763de97922" },
		new() { Input = 128, Expected = "0x143ea5e89e8c3d07858a6ab92b46856835327541fb6ef1ae" },
		new() { Input = 129, Expected = "0x33422455b5caa91c4fc502d3d457e27a8facc3183a4b1bc9" },
		new() { Input = 178, Expected = "0x08042828db88e63be739f886944a3b2935eec9d5ec77c6e1" },
		new() { Input = 199, Expected = "0x9842587b4b0f889d1276cbb34501d90dd032793cbeab361e" },
		new() { Input = 200, Expected = "0xf99e78d3f99f4c30c34178eb9c5d80eb45c71943bf7445b3" }
	};

	protected static readonly TestItem<int, string>[] DATA_HAVAL_3_224 = {
		// NOTE: Input denotes the number of bytes to take from DATA_BYTES when executing test
		new() { Input = 17, Expected = "0x6a1fa67f6aff73c436741eb25a48b8ef74cf1ea293e31a9d1f9f68d9" },
		new() { Input = 31, Expected = "0x7ab8c84e8d64aefc08964c6e7ed50fed30ac936ff2c2548393aff6b9" },
		new() { Input = 32, Expected = "0x5d57febb52ff020b6cb59c1163e907973472d2b5ba06f4e906264068" },
		new() { Input = 33, Expected = "0xae97caf3bd5f727edf1a63574ca7f114af3df2bfb7b41914335347b4" },
		new() { Input = 34, Expected = "0xf6551b9925d609655ef5cd2dd7ecc0da2bb9aad28a4bbaa9bb9a82f5" },
		new() { Input = 63, Expected = "0xd057ad4b2b24beb4f08163c37e6e7e7621ea003e99b6615250d92b0f" },
		new() { Input = 64, Expected = "0xa51dc4f500afea871c789c8b0086b392478ce2ff999b52e0afd850df" },
		new() { Input = 65, Expected = "0x80d232e3394d73efd9bf69d7a2928f5aa704587bf45c90cb8a51c7de" },
		new() { Input = 117, Expected = "0xf0cb30ab478b024fe637cb5405c977bbb17a6d745d7971cfed099687" },
		new() { Input = 100, Expected = "0x2c8de21311bfa5ed0e1576ea8a12c35d64f7d88543f087b669f11a56" },
		new() { Input = 127, Expected = "0xe1ebd7a5494916b844b0cfd6466c9af0f7ba0873fbc7e096cb81350d" },
		new() { Input = 128, Expected = "0x1f72e7fc701bae656f463d086aa3cadeb622dad0ef4070927e577d6d" },
		new() { Input = 129, Expected = "0x87d9a105f5e0d241e4e76dddb85829b45db0e2b570c1dae9804e055c" },
		new() { Input = 178, Expected = "0x877f5d3f0e4ba7e507096f7a5d05fd84b7ac8885331c3e0faf5f53ae" },
		new() { Input = 199, Expected = "0x517e323e80a949643f26e1bc670acaa606b6699200a35062f66ddb6e" },
		new() { Input = 200, Expected = "0x756472897fdf3ea86c7d14bafc34c7d0e1726fd7b966149f58e9cd26" }
	};

	protected static readonly TestItem<int, string>[] DATA_HAVAL_3_256 = {
		// NOTE: Input denotes the number of bytes to take from DATA_BYTES when executing test
		new() { Input = 17, Expected = "0x9488b37c6caf093a8b8c2a7e5ecd353ef0b7d0450daeeeef24e06236b30452c3" },
		new() { Input = 31, Expected = "0x38fbdcd7408c65be6d4badf8888802b8c1127851be217e6d8d8211abd111dfd9" },
		new() { Input = 32, Expected = "0x0aaa1f36755af0d849e1ba53a810df8a5360233e6f715791aeec9d144ec85cb9" },
		new() { Input = 33, Expected = "0x3772869d16577a8540f311e52816e2e717e6df00e5ad593b45024404120a15fd" },
		new() { Input = 34, Expected = "0xd9d660be2accb7b97c37fee503cab755b873691fa1e6355047c580db46c3fdc5" },
		new() { Input = 63, Expected = "0x069cf07c33738d4c1b637e5e25497c932abd0f4cb239c2a7a73382de6ef66486" },
		new() { Input = 64, Expected = "0x452f7bcfc177df3606e9717557ded37071c3b7081dd23261b08545ec3cdf9252" },
		new() { Input = 65, Expected = "0x714e9cad0f3bb4911ac6b7661cd942d656cdeab296460fac6b1c05e749c5dd16" },
		new() { Input = 117, Expected = "0x53800e41b5ce5ddf5f2aeb849176b5a9f62c29353f1329a5813727a080ab748e" },
		new() { Input = 100, Expected = "0xd6891d08d252ba82002ec4b0753d091541b9454ec41f6071480e7d48c423195a" },
		new() { Input = 127, Expected = "0x2081551256107a4df00ad156c8adc09b8f13297d070f0c7bd632d38abe6564e6" },
		new() { Input = 128, Expected = "0x48e3d0bf4c2b82050577803d7b0c7626f7bde3d14330d6d7aad61b978b8138ec" },
		new() { Input = 129, Expected = "0x2a79132fd162f9a2c9d3ee844e5a37b1bd4835804d8ceac5311a47a92c6c3eae" },
		new() { Input = 178, Expected = "0x4a1ad196fc572888d2728cfe2b65a4be871435017d08adf04a20d4de2e0a2b52" },
		new() { Input = 199, Expected = "0x0c2bf9e4f95019e9d7d2d21e9f51bfe9aad0b25211ff884363b4b28c3c5d4211" },
		new() { Input = 200, Expected = "0xbe14cba7ea7da26e8a94233963c750228b4be0be6173a576f7e8a5749ce6e16a" }
	};

	protected static readonly TestItem<int, string>[] DATA_HAVAL_4_128 = {
		// NOTE: Input denotes the number of bytes to take from DATA_BYTES when executing test
		new() { Input = 17, Expected = "0xdca2a0a49b0fb1c05b61ba54e3182913" },
		new() { Input = 31, Expected = "0x52bffe827d7f6e9e37d9eeadce6703b5" },
		new() { Input = 32, Expected = "0x805207dd9b4b8b2b5797a2a5b980a7c7" },
		new() { Input = 33, Expected = "0x58231f3d1e54cfd8377eef566cc0d042" },
		new() { Input = 34, Expected = "0x26ac2a913fb53b8483fadd9b1d3bdc64" },
		new() { Input = 63, Expected = "0x12014e6927053a628b6ccc0db96668e8" },
		new() { Input = 64, Expected = "0x8f496aa4c81de307c400eb80b2526952" },
		new() { Input = 65, Expected = "0xc0d4e6a3fdce54b7f2942b051c8131f0" },
		new() { Input = 117, Expected = "0x9d347fe1e465c405a28b8ef27ca3a02f" },
		new() { Input = 100, Expected = "0xadd5b2dc8be36e661d83afebd51b80fe" },
		new() { Input = 127, Expected = "0x864088f0d2e70110a0ece707ad597832" },
		new() { Input = 128, Expected = "0x3da1dd6e543512ea7d940fb35b621335" },
		new() { Input = 129, Expected = "0x5f3d38ca76f4da33fb25ab330192e689" },
		new() { Input = 178, Expected = "0x8d45bf644ff24a479c6119decfdc1e89" },
		new() { Input = 199, Expected = "0x8b98002b4aed5428eba355df5e2d3b78" },
		new() { Input = 200, Expected = "0x16d42536dc88c0fd21ec2ae8954a88e1" }
	};

	protected static readonly TestItem<int, string>[] DATA_HAVAL_4_160 = {
		// NOTE: Input denotes the number of bytes to take from DATA_BYTES when executing test
		new() { Input = 17, Expected = "0x12c7d969d0a7397d9b2a851af1c6cddf3edf79a1" },
		new() { Input = 31, Expected = "0x4d69d6b82e94aecac369e721fab4a5f2540400fa" },
		new() { Input = 32, Expected = "0x5d4da94d89587527e2e7b2808a53d38b4ea3c524" },
		new() { Input = 33, Expected = "0xa40d4e52f0a5ddde3982bef9e6541edb2fe9913d" },
		new() { Input = 34, Expected = "0xc469dc1a1b4018ce4ffe3e300acd6da9003ffbd9" },
		new() { Input = 63, Expected = "0x428037e464756ece444647b8ac243e5d8a2cf52e" },
		new() { Input = 64, Expected = "0x1c27f892ba903e92e7dc9980174bac49ab347d65" },
		new() { Input = 65, Expected = "0x6a19cac8b93b6cb2c6101a6c821f0c43f2dc8bfb" },
		new() { Input = 117, Expected = "0x15b94a9d014d1f49818cadc19eb34884ce8825d3" },
		new() { Input = 100, Expected = "0x4a3099ddf93cc105d4981f9f050fac1099b4e8fc" },
		new() { Input = 127, Expected = "0xe4b2a8ead2b4271a9eb17222587dfbe89cc7ecde" },
		new() { Input = 128, Expected = "0xd3bfcc2f2b471e9f5cfc3561701374a4b4ce151f" },
		new() { Input = 129, Expected = "0x2ceff11a80c74f061e82b4c921d4dd583976c9ca" },
		new() { Input = 178, Expected = "0xf355ca6a1c2c25674a9680204c0a82f80d813354" },
		new() { Input = 199, Expected = "0x3e9b62a383be357d7e8c0a84c84aff1118696818" },
		new() { Input = 200, Expected = "0x6f5f6542584ba856bb69cf33a52e0f6c6689b490" }
	};

	protected static readonly TestItem<int, string>[] DATA_HAVAL_4_192 = {
		// NOTE: Input denotes the number of bytes to take from DATA_BYTES when executing test
		new() { Input = 17, Expected = "0x99eaa62fee298c9901d9b8c9e5bb494b3df5a41700a86384" },
		new() { Input = 31, Expected = "0xa70947f5537b924403b74bcdbc6a6afbe91f2dc6e4013df2" },
		new() { Input = 32, Expected = "0xe8b5115afca4237746d7a3282e230f93d47fb89ad4a42dd9" },
		new() { Input = 33, Expected = "0x5ab64fc1163bb9dea8cf5646658b657fed5e67be4d2c5894" },
		new() { Input = 34, Expected = "0xe6a16a2f823a70075f71a183bef47ffe7fd70f82f14d4aa3" },
		new() { Input = 63, Expected = "0x4f0c8d7a4f24a2964dde343b4924f5a74aaa89d6555a6f96" },
		new() { Input = 64, Expected = "0x749f8a1aed9387dab5d272e79b8287f1d2de6f874fa931b3" },
		new() { Input = 65, Expected = "0x9bb110770244c582e41603ec7da55fe61fa60704c73eadd0" },
		new() { Input = 117, Expected = "0xc7084fcebb726613361a19db17383bf3f8b4686b174ba529" },
		new() { Input = 100, Expected = "0x159df5d409ee6aa6bfe59f1c98dc8609edd9ea488bf54a3f" },
		new() { Input = 127, Expected = "0x19bf93b65039bfb6ba462259c80f5654bdfca75e68abb991" },
		new() { Input = 128, Expected = "0xd786674b2732ad498341ba352fec631b19dc8eee8f054075" },
		new() { Input = 129, Expected = "0x2a4a2dde39bddd32ad7bb40b51bada769dba3310a6773db1" },
		new() { Input = 178, Expected = "0x7e963d667873b0e8cde3b7d34bbb77c2149c4f9b0d153a7e" },
		new() { Input = 199, Expected = "0x97af585b2385476fb5382773e1d83d51839cce60786457cc" },
		new() { Input = 200, Expected = "0x718a89ed7b121d4ff6a0e1188e50e53451f8c3e936036ce4" }
	};

	protected static readonly TestItem<int, string>[] DATA_HAVAL_4_224 = {
		// NOTE: Input denotes the number of bytes to take from DATA_BYTES when executing test
		new() { Input = 17, Expected = "0x5fe77e369d99d4eda48689d2f758a734ffc5fc003ef4d4c686fe6134" },
		new() { Input = 31, Expected = "0x0ba7d1b3a6ca28819a1ad6d7e0853b2ea9369eff5a6d34837022bf1c" },
		new() { Input = 32, Expected = "0x7c7b63873c920a266582192efc501748b664860252ed6635cb521005" },
		new() { Input = 33, Expected = "0x0214db26cf61cd5a8525bb80403530bc4acbde40c3d6f963d60b2a91" },
		new() { Input = 34, Expected = "0xbe03cc76307c3afb33f3e3aa33e657273fdb4267a4a72e02a9a1f3ce" },
		new() { Input = 63, Expected = "0x9e36d956daa979e1aca4ea4a2bc2a7cf77eb78131ba1834d1e038f20" },
		new() { Input = 64, Expected = "0x131f5f8c06adbff363d44e68759f4f75fa43b5903a1b37bd9226b7a0" },
		new() { Input = 65, Expected = "0x304bd9d4aa72150e9d2b48071355859175b5aa9c3694bad506d32480" },
		new() { Input = 117, Expected = "0x71d7d789e77942833836675d98ec65e1d16dc8e02aad5bd847b72220" },
		new() { Input = 100, Expected = "0xadf867d248bfe34af82ffbd9065fe7090e67db8c05a119a2257c801d" },
		new() { Input = 127, Expected = "0xe082599737d085150423546ef7b8c921458d10b13e5d4fbf96ac37c2" },
		new() { Input = 128, Expected = "0x5a1520003337e7e13025ba552e4f93d40c3bb4ead7e37187ac745d38" },
		new() { Input = 129, Expected = "0x67a37c1c9b25051ed998c05d4490708427e2b2e84dacdd494f280b3c" },
		new() { Input = 178, Expected = "0xe58401cb5a43e1249da74438f70164e7986130d1f2adaf9656f39dff" },
		new() { Input = 199, Expected = "0xd7ea3b79cbe0368d53830d31f68c2118a6990b0f6c413c3ab1000a73" },
		new() { Input = 200, Expected = "0x9c67b3e1beb1c3ff4a3d9b0d51682389a254c821d572d14fe870e79e" }
	};

	protected static readonly TestItem<int, string>[] DATA_HAVAL_4_256 = {
		// NOTE: Input denotes the number of bytes to take from DATA_BYTES when executing test
		new() { Input = 17, Expected = "0x05ebec6e852bf7512c2bda21f44c52f850374e312737c34b06bee6575f43bfcf" },
		new() { Input = 31, Expected = "0x82189fbae31134114fa623dd4bbb0ce02bfea9c2e6373bff56e4cf6ef0dd3227" },
		new() { Input = 32, Expected = "0x5a6e6c868effe8df7ae60a789d6b6df89b41b9b77b9ed51796424556e8147746" },
		new() { Input = 33, Expected = "0x581f59b7b98fa7607642a145b638b84b40179ad5e6509c727b1af234db6a03f7" },
		new() { Input = 34, Expected = "0xdf3f0fa30b99e0ce010a17f7dc8629d7c1263414c39ee7465ec24a3705a8ba08" },
		new() { Input = 63, Expected = "0x6d49b91ecf0964a0bf3ecd30dccf84ce4860fdbf5d53e98d755f9410a1ee84b5" },
		new() { Input = 64, Expected = "0xae6b654546e2fb648e814d4271e46c8a979fead9fef4cb673c1d9fd432c3de59" },
		new() { Input = 65, Expected = "0x6f2aff7517a74c90cb2208e259aa74097cbce776268ece5add3f03d908b157e2" },
		new() { Input = 117, Expected = "0x13c5cffccf0a3d858afb4a0ddf5cfaf7f7defafcea0989dedf1f8a9c970fc065" },
		new() { Input = 100, Expected = "0x796529ae52c35a8ae2bac7d4e51cc6daf1d72f49d621fa373e91e438a101acb4" },
		new() { Input = 127, Expected = "0xe6d938d79ee1967718cbb07ed356fab1628f0f507b7cbb7bf6ec166e3abca5e5" },
		new() { Input = 128, Expected = "0x6aead7a0e80eeb7ec82def7aa9e094d7b8d88edb37ff307ba45fcd890f5f47f7" },
		new() { Input = 129, Expected = "0x0d6a4c5bbb8ee8a2270ece0e5811a21cc1a8f71479572599fff182f19a39a668" },
		new() { Input = 178, Expected = "0x6de3a437b6f7ecc066fbfbce65a178d79834916be4e17aea3c5ad28584454637" },
		new() { Input = 199, Expected = "0xa1c3208ab02ce3e276f5332cc8f1b1258c1f11225ba9d2c1885e934e0fe28d51" },
		new() { Input = 200, Expected = "0x4da9abfb07d05da6db4398b04f7d3fb9fb816f1b53427cd34945b93164744d9e" }
	};

	protected static readonly TestItem<int, string>[] DATA_HAVAL_5_128 = {
		// NOTE: Input denotes the number of bytes to take from DATA_BYTES when executing test
		new() { Input = 17, Expected = "0x5c36785f3a4c8680469cdc31a755c954" },
		new() { Input = 31, Expected = "0x0871eef2ffcb68a821b0ad9c769d306a" },
		new() { Input = 32, Expected = "0x7fc2474f032dcd23f27a52a43dba4108" },
		new() { Input = 33, Expected = "0x0e82bc03b53f6f24441e5966315503ca" },
		new() { Input = 34, Expected = "0xa8ff6a58ce8a74b7725b0e96d77723b7" },
		new() { Input = 63, Expected = "0x2ccecb866635f2853b9718dde9a182e2" },
		new() { Input = 64, Expected = "0x1915e1b50e1ad1df36d79261ae641060" },
		new() { Input = 65, Expected = "0xb73826215f83796b8d59c78081ee90ac" },
		new() { Input = 117, Expected = "0xe6de37211798258836e678dbb1575e5d" },
		new() { Input = 100, Expected = "0x8ec0296bd84e40f5da6e8c7397bcf3e9" },
		new() { Input = 127, Expected = "0xf5fa972cf7185839469c3701513a37e2" },
		new() { Input = 128, Expected = "0x2c261e45788712f1588bc72b404f0524" },
		new() { Input = 129, Expected = "0x35acb344ed18902787c12ab8a8376d30" },
		new() { Input = 178, Expected = "0x4784db607b199274e8e6e157331a463a" },
		new() { Input = 199, Expected = "0x6708cf09a216b4bf020fcbca8f6af541" },
		new() { Input = 200, Expected = "0x53260c9977582901509fd95e969ae3f6" }
	};

	protected static readonly TestItem<int, string>[] DATA_HAVAL_5_160 = {
		// NOTE: Input denotes the number of bytes to take from DATA_BYTES when executing test
		new() { Input = 17, Expected = "0xc8f4a3717c7ae970dd343ea6fd8cf2a17d099818" },
		new() { Input = 31, Expected = "0xc6afd4b452705ee36370cfd9ec0fc3b40303f468" },
		new() { Input = 32, Expected = "0x2a140cdeca4322db86a6164e7e972693ffbf0783" },
		new() { Input = 33, Expected = "0x374e76edf0c852735daa5bdeb437e10083916c2c" },
		new() { Input = 34, Expected = "0x4bbf730e316b75ee5588d3100c40138a4cb394b7" },
		new() { Input = 63, Expected = "0x058f724747643655df9fe0fa8ae8df117d137eb3" },
		new() { Input = 64, Expected = "0x1130cc363c6b8b3a81fa8214814b82fa26adecc0" },
		new() { Input = 65, Expected = "0x1aa37c5b1e0795eaa86ade17b5f4c28533f157e5" },
		new() { Input = 117, Expected = "0x559b3a404f58854d450724d9da7f4198b05c07db" },
		new() { Input = 100, Expected = "0x61f4d70f3f94a1b63c8c71b3a238ab4be46e5557" },
		new() { Input = 127, Expected = "0xc756a7e6a02f7ca38661bb88968b99322185f5c5" },
		new() { Input = 128, Expected = "0x4779da21617c154c3f0d9936f84cf84c85138f5f" },
		new() { Input = 129, Expected = "0x6a8c7279be3bd1b80e1cf52d46ba2baa297e4cd8" },
		new() { Input = 178, Expected = "0xbea651ecce3ad71665a1f4d99028c3c2eb63bbf1" },
		new() { Input = 199, Expected = "0x6e2dec45f7319361e69001a6da311d84697dc480" },
		new() { Input = 200, Expected = "0x96a177296ed0d67014e2fe73628f737dcf314cce" }
	};

	protected static readonly TestItem<int, string>[] DATA_HAVAL_5_192 = {
		// NOTE: Input denotes the number of bytes to take from DATA_BYTES when executing test
		new() { Input = 17, Expected = "0xde071aea15eb7709a9cdababcd06c15718ccfc9089f4a0a9" },
		new() { Input = 31, Expected = "0x48bdd6654d320b4dbaa5390d07c4536472181330320d11c6" },
		new() { Input = 32, Expected = "0xe6842b0c87c9bf82c89a9b62ed23ae97730cf85f24a0c371" },
		new() { Input = 33, Expected = "0x4067bcaf2dd05d2f056a9f7292352f881170a19896536636" },
		new() { Input = 34, Expected = "0x8a03c5d18701f8a9ece7655a225e3dd8b54609712a9e05a3" },
		new() { Input = 63, Expected = "0xcf462e462b4cd4ad95d9eff4eac9aa8336703cc13c78f003" },
		new() { Input = 64, Expected = "0xa3951743623a3e452b81a8c21b80390c254bb7515d4c5b92" },
		new() { Input = 65, Expected = "0xb3bd91c00640be13b06d435e5a0e2a14f6301ac7bfa1e763" },
		new() { Input = 117, Expected = "0x41b708afb8e7706aec02ea741d0e17bfcf69e8f6e65cb5ea" },
		new() { Input = 100, Expected = "0xdd00942cd65261e275136fb3e62336adf5af1bc90246c828" },
		new() { Input = 127, Expected = "0xf92ac30dcaf5658896c5b4f3fb1682e42e69a2d577032480" },
		new() { Input = 128, Expected = "0x52af2e7944cc70b0026d375470cc9178ea5877bc2125a21f" },
		new() { Input = 129, Expected = "0x78fd59c64b55081d42291311646d8527e3e5d9bf440c39de" },
		new() { Input = 178, Expected = "0xad5a46ad1b08d6492f9bf7eeb5dd2cc931e7e2b04827e023" },
		new() { Input = 199, Expected = "0xdf9c163d6c839398348aee11c78cb9a9afdd7b157a852b02" },
		new() { Input = 200, Expected = "0x75bae49f70c0e27ad6e840cb64c819d98be5976160878dda" }
	};

	protected static readonly TestItem<int, string>[] DATA_HAVAL_5_224 = {
		// NOTE: Input denotes the number of bytes to take from DATA_BYTES when executing test
		new() { Input = 17, Expected = "0x097075ac2d6828cb27caac4df862f4d450573c6f6c545bf42e42ba7e" },
		new() { Input = 31, Expected = "0xe1e40eeb2f0c93057ab548d77f81e268a3d92dea2f33728c4673600d" },
		new() { Input = 32, Expected = "0x28d197f51bcec28e7b8f93979c070634d77e090f52966de3e8fae395" },
		new() { Input = 33, Expected = "0xe333926d4ce7e43c48769d52a3db5f9305d31f5c8901c51f215e7067" },
		new() { Input = 34, Expected = "0xe56b2c6aa146a14bad393ae274ec90c3226218c6092e823e36b6d9f9" },
		new() { Input = 63, Expected = "0x7a77e4be141facf3b5fbfd641e1c077f9d211112ec9d7e08043542ae" },
		new() { Input = 64, Expected = "0x4e3135e613e837a7d57758cfdaafcfc3bb26f832f07ebc466b2ad905" },
		new() { Input = 65, Expected = "0x6350baca686e0e20914e3f99b63f90eba8e4f95dfe7b06072bf9cb18" },
		new() { Input = 117, Expected = "0x6779fd851dcc38583fd516720a9b4a8e26ce686244a79f733558638d" },
		new() { Input = 100, Expected = "0x423a392c0a92d5d7fd2f500e907d4433a3c6f23b49ca3de57107015a" },
		new() { Input = 127, Expected = "0x1e2ec33103cef66409e5cd65852c81cddce1c55b4d9fdabae7631940" },
		new() { Input = 128, Expected = "0x77c9c5b359f40d7aefe309e14208b0ec96fe2b87235680030c0d6f7c" },
		new() { Input = 129, Expected = "0xd77bf5e0a98a4b150f1a5390e2cd6c58a5983252d6d621e376fc426d" },
		new() { Input = 178, Expected = "0x6dde5baf5a88dece5a582c390153f735b63b1c06f1a96e332db5f5de" },
		new() { Input = 199, Expected = "0xc934124bc9edfcb2120a6b5338731bdf8f6b2759af67716e2ac1d9fa" },
		new() { Input = 200, Expected = "0x915da86942b18e0cf0261c13ef6be9a755928ec609b65177e7b15358" }
	};

	protected readonly TestItem<int, string>[] DATA_HAVAL_5_256 = {
		// NOTE: Input denotes the number of bytes to take from DATA_BYTES when executing test
		new() { Input = 17, Expected = "0x1c3afb53f06ade5399c4797800b44abc301d9faaf698fe66ca36b18a26da5153" },
		new() { Input = 31, Expected = "0x4cccbbaf8e991a805626c96f3d2850862ae7e77e970a6e7b818444a7c92c8cf9" },
		new() { Input = 32, Expected = "0xc5b81863a6c8c1ac6cc3f429a7fce6ff6ecb1f459856d241f5c5f1820f229927" },
		new() { Input = 33, Expected = "0x6453387b3f0b6d6dd6a8343cab021ceeedef2f8fd852ab35a8aa5472f3653909" },
		new() { Input = 34, Expected = "0x71dd44d1eb0ced6208ac71360611b7ac50cbc49365c135fa253771814f8fd224" },
		new() { Input = 63, Expected = "0x09182c9035cd025d5cca7f2a9525bccdf8314d6c03419987a03a0bec59e76e38" },
		new() { Input = 64, Expected = "0xd6cc048cdd7c944ad99b1bb8ff9b48bf8f8ecfa783369e3d008902fedd98009f" },
		new() { Input = 65, Expected = "0xe7e1ead7bad22f210bfe98825022a71e9ebc8b85cf8710b2ef6fb9e457fb96db" },
		new() { Input = 100, Expected = "0xef0ecb677bee8f32a0e234f9f1944528a17f2e148634d7ee99d490c21898b245" },
		new() { Input = 117, Expected = "0x883de42fabd84a49dbc4a5cc6a71f6b8c8c2b2ce91eadce672a21b0df5d38683" },
		new() { Input = 127, Expected = "0x0c57f4b86511b060b39c9d7b101fc6282642654890fc9dfdd010025e632c9ce8" },
		new() { Input = 128, Expected = "0x63829e28fce75643700ebe1e4750fc26001c81335401b19b5e86acf3866e4672" },
		new() { Input = 129, Expected = "0xa29f9c16a35abbcc06d5f3e77854008dea21c38093729ec347cd3cf24ab6fdc8" },
		new() { Input = 178, Expected = "0xf83274137a08ac4f8738587e643a85907716f2df0462d32673f5d79c5e301e6a" },
		new() { Input = 199, Expected = "0x9d08dcbcffe809a60e60fcad8b515ed73e339e73f885c5b50479d7ea2afb6e3b" },
		new() { Input = 200, Expected = "0x5e1e2503132805abbdd447a5428dc9ddf7071da09fc5bede1a2db78731177fee" }
	};

	protected static readonly TestItem<int, string>[] DATA_KECCAK_224 = {
		// NOTE: Input denotes the number of bytes to take from DATA_BYTES when executing test
		new() { Input = 17, Expected = "0xb528a498c3cf62dee1642dcea4aaa47d8f11d6826a61a6aed0dc2272" },
		new() { Input = 31, Expected = "0x24ad1ab7a57969f3322cbf88637c2063ab9f72fbb358565ab45ab41b" },
		new() { Input = 32, Expected = "0x13c9101dd1f472fcf484c20962fa65401724fd9c4cbd926c0a7ad2b0" },
		new() { Input = 33, Expected = "0x61992904211eb47398ebca0f6867aad611d973242a4013a0ac725283" },
		new() { Input = 34, Expected = "0x4f0e46a5050685e52771a1785d2558cb8232f6e2bc1613aa7fa86852" },
		new() { Input = 63, Expected = "0x1f8793dedee6209952a48234851435f8c8a3fa032996bb9f477a7a20" },
		new() { Input = 64, Expected = "0xcf6419899b2113aadfc0ffaa26d0bdc85e42538e69e61d791142a473" },
		new() { Input = 65, Expected = "0x3f96bfb7d91cced8aac8e21b2e129821b04b648be52915fa5c47b0d1" },
		new() { Input = 117, Expected = "0x3bb3f9905a04657de657b4abe225dbd4f93cce2f52945f7c6adaaef9" },
		new() { Input = 100, Expected = "0xb0a4a937cc6b60b408f2711ff5f1df05a48ced882ac8ba99d7119645" },
		new() { Input = 127, Expected = "0xcc9c824bc2780bf304470de750ad086397f2b05bbc38e6fc4f47731b" },
		new() { Input = 128, Expected = "0xf45b8f21210090b7acc0186a64da6e5b7f944415660eb07531fe36d8" },
		new() { Input = 129, Expected = "0x3aa8909a7466c00dbdc61d3a55df173531239eb16d556bccbbe00ee4" },
		new() { Input = 178, Expected = "0xe5ccfc343fb077917579bb14ce3c31c935e032b674e6bc0ab957d23b" },
		new() { Input = 199, Expected = "0x39cb4117c98a5071b34c43a81a76d712be15ba63c6105d2c6f1784b3" },
		new() { Input = 200, Expected = "0x2861c1e403dc473709e495aa3bf7769f287c7fa49187f291c1cd8816" }
	};

	protected static readonly TestItem<int, string>[] DATA_KECCAK_256 = {
		// NOTE: Input denotes the number of bytes to take from DATA_BYTES when executing test
		new() { Input = 17, Expected = "0x2445c06e529bfac7e8af2a7384c598f2fe6f5af9a44cc1e7810f3d00be0e7e5a" },
		new() { Input = 31, Expected = "0x138d8a8faa38a561dd023e085359c2e5d808e2692d8c4263fd1569f681dcd451" },
		new() { Input = 32, Expected = "0x0a850d8ec0e9f36d3c75edc47f1e08da693924b665648a51024abfb943096306" },
		new() { Input = 33, Expected = "0x7da3e291fd354f138a2e2bef20999ae90910a112e280ecd4d1eb0e829bc8a2de" },
		new() { Input = 34, Expected = "0x430dca71ea93ab23c179753977057fdcdaa8c17652cc6855cf9365cb8124f6b8" },
		new() { Input = 63, Expected = "0x761f730b4574867fa52f2aaab532b2eec5b3ed1d775067f3175604a0144a4a52" },
		new() { Input = 64, Expected = "0x30264a5a832e703a50c52b41f62771cab58669fd4a71d0d3b651d9e72655ca58" },
		new() { Input = 65, Expected = "0xb7227663352d9021bec515b24abc78b2d749408c3e8e0a6ec8040d90fbee10f1" },
		new() { Input = 117, Expected = "0x1e1b9782f4f489b6227cc50288d66fd67df79d0ff07abba875b8d29832b9c45e" },
		new() { Input = 100, Expected = "0xce57f19397fe81b90281492149d81fa6b6bca9b9943e0f9792e0b27a8776e95b" },
		new() { Input = 127, Expected = "0x783d9c05516307b2568392a8f67612a7839a7cf1185902f231f352187c7e78c2" },
		new() { Input = 128, Expected = "0xb5225ef73855a031085fbc6fbef320fa691fccf8de9d28b6cfcd68a6b6518e10" },
		new() { Input = 129, Expected = "0xeb4c031cfdadcd3ff6b9cf32371c8386f96875cd24da562bfc7d727a5da95f12" },
		new() { Input = 178, Expected = "0xd0465275d67cecc303cd5793c14692d760a3b95b672daab065409d277888b574" },
		new() { Input = 199, Expected = "0xbfcf47ec0ea749f22f391a9315a8b5f4af8fc7cc5a2b634fc92619462d935019" },
		new() { Input = 200, Expected = "0x3f5d661c9277e8068bd1c3baeb791feacd1c1d431fe86fda43ac96ae2e7bfd7e" }
	};

	protected static readonly TestItem<int, string>[] DATA_KECCAK_288 = {
		// NOTE: Input denotes the number of bytes to take from DATA_BYTES when executing test
		new() { Input = 17, Expected = "0xaf71a3271ef161236c1a1dd24518fa721de7b1158ea5a728fed80d43c95d557d16ed5706" },
		new() { Input = 31, Expected = "0x90b4167d693f4b184ddbac0666b3c85d8d747499c50f1ef17b0e0d64a685c19fd9f6ed6c" },
		new() { Input = 32, Expected = "0x27505e2d430a543aad7fefa637c2df22509b0ffda41380bc258b0cd8bb4a1b0b156f3db1" },
		new() { Input = 33, Expected = "0x01eec7fa88ee34fa459aac0f19f971a12126e0a4c7a2bdc378e9a741dfdf50c4963249d9" },
		new() { Input = 34, Expected = "0x16bc58943ae64aa75e17c6526c3e71f6006e1c48fa4fd3fb0105b4f43398c485d7e2e7b3" },
		new() { Input = 63, Expected = "0xa965e13d39848b524e7cf886c71c694eb83fc8522e83bd82f7de0bf3f68aa1876e1dd6e8" },
		new() { Input = 64, Expected = "0x17b5ecfa246d4bb5a5bccceb8a74941d6da1ef35c7785ad4059649b1ac6535f0224a0f77" },
		new() { Input = 65, Expected = "0xc0dd11a972482251731d16d52931a263b5b400f09a9982e6e8cdbf10749b9f1a25fe8469" },
		new() { Input = 117, Expected = "0x31e41e1c22f23be8e579e149b90e58c18717e086a26306614265c1ff5287000c0a21f4b1" },
		new() { Input = 100, Expected = "0x2cb32d240e5d57d6511ba2db4ad6713377faac779052e19a425869d3189f37b49af3cc9a" },
		new() { Input = 127, Expected = "0x8c5da2c308e75f4d2b99a9a039b0d33b03cc08310418433571933e7e53f160e3ae33fa28" },
		new() { Input = 128, Expected = "0x7bc32cce595946fbdf4936f4ff913b5a07670c508313c46c2ca370bb53d686485f55ea6b" },
		new() { Input = 129, Expected = "0x8873323a395262224d18ea068124ca3f208241c61d0de2e753f94db2ddd7062e1d654c9f" },
		new() { Input = 178, Expected = "0x07cd6d165247575bf0f0bdf6595aca98b3381cbbb3aa9e763d3798db2e578497e643d67c" },
		new() { Input = 199, Expected = "0x0d3ec6ff8438a51da2ac4cb8d0add686f5c44a9ecf3de3a927123c6548392db0cf19cf8a" },
		new() { Input = 200, Expected = "0x5b60e4054d6706fb135ccd781a645e6f60c2f7776924096e8ddb1322877776e54119df26" }
	};

	protected static readonly TestItem<int, string>[] DATA_KECCAK_384 = {
		// NOTE: Input denotes the number of bytes to take from DATA_BYTES when executing test
		new() { Input = 17, Expected = "0x1e55d4e7d87bdb059670fc0b5d4d83b4a9437f1a4a71b576b3109d3d2986cbc181647994f6b1b2fb982849dc4e675374" },
		new() { Input = 31, Expected = "0x25af1628a3a252c9506f600dbd4f9c2b56f4282fe7511843d5f90dd54a0505845cf992042398bc0d69442dbda399f219" },
		new() { Input = 32, Expected = "0xa7a9ba391aaee75414e5008cb35002db1fa2c6741fddf339211b46920740b0a0c43e9d0ebcc61f697fe73ed76ef7466b" },
		new() { Input = 33, Expected = "0x01e4b1bda5d6d9b078886777de3e07656b4f691ca22fa1e037a9106f120fa66472753474e283069a526361d436f29bcf" },
		new() { Input = 34, Expected = "0xbb307ae8d39c1cfa8bb59c62c1e3bed1ca00cb085eadc7de7e94c22b29a3a5deb754d9083121436ccfa0061e0041205f" },
		new() { Input = 63, Expected = "0x4b2e3ece4da5fb3e4be569b6f8d1b2396b78fa5bce0a8dbd696c653c55970f9fbc99f63def1d4c43f6efcfeaba505c69" },
		new() { Input = 64, Expected = "0xa7a19c6e4a259d4aba1b31848a0acd11f4c4495e959981fa41ccd99a16f92d5c71b680ffbeb1c05ab22bd518dadb1873" },
		new() { Input = 65, Expected = "0xed02b920d964ea30c995908932251291d8a58705b55a54b507dd77208ec7b34d5070b86b3abf716b8111a93a2725db5d" },
		new() { Input = 117, Expected = "0xce7a7c0593492e66b9956d3a6a1aebc89ebabf66c6471ac87773012d28b9971d07a2c25f4ea800164bb6132a1aec2fbe" },
		new() { Input = 100, Expected = "0x7b7e87750b7a74f42e24461aa3182fe1028a4523ea6c192f7c8700e330955acd608b6799829a5f346296458ddd71e0b0" },
		new() { Input = 127, Expected = "0xfe3b99f20ab78640547850c49173a98d12ca0feaa39f7434a475462c8d262e896827c9d3f8b2b53b8532be18cad8f875" },
		new() { Input = 128, Expected = "0x8748ec03174d77248853138a53d4d0c52678f997057e346c3d756960a30ea8e6cbe87388b9e7fe51b75325188361dbc8" },
		new() { Input = 129, Expected = "0x08b32294fd2806948ffd1cc63b24fe8aea6a6145629a66def9f8a3b6c25374fdad0c46d2600402f850df469cf93c3b71" },
		new() { Input = 178, Expected = "0x235a4cb6928ac901d89ca27c6e6be2bee4f3f2eddc70f92d37a742d400d809ea4e894361d5a3234ad42a79074483398c" },
		new() { Input = 199, Expected = "0x917e4351df67587f53ea904a1f6bd104f5d52d867eb900fe342dc442f67512d5d35d78c7ae776a682ad059c43e6f9101" },
		new() { Input = 200, Expected = "0x583f86d7747ec35ae6c269d9274c24a471a4ef94a752b330d36c8891c247a40e61f08937bc5e00531e62a5512fcd6a2f" }
	};

	protected static readonly TestItem<int, string>[] DATA_KECCAK_512 = {
		// NOTE: Input denotes the number of bytes to take from DATA_BYTES when executing test
		new() { Input = 17, Expected = "0x33a2b5778497d950418def75b86f31fe72ab3ef91d529d4b0c9ec099e1d73ef1ee3483f142fe533ed28e3eafd3908b19821fe3ea298bd948088781b21e55fce4" },
		new() { Input = 31, Expected = "0x16673011339dfc4fb91592717a3d9a9b87ea079bf9617f4a2efb3dc34d5ad4b9b6abc92b24aa04753ff160ebc5444061a5b29823f0b74307975eedbb2788dbdd" },
		new() { Input = 32, Expected = "0x2cda2cbe332a6270386e999e55b32a5f5954537a6312b722139125a73c015a50d254f6f90ab3aedc3b88b457723f9a2f0a079d0882bef5c1fe7e8c2c6150621a" },
		new() { Input = 33, Expected = "0x8e2914e34bdc819d7eb4ecf2ec35637d5f36660a3ff33996598f5ca5399c3d1b5da6fed3e115509063f4c25418e5a011f699ce6bc6070ed02ff915dbada86633" },
		new() { Input = 34, Expected = "0x371154907f5672c76ffec9d30c342a9e334b1d951f6f2d06b5145e18a2641bcca600a3c901c3182efe1d72c2ef4a7c8907f2267a79b411ac2be463dd15d029a6" },
		new() { Input = 63, Expected = "0x07920c74b21fa46654ee686a4c1aeea04b99ed23219144d74c3ddccd83713b2c32fab5225642ef4ee94c6dfb916fbd4fa769e0c303e69bfbfddd1939867f46c8" },
		new() { Input = 64, Expected = "0xf3777e6c020fb53b4a6dd57ac7d34218b8f7c2e612aa425ff8741d0f260e0be388eff79df477da3f9bc6e0dcaa1f1394c808cc5f8ea7edd4a06723e70ab007b7" },
		new() { Input = 65, Expected = "0xe028717d8f536d4b47599030122bb2f91ce09c029a2458a3b6a16036dab2938ec92355531201a07743610aeb521191bee86be60219e6dc8078bad0ddca794223" },
		new() { Input = 117, Expected = "0xf50aef9b368e7e3cda0e8f743f49cbb73630280d89365357dede39d6181b9f87d673e6765b7755e27f574e7f1a8e15c0ef24c516e7eea9e0dbcd29f18968889d" },
		new() { Input = 100, Expected = "0xc0b70b4eb9657b87b68c8a383ca5b3b1d625192a1181f1a7be117ddaddaa330831bd4e213dd8964dd423b7921ec6d8e7c71cec833c3819305ca8c7db024e30f0" },
		new() { Input = 127, Expected = "0x0d7af38a059b06cdb69747f7c427167c17059aae3af2770b1358551a1b76f87f874ad7de98d391e8ed2bbd4836d099605c05a98d93fca920ce73b0fd699aca8b" },
		new() { Input = 128, Expected = "0xcfb70c5901e286d4c5fbd55da006cf09e6232ceeeaea10055e19013531a5e39c9ce16d8f8a5a58d9ad95f0015e33fb99b7059bae97cfd2a1a8c879161d84fc72" },
		new() { Input = 129, Expected = "0xdc940335ffeb88002e1cafa1d4b6d740e16d81099d4b014048b8792f046e9b37907a599324a8fde48cb1f9d18930f4d67939fe2b97ca991ffdd9613419ee4be3" },
		new() { Input = 178, Expected = "0x379502aa0a3ef35f35b46c00b336031a09cad9ba9108165064a855be60ce10400f88558dae834a4eaaaa498afd6a7d634524668f6b8a4693bb291d228c67f329" },
		new() { Input = 199, Expected = "0x6bff209d9a7eeeeb1f3782b55e8e46311d70ad9584b3fa878169c630b652ed1ba9a5fc6e86b2a6768192485eec61d4061e26ba3110b3eb191dd11a3bec196c76" },
		new() { Input = 200, Expected = "0xf89f667e77e4ebe01c48c9143b31a71be4780bcf61d50f2bb1a2ab8b23d2448d16257838536007cdb13953ac9f949fd27d34149a2d09fc03f9e43c39c69707d9" }
	};

	protected static readonly TestItem<int, string>[] DATA_MD2 = {
		// NOTE: Input denotes the number of bytes to take from DATA_BYTES when executing test
		new() { Input = 17, Expected = "0x5f5025f38c961ca9b7c0f03dd7410529" },
		new() { Input = 31, Expected = "0x0f61682487e2b54fed39840be0592d78" },
		new() { Input = 32, Expected = "0x3a9bbbbbb6e36d19ce4623649e4f90d1" },
		new() { Input = 33, Expected = "0x911c373ed0f5087fdfe801438d684d3c" },
		new() { Input = 34, Expected = "0x9ff77c1a799ae2f7cca7c7bc5175090c" },
		new() { Input = 63, Expected = "0x348a479fbbc1b1129e8cddc31ccaea79" },
		new() { Input = 64, Expected = "0x3380b27b8ab2d10b5a5589ae877ca64b" },
		new() { Input = 65, Expected = "0xd5b65e7f5cb45a5c4846001a9afed3ae" },
		new() { Input = 117, Expected = "0x94a4b4935e313fbc92e91ac963d66968" },
		new() { Input = 100, Expected = "0xaf995f320df97154688d71878a73550d" },
		new() { Input = 127, Expected = "0xfb5de403f13012d883a603ddc8be60fb" },
		new() { Input = 128, Expected = "0x1f59c2e552a17e7f60deca12340199ba" },
		new() { Input = 129, Expected = "0x62ab5eddb71d32f99501ec896e1325ec" },
		new() { Input = 178, Expected = "0xb3f438e163a86f153cc2124f33f96fd3" },
		new() { Input = 199, Expected = "0x8f7ba656fb867696568c4fa3cd262d63" },
		new() { Input = 200, Expected = "0x556117769860bb73130af54af2f2e11e" }
	};

	protected static readonly TestItem<int, string>[] DATA_MD4 = {
		// NOTE: Input denotes the number of bytes to take from DATA_BYTES when executing test
		new() { Input = 17, Expected = "0x09f516249b098afea9ad449f112b3a37" },
		new() { Input = 31, Expected = "0xeb6db17c1e1760e2af652efb8baf7b15" },
		new() { Input = 32, Expected = "0x2dda8ef911b2f3749d0d43ad8c63dedd" },
		new() { Input = 33, Expected = "0x5f06b2a25e3fa5bf42258467a90198a3" },
		new() { Input = 34, Expected = "0xda2aa7ba8f28b5bc0a508d95e7cbcda6" },
		new() { Input = 63, Expected = "0x16f97893e239b42ea7f1ac9bc8802d6d" },
		new() { Input = 64, Expected = "0x8c9107942d009e78eaed0b774efcd249" },
		new() { Input = 65, Expected = "0xfe8ee693928e453560fae8e06572b316" },
		new() { Input = 117, Expected = "0x2b5c503c1f318bb5c0161cdbefd16f25" },
		new() { Input = 100, Expected = "0xc1220933ed4a2eb892f736c7e32d6564" },
		new() { Input = 127, Expected = "0x53675dac941577189c270454963b7bd7" },
		new() { Input = 128, Expected = "0x467faea31ec0c4ad6a94fa57ece3da56" },
		new() { Input = 129, Expected = "0xb25a79ce4ccfac99e22066e0c53d4c82" },
		new() { Input = 178, Expected = "0x3de1d9b99dce9f17ca9d9d1ed6272ede" },
		new() { Input = 199, Expected = "0xeba1d5694b49d9bb203b9dca515c3bcf" },
		new() { Input = 200, Expected = "0xa15b8a5f14c2263a1dd9addb238bc4f9" }
	};

	protected readonly TestItem<int, string>[] DATA_MD5 = {
		// NOTE: Input denotes the number of bytes to take from DATA_BYTES when executing test
		new() { Input = 17, Expected = "0x990d2e3e54e0d540e17e28bf089cbc8f" },
		new() { Input = 31, Expected = "0x5120a7106123521029896a89890decbb" },
		new() { Input = 32, Expected = "0x49848dfebea23abc37872a22bb76e1ea" },
		new() { Input = 33, Expected = "0x62b3040c9f11e5ef68f5b029beffb3ec" },
		new() { Input = 34, Expected = "0x61c9c3ec798fdb6fc587065114a093b5" },
		new() { Input = 63, Expected = "0x89973c44bb3e207dc60d789e3b9b482b" },
		new() { Input = 64, Expected = "0xe2ae3f3eeffb99c0b46f12254ad6eb4e" },
		new() { Input = 65, Expected = "0x3c2b0369b053d0df325c7343f0a5401a" },
		new() { Input = 100, Expected = "0x98e0bd2b4eb38f4d7e6d33d1cb5fbc1d" },
		new() { Input = 117, Expected = "0x135a0450af2b16e8529060246e402a27" },
		new() { Input = 127, Expected = "0x74f3b69ddcd9d6ce64530eeef42cec35" },
		new() { Input = 128, Expected = "0xed31bf5fd4dbc2d509ac4cb880ec685d" },
		new() { Input = 129, Expected = "0x2cacdddc8999a30233627b929921202b" },
		new() { Input = 178, Expected = "0xc6f6aae119ba216edf22c62ed898bc56" },
		new() { Input = 199, Expected = "0xc5fc302e8942cb54a37a7c46adeab3d0" },
		new() { Input = 200, Expected = "0x9242480e2630061d3eccb16821e98d30" }
	};

	protected static readonly TestItem<int, string>[] DATA_PANAMA = {
		// NOTE: Input denotes the number of bytes to take from DATA_BYTES when executing test
		new() { Input = 17, Expected = "0x9e5d1af956350bb7c899bb9b0ea5db8cef53dd0b791256409d22703b0290db8e" },
		new() { Input = 31, Expected = "0x51b66d2ef4d8643de83d84488bfd93ed88457d18ff2399bcd676d70a6c308a10" },
		new() { Input = 32, Expected = "0x86f67b292bbab6f6bb708b43ed4a9fa9afb519b85d1cb14472f0208efbf25762" },
		new() { Input = 33, Expected = "0x877c54585feb0d942af19529241528d7a289976adee08eb9ad3d2ff4454778b1" },
		new() { Input = 34, Expected = "0x9c7a2d94e36050f06e19a16275f28ef9fb9feb545962593c934bf1f49986b000" },
		new() { Input = 63, Expected = "0xb51550135fe12876bd811f03cd22530067200b0ab58c38e32d4cc62388fc5e61" },
		new() { Input = 64, Expected = "0xb4c19c1a1353cc956ff663413fb2a7fcd07c23d6fff63a62637f4958b5928660" },
		new() { Input = 65, Expected = "0x4dd97dc79e0ff2b4e1fd4c9d376c3d828ea489fc4bb70e153aa75f6b03101209" },
		new() { Input = 117, Expected = "0x27573fad54ec2b8d3076ca1555535e97c151b06a81b9fc76bf2da96a5e98afd9" },
		new() { Input = 100, Expected = "0x64eb145168b0a822c6ac066548f5f6f90990fed92f7671c174c76c855aeb66ef" },
		new() { Input = 127, Expected = "0x5fcb44ce2628fca33c93f76fb2db70eab277d769e20a1bd050e6f088ee1878e0" },
		new() { Input = 128, Expected = "0x93ffd82e8f1b28ef6066874f3fdd233fe97841364fdd6a274a774db7bb0796e3" },
		new() { Input = 129, Expected = "0xdcb73c5d710e18a3dc8f0142f312a87e57071ec71a8baf923108040ecdf135b4" },
		new() { Input = 178, Expected = "0x259633707f91035c5dcde7a3121aeafcbf986780f8e918e9eed9e79f50b44669" },
		new() { Input = 199, Expected = "0xcd20a23cd1c17554f9045c4746a230bbfeb4fcc3f9440f22652910eadc0698d3" },
		new() { Input = 200, Expected = "0xdbc5ced7f203a814d985a992e0ed07d81a3d47d0dfc65493afb58ee48b0d863b" }
	};


	protected readonly TestItem<int, string>[] DATA_RADIOGATUN32 = {
		// NOTE: Input denotes the number of bytes to take from DATA_BYTES when executing test
		new() { Input = 17, Expected = "0x65024d09e2b8a46d8b6a2aa87af2445a9d640a74081e5d7a33062307a1c47b0d" },
		new() { Input = 31, Expected = "0x32b17be7c6fedb037515313b5604e1661ca1f34e282107e20d3e907864751421" },
		new() { Input = 32, Expected = "0xff4d011327d8dfccde7901523cd044fdc8c89479a831a61a8179ccb1eb6b34e7" },
		new() { Input = 33, Expected = "0x92dd5fbface846262b32ebca67a20fa571a87c435c11daacaca4cd96da4c9c2a" },
		new() { Input = 34, Expected = "0x289590b6bbe0da22917b8d62b5752c4ea032de707e753d98771da87e7a6f68d9" },
		new() { Input = 63, Expected = "0xda5cf1e7f0b880c419201aeb2f537fe27594d9e239b738f1bc677d59f2927923" },
		new() { Input = 64, Expected = "0x9b21fc33aa89b1a709c0af3b0305ee0ce491462ea34900d52f44682938f8b5ae" },
		new() { Input = 65, Expected = "0xc2856589442488830608c6d9669f1d93bdb39c83616294499b36dffba17d2bc0" },
		new() { Input = 100, Expected = "0xbbda668f9d7b3cb2729b4a6a840b48ce3f938864d41a37a8b6c1df0926923291" },
		new() { Input = 117, Expected = "0x4e99747c8623d579b13f1cc6593a83c7a363d70157ae3a83165d817e836a22d0" },
		new() { Input = 127, Expected = "0x86517f426d2c55fd69d07a434f90bfee70539cde89f024dc1ba0e52d0ba5710a" },
		new() { Input = 128, Expected = "0xbb266e01e0ba48d3c8a5f465d41dde07c67396f05011b1eee0fc8c95e11b2525" },
		new() { Input = 129, Expected = "0x9935ccf10e79f6077845f4f6ad9a41df57a8ce7d854a0899090de8140ca38b67" },
		new() { Input = 178, Expected = "0x344442532a514e9dbb4b9c4232d45558e7e38510109ef62b17f54f402885cbde" },
		new() { Input = 199, Expected = "0xb76be67d94ce6014e5a125c371c22abfce3bbccc86f92dac31c394226b0c7912" },
		new() { Input = 200, Expected = "0x16696fe96e850fce272f90b59e9114e55098c03ee0d3e40e0d0616a1926a8ed8" }
	};

	protected static readonly TestItem<int, string>[] DATA_GOST = {
		// NOTE: Input denotes the number of bytes to take from DATA_BYTES when executing test
		new() { Input = 17, Expected = "0xbe1624f6d74a04b038e317e3bb3943611e8afadd64ca0b9d2871a0f2c2a67a94" },
		new() { Input = 31, Expected = "0x593a349119478e323958dcf4dcb2d9a866087cdd2312d0fb6d8efbf0fb93ea41" },
		new() { Input = 32, Expected = "0xadd5e68dc66fc83be85ed4b5bcf497904a2132ffe5b06a2d9ded16064d711dd2" },
		new() { Input = 33, Expected = "0xf67785a257fbba2b3882e0fb71e5a269bd99c952135a4aa2440c11a0d59909fb" },
		new() { Input = 34, Expected = "0x01d9221eb3868ce57cbd9e17fa7322ad42df0330bf7d34a506b354c692aed3d7" },
		new() { Input = 63, Expected = "0x563bac0e170d99ff7517cd1bf1ac0656d754cd23adf146889c61fae20dcd4eb4" },
		new() { Input = 64, Expected = "0x2380f3405241b40f7ea6a1f8da5370fe37e7c30d317451623fd6b863d1613793" },
		new() { Input = 65, Expected = "0x1f1cbb904ed7418805100f213a5ec5a719c583db2ad9f5b762c1a307cf572e56" },
		new() { Input = 117, Expected = "0xa8043e9ddc73979e9891ac244d2122858700cca6f94590602dc318b163fd9e27" },
		new() { Input = 100, Expected = "0x9f3faed24d0b77532e67a39b9e1901eca64b5a429c288d871f264e3f35f5c2c2" },
		new() { Input = 127, Expected = "0x1fd15380a671d5a56c6924b226e9ab0464ecb26fcd21b3f440efc5d501af7d6b" },
		new() { Input = 128, Expected = "0xe0d20b403ce9b292531a65299388e46281f04b0edccba71b01a50ebe06cd589b" },
		new() { Input = 129, Expected = "0x25cd57dafc7f73be61deb7884baf970e9258cc2dc81011111d25bc712bbe55d9" },
		new() { Input = 178, Expected = "0xdc7b3cad1e054783b4a63ec3def98c2883dc2944d3efe332c422ebf436c91463" },
		new() { Input = 199, Expected = "0xb0f1aa576af8eeb0cebfd4945704eddae4031784ef8768dc530ee5b37ed8433c" },
		new() { Input = 200, Expected = "0x4c3c2005aa872b3fe3fb516c91d38c8c6da5740b52037159a2fdf04b0afb60f4" }
	};

	protected static readonly TestItem<int, string>[] DATA_GOST_3411_2012_256 = {
		// NOTE: Input denotes the number of bytes to take from DATA_BYTES when executing test
		new() { Input = 17, Expected = "0xbd425c0e2198b9e11b24094304c25c42883f5204d80b2e372f2fb17c5c84b0a4" },
		new() { Input = 31, Expected = "0x872638d0fb9274a3d10a081a508c89d2078ff8f4dff541c34904317d65fcde8a" },
		new() { Input = 32, Expected = "0x698bc0b87d736d69f5381d6db2e7ddd6e820cf2b2b590e3ed882e9ce5d5b639a" },
		new() { Input = 33, Expected = "0x2ed954fc835b019200ba6eb50c4c1399302a789b77ed18a295b8bc37df4a4cf1" },
		new() { Input = 34, Expected = "0xf19736bdf5703090ecd41bfc48a92079a4c3fc5065b5fd6885b5f0a419a23e59" },
		new() { Input = 63, Expected = "0x7b82c3434252f0477537ff26befdaa5406e1c2cf07cdf94c769dcc453833a3a9" },
		new() { Input = 64, Expected = "0x926c54dbcb9cf6e4c578b7dd212981b0ec214483ddd33553367621cd30d7446c" },
		new() { Input = 65, Expected = "0xb1c8283c46bfb09d1202b47f98b02943f660ede624b045c7e027d44736779cf0" },
		new() { Input = 117, Expected = "0x2d0007c9f8f4445738756c437bce8066830dca12be5d3e49318c9ebc79ab2795" },
		new() { Input = 100, Expected = "0x9ddedddc5862e4fb0bf8586af4d61c138d2c05b454e882cec77efa5708b373e7" },
		new() { Input = 127, Expected = "0xa73c3c56ff0dd1893c20d80d364ee465f4caad775417c8f2942c15f157f73040" },
		new() { Input = 128, Expected = "0xc3525deb3adf295ea2b4c8bbc55f4c5362c1dffa330faca02b7b0cca69db63e5" },
		new() { Input = 129, Expected = "0x24a462c684e5ddca7003de34c69a194e6a5962ec0907e2afd305047b99c487b0" },
		new() { Input = 178, Expected = "0x788f8c9df5ea1b6a2ac63b624ea9650d55274cc312ec948334964c589ea31138" },
		new() { Input = 199, Expected = "0xdc24828eb4ffa2d706e81f0db9a54c687c256ac8262f5781e6ee2e27b7380e5a" },
		new() { Input = 200, Expected = "0x83394ab99f92a8ab8b0ea2d68094c36736dbf70cad00ac2431730a6d00913360" }
	};

	protected static readonly TestItem<int, string>[] DATA_GOST_3411_2012_512 = {
		// NOTE: Input denotes the number of bytes to take from DATA_BYTES when executing test
		new() { Input = 17, Expected = "0x83a884684383fc33089029012873f965e29241d876aebf551293e702b4c132fea5e258fd089ac19b32410fc399e6a154ea0ef3a7de90e6367581dfa6710f2e2c" },
		new() { Input = 31, Expected = "0x5102dde1850e44b901eae5c313230d88cb326238d3f8e4e5682375fe9cf2a7e6df8797bbfa3557dd469fcc8499f6eccc3dbaaffec04a972cb254e8dca1bb8d15" },
		new() { Input = 32, Expected = "0x1e0a2a3c8c092e5f8239ca02091d8c498801043c545f01257a9185664e16f8acd6fce530aff6f40e1ebb7011551a6b4a794f9176d1ae0ce27e595dc47975a680" },
		new() { Input = 33, Expected = "0x056cddcfc264498692b961958f1f16b308a46b1ff920d10e872e6274971ad5ac0879e553391db9ac434bffa6897d2cd5bb26bbc2a23934fad99d7b61ca35463b" },
		new() { Input = 34, Expected = "0xa38810182a4a687be2f3a6a79ebb810b861fb1f613dbb301e9a433f6c36a93a7eb95c3ddba90226ff97fd7e2f05601e140c60bba26a253364b37403193fc3bc5" },
		new() { Input = 63, Expected = "0xe45dbea75dcda099ef6466d2fb2570bf0a4941dde19bc8980dcb281a0e9b56575096d7a58282b29ea4143e5970c1d20ff5527fc4bb748ef3c5ccd2373ffc031a" },
		new() { Input = 64, Expected = "0xa9b4fa08248fab25fa89d82d68b8322b44d4729ffacaa515da210378c50b03bdd1110d47b2d0bebd0fe41e0702e9a71d958a786c1e2d62d3328c7e3f1d98649c" },
		new() { Input = 65, Expected = "0x68a46f580c2b5b80e05b1150fc5e79a81ad5bdeed492b29ac6684c2171959c58643fb6f83b84f3e9cae7c9baec93f7c078a95a8b539ae7136c674fa566330423" },
		new() { Input = 117, Expected = "0x3faf6282aaadffcf32577c25bb7b77ffa81be6ac8ad8abbfc67eeb2b54690ddb14802bd17afe324dac2678ffa41246e559c0136f36e371ad5017cc1776cda553" },
		new() { Input = 100, Expected = "0x9613f8940055005d0408df5799eafb1252a2eb27c3e119e53450268192047862aeb301615a16ae4ac0538eb61e2edbb0d603c56018e1f57bb1599564cd50d52e" },
		new() { Input = 127, Expected = "0x044f5f254b0189082de63bb25140191b2e1ac91033c84b455e264969a54f3886d2e0b431fbaa446c1cefa70ff9dc58288fae236b1db88b6953e2da011fbc6365" },
		new() { Input = 128, Expected = "0xb9a84a87bce0fb8e094bdec66ac75c350d45d3e6e3bbdfcc5e4718e215b8e33a2865d5046f63c8cb56d10a47fefece1986585f7b8c7b06082887da868ad0dec7" },
		new() { Input = 129, Expected = "0xed6a10af5c1d497ea4ce27e36be4a2054f6ca1b55533749a29872a693a744e4e71729159612494281668728bb5a553457f534be107ead89ab76a21e6eb7b5aa7" },
		new() { Input = 178, Expected = "0xa34b76d2709e7da134c24cbca49f520e0b0cf57d29bb27c3be0cb15420be37669cbe04bc2d77a001e79477c9233b3ee8436cf91fa929fefdb9cd6d98eb66c0f0" },
		new() { Input = 199, Expected = "0x94ba065ef1953d69c9c739b07f8df76147f8c2c49c7976deb47abb33419249f30736ab70ca89fbeb8d5c85071e78403251c5e022239575b503c831c3841f28ea" },
		new() { Input = 200, Expected = "0x1f68356adc79e5a5708de190af0126f829d1a886336700a3ab5037944a600b4dce972049651d30f8206038b844bf4274582ae2ba5794446d49146c2e6d9559a4" }
	};


	protected readonly TestItem<int, string>[] DATA_WHIRLPOOL = {
		// NOTE: Input denotes the number of bytes to take from DATA_BYTES when executing test
		new() { Input = 17, Expected = "0xeb986421c1650306056d522a52f2ab6aec30a7fbd930dff6927e9ca6db63501c999102e1fc594a476ac7ec3b6dffb1bd5f3e69ed0f175216d923798e32cb8096" },
		new() { Input = 31, Expected = "0x8ec8f6838a7f78f9a1104a15e6e51f690b8bfe69e412438a6591dd90ff1bdee732ee32b75eda9d679900081a17e10d1dec77fdaa109a6ede060bbf3fa7959a8b" },
		new() { Input = 32, Expected = "0x5687a34495d2ebe57ee157fb0eb4c9674079d6ce97d70a091abfb92fb0096f2065197ea7379bbbfcb10a148beec4381bf2dd3662bcaeb9077a014d5d51acff7b" },
		new() { Input = 33, Expected = "0x017cf76d956e88528f0d1f48dbf895c645f0d9a7269ea21df15da6e24e15d711edbf88f0a6872c2074afb0f5c2905291395862b02e06019ffea960aa92ae7f98" },
		new() { Input = 34, Expected = "0xc5ef49c4ba2aadecadd8820034378e53174d66b6bee6583ea3d36dde0ebe652be2571f9c5713e38e98f433817b3cfd4d633e3cbf62e6091943ed241c0b8cae37" },
		new() { Input = 63, Expected = "0x9d3afbcb5b7bb86e27378090dc4664abc46f87bd69dbdf2481a5a1c25ebc216eeae5bd9a900f996d1fe8749c7127986602bd1221b73ea7c3cebcfd2fcf529773" },
		new() { Input = 64, Expected = "0x0a2cf63dfda157514c4d9a54198265b7d09100922c8a6431d2b29b62c74f0ad7a0b0c661005aa686d5e2cbb5cab76563ee883bcbe52a4f4f32f2852ce3793b4c" },
		new() { Input = 65, Expected = "0xfe6b3567fbbb1f1490d263248ef4f8ee7136a0c7627abb229c98fb90bd91710a15f135dffe1ab84a31984b3cc4869e870e64168efead9b8921a6139cd84b387f" },
		new() { Input = 100, Expected = "0xc95fb60f44a4eeb27cab9718ec3e3e6bdcd4bc3e2e59124f64defceeb17acf90121b65bf4693ae094e76f0db8d6f309a8531a474b53f49d5c4a7686fb9261d4f" },
		new() { Input = 117, Expected = "0xdff715603eaff8b2cfd3e0aa49ee50b0afdfa445e4f4b4a2b148959c4b23c6594bf8e2c81228db3c57c147e3b8a2fb91763b9a7abc0bff48052c30a9117d6b04" },
		new() { Input = 127, Expected = "0x76a8e2c8f91308134eb2a6485f4c8b1ed186632f5d4a477d5e2bd591c1a5913f39c97baf4a89ec56d0b46de38e72df6d43a0e8101f65e1441b415e4200cbe313" },
		new() { Input = 128, Expected = "0x661dc7ddbc9cd25ce94dfba19b7941daf12ff9a0a9d1b151d691ace392ed9d6c8d8cd1c12b2f0fda9ea116291cf81f04aca12f40fa2c482976228eb703d64029" },
		new() { Input = 129, Expected = "0xa9cf1d955a634da1f5b1068d1a0d631948ccd947c2e44eaf20584a79a810070bc3d30a208d63c023146d8bff79571ae6a9d10c90baf3e0031a733016f4473356" },
		new() { Input = 178, Expected = "0x53ca52a4baa75c13ef909fb6f6ec680338902bda1269c6a7db456c187a40f5e9e0dfce6f3d151e3b533f1b18c0b35a955095b24c94bd75a69bca5c67720c8e24" },
		new() { Input = 199, Expected = "0x26c9f7bed820bef29e35521bb6e89ccba04ad473eb7f8d9e51952ed4d414b71da10e57fc2d30ac8d6405722af51456bb515553a9fa9108cd022d270b9fda6ffe" },
		new() { Input = 200, Expected = "0xd88dceef0776780b8439dd8338cd972734d6e973b4dc43b6d298622d9ed0a1ab3e9a37664ecbd14d4155c65cde93dcfd70707ba4dd7eecbf15af2a5ffee48d1e" }
	};

	#endregion

}
