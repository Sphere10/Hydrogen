//-----------------------------------------------------------------------
// <copyright file="ModuleConfiguration.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (http://www.sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// <author>Herman Schoenfeld</author>
// <date>2018</date>
// </copyright>
//-----------------------------------------------------------------------

using static HashLib4CSharp.Base.HashFactory.Adapter;
using static HashLib4CSharp.Base.HashFactory.Crypto;

namespace Sphere10.Framework.CryptoEx {
	public static class ModuleConfiguration  {

		public static void Initialize() {
			Hashers.Register(CHF.RIPEMD, () => new HashAlgorithmAdapter(CreateHashAlgorithmFromHash(CreateRIPEMD())));
			Hashers.Register(CHF.RIPEMD_128, () => new HashAlgorithmAdapter(CreateHashAlgorithmFromHash(CreateRIPEMD128())));
			Hashers.Register(CHF.RIPEMD_160, () => new HashAlgorithmAdapter(CreateHashAlgorithmFromHash(CreateRIPEMD160())));
			Hashers.Register(CHF.RIPEMD_256, () => new HashAlgorithmAdapter(CreateHashAlgorithmFromHash(CreateRIPEMD256())));
			Hashers.Register(CHF.RIPEMD_320, () => new HashAlgorithmAdapter(CreateHashAlgorithmFromHash(CreateRIPEMD320())));
			Hashers.Register(CHF.Blake2b_384, () => new HashAlgorithmAdapter(CreateHashAlgorithmFromHash(CreateBlake2B_384())));
			Hashers.Register(CHF.Blake2b_512, () => new HashAlgorithmAdapter(CreateHashAlgorithmFromHash(CreateBlake2B_512())));
			Hashers.Register(CHF.Blake2s_128, () => new HashAlgorithmAdapter(CreateHashAlgorithmFromHash(CreateBlake2S_128())));
			Hashers.Register(CHF.Blake2s_160, () => new HashAlgorithmAdapter(CreateHashAlgorithmFromHash(CreateBlake2S_160())));
			Hashers.Register(CHF.Blake2s_224, () => new HashAlgorithmAdapter(CreateHashAlgorithmFromHash(CreateBlake2S_224())));
			Hashers.Register(CHF.Blake2s_256, () => new HashAlgorithmAdapter(CreateHashAlgorithmFromHash(CreateBlake2S_256())));
			Hashers.Register(CHF.Gost, () => new HashAlgorithmAdapter(CreateHashAlgorithmFromHash(CreateGost())));
			Hashers.Register(CHF.Gost3411_2012_256, () => new HashAlgorithmAdapter(CreateHashAlgorithmFromHash(CreateGOST3411_2012_256())));
			Hashers.Register(CHF.Gost3411_2012_512, () => new HashAlgorithmAdapter(CreateHashAlgorithmFromHash(CreateGOST3411_2012_512())));
			Hashers.Register(CHF.Grindahl256, () => new HashAlgorithmAdapter(CreateHashAlgorithmFromHash(CreateGrindahl256())));
			Hashers.Register(CHF.Grindahl512, () => new HashAlgorithmAdapter(CreateHashAlgorithmFromHash(CreateGrindahl512())));
			Hashers.Register(CHF.Has160, () => new HashAlgorithmAdapter(CreateHashAlgorithmFromHash(CreateHAS160())));
			Hashers.Register(CHF.Haval_3_128, () => new HashAlgorithmAdapter(CreateHashAlgorithmFromHash(CreateHaval_3_128())));
			Hashers.Register(CHF.Haval_3_160, () => new HashAlgorithmAdapter(CreateHashAlgorithmFromHash(CreateHaval_3_160())));
			Hashers.Register(CHF.Haval_3_192, () => new HashAlgorithmAdapter(CreateHashAlgorithmFromHash(CreateHaval_3_192())));
			Hashers.Register(CHF.Haval_3_224, () => new HashAlgorithmAdapter(CreateHashAlgorithmFromHash(CreateHaval_3_224())));
			Hashers.Register(CHF.Haval_3_256, () => new HashAlgorithmAdapter(CreateHashAlgorithmFromHash(CreateHaval_3_256())));
			Hashers.Register(CHF.Haval_4_128, () => new HashAlgorithmAdapter(CreateHashAlgorithmFromHash(CreateHaval_4_128())));
			Hashers.Register(CHF.Haval_4_160, () => new HashAlgorithmAdapter(CreateHashAlgorithmFromHash(CreateHaval_4_160())));
			Hashers.Register(CHF.Haval_4_192, () => new HashAlgorithmAdapter(CreateHashAlgorithmFromHash(CreateHaval_4_192())));
			Hashers.Register(CHF.Haval_4_224, () => new HashAlgorithmAdapter(CreateHashAlgorithmFromHash(CreateHaval_4_224())));
			Hashers.Register(CHF.Haval_4_256, () => new HashAlgorithmAdapter(CreateHashAlgorithmFromHash(CreateHaval_4_256())));
			Hashers.Register(CHF.Haval_5_128, () => new HashAlgorithmAdapter(CreateHashAlgorithmFromHash(CreateHaval_5_128())));
			Hashers.Register(CHF.Haval_5_160, () => new HashAlgorithmAdapter(CreateHashAlgorithmFromHash(CreateHaval_5_160())));
			Hashers.Register(CHF.Haval_5_192, () => new HashAlgorithmAdapter(CreateHashAlgorithmFromHash(CreateHaval_5_192())));
			Hashers.Register(CHF.Haval_5_224, () => new HashAlgorithmAdapter(CreateHashAlgorithmFromHash(CreateHaval_5_224())));
			Hashers.Register(CHF.Haval_5_256, () => new HashAlgorithmAdapter(CreateHashAlgorithmFromHash(CreateHaval_5_256())));
			Hashers.Register(CHF.Keccak_224, () => new HashAlgorithmAdapter(CreateHashAlgorithmFromHash(CreateKeccak_224())));
			Hashers.Register(CHF.Keccak_256, () => new HashAlgorithmAdapter(CreateHashAlgorithmFromHash(CreateKeccak_256())));
			Hashers.Register(CHF.Keccak_288, () => new HashAlgorithmAdapter(CreateHashAlgorithmFromHash(CreateKeccak_288())));
			Hashers.Register(CHF.Keccak_384, () => new HashAlgorithmAdapter(CreateHashAlgorithmFromHash(CreateKeccak_384())));
			Hashers.Register(CHF.Keccak_512, () => new HashAlgorithmAdapter(CreateHashAlgorithmFromHash(CreateKeccak_512())));
			Hashers.Register(CHF.MD2, () => new HashAlgorithmAdapter(CreateHashAlgorithmFromHash(CreateMD2())));
			Hashers.Register(CHF.MD4, () => new HashAlgorithmAdapter(CreateHashAlgorithmFromHash(CreateMD4())));
			Hashers.Register(CHF.MD5, () => new HashAlgorithmAdapter(CreateHashAlgorithmFromHash(CreateMD5())));
			Hashers.Register(CHF.SHA0, () => new HashAlgorithmAdapter(CreateHashAlgorithmFromHash(CreateSHA0())));
			Hashers.Register(CHF.SHA2_224, () => new HashAlgorithmAdapter(CreateHashAlgorithmFromHash(CreateSHA2_224())));
			Hashers.Register(CHF.SHA2_512_224, () => new HashAlgorithmAdapter(CreateHashAlgorithmFromHash(CreateSHA2_512_224())));
			Hashers.Register(CHF.SHA2_512_256, () => new HashAlgorithmAdapter(CreateHashAlgorithmFromHash(CreateSHA2_512_256())));
			Hashers.Register(CHF.SHA3_224, () => new HashAlgorithmAdapter(CreateHashAlgorithmFromHash(CreateSHA3_224())));
			Hashers.Register(CHF.SHA3_256, () => new HashAlgorithmAdapter(CreateHashAlgorithmFromHash(CreateSHA3_256())));
			Hashers.Register(CHF.SHA3_384, () => new HashAlgorithmAdapter(CreateHashAlgorithmFromHash(CreateSHA3_384())));
			Hashers.Register(CHF.SHA3_512, () => new HashAlgorithmAdapter(CreateHashAlgorithmFromHash(CreateSHA3_512())));
			Hashers.Register(CHF.Snefru_8_128, () => new HashAlgorithmAdapter(CreateHashAlgorithmFromHash(CreateSnefru_8_128())));
			Hashers.Register(CHF.Snefru_8_256, () => new HashAlgorithmAdapter(CreateHashAlgorithmFromHash(CreateSnefru_8_256())));
			Hashers.Register(CHF.Tiger_3_128, () => new HashAlgorithmAdapter(CreateHashAlgorithmFromHash(CreateTiger_3_128())));
			Hashers.Register(CHF.Tiger_3_160, () => new HashAlgorithmAdapter(CreateHashAlgorithmFromHash(CreateTiger_3_160())));
			Hashers.Register(CHF.Tiger_3_192, () => new HashAlgorithmAdapter(CreateHashAlgorithmFromHash(CreateTiger_3_192())));
			Hashers.Register(CHF.Tiger_4_128, () => new HashAlgorithmAdapter(CreateHashAlgorithmFromHash(CreateTiger_4_128())));
			Hashers.Register(CHF.Tiger_4_160, () => new HashAlgorithmAdapter(CreateHashAlgorithmFromHash(CreateTiger_4_160())));
			Hashers.Register(CHF.Tiger_4_192, () => new HashAlgorithmAdapter(CreateHashAlgorithmFromHash(CreateTiger_4_192())));
			Hashers.Register(CHF.Tiger_5_128, () => new HashAlgorithmAdapter(CreateHashAlgorithmFromHash(CreateTiger_5_128())));
			Hashers.Register(CHF.Tiger_5_160, () => new HashAlgorithmAdapter(CreateHashAlgorithmFromHash(CreateTiger_5_160())));
			Hashers.Register(CHF.Tiger_5_192, () => new HashAlgorithmAdapter(CreateHashAlgorithmFromHash(CreateTiger_5_192())));
			Hashers.Register(CHF.Tiger2_3_128, () => new HashAlgorithmAdapter(CreateHashAlgorithmFromHash(CreateTiger2_3_128())));
			Hashers.Register(CHF.Tiger2_3_160, () => new HashAlgorithmAdapter(CreateHashAlgorithmFromHash(CreateTiger2_3_160())));
			Hashers.Register(CHF.Tiger2_3_192, () => new HashAlgorithmAdapter(CreateHashAlgorithmFromHash(CreateTiger2_3_192())));
			Hashers.Register(CHF.Tiger2_4_128, () => new HashAlgorithmAdapter(CreateHashAlgorithmFromHash(CreateTiger2_4_128())));
			Hashers.Register(CHF.Tiger2_4_160, () => new HashAlgorithmAdapter(CreateHashAlgorithmFromHash(CreateTiger2_4_160())));
			Hashers.Register(CHF.Tiger2_4_192, () => new HashAlgorithmAdapter(CreateHashAlgorithmFromHash(CreateTiger2_4_192())));
			Hashers.Register(CHF.Tiger2_5_128, () => new HashAlgorithmAdapter(CreateHashAlgorithmFromHash(CreateTiger2_5_128())));
			Hashers.Register(CHF.Tiger2_5_160, () => new HashAlgorithmAdapter(CreateHashAlgorithmFromHash(CreateTiger2_5_160())));
			Hashers.Register(CHF.Tiger2_5_192, () => new HashAlgorithmAdapter(CreateHashAlgorithmFromHash(CreateTiger2_5_192())));
			Hashers.Register(CHF.WhirlPool, () => new HashAlgorithmAdapter(CreateHashAlgorithmFromHash(CreateWhirlPool())));
			Hashers.Register(CHF.Panama, () => new HashAlgorithmAdapter(CreateHashAlgorithmFromHash(CreatePanama())));
			Hashers.Register(CHF.RadioGatun32, () => new HashAlgorithmAdapter(CreateHashAlgorithmFromHash(CreateRadioGatun32())));

		}

		public static void Finalize() {

		}
		
	}
}
