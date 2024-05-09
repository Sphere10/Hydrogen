// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using Hydrogen.CryptoEx.EC;
using Hydrogen.CryptoEx.EC.Schnorr;
using static HashLib4CSharp.Base.HashFactory.Crypto;

namespace Hydrogen.CryptoEx;

public class ModuleConfiguration : CoreModuleConfigurationBase {


	public override void OnInitialize(IServiceProvider serviceProvider) {
		base.OnInitialize(serviceProvider);
		InitializeInternal();
	}

	public override void OnFinalize(IServiceProvider serviceProvider) {
		base.OnFinalize(serviceProvider);
		FinalizeInternal();
	}

	public static void InitializeInternal() {
		Signers.Register(DSS.ECDSA_SECP256k1, () => new ECDSA(ECDSAKeyType.SECP256K1));
		Signers.Register(DSS.ECDSA_SECP384R1, () => new ECDSA(ECDSAKeyType.SECP384R1));
		Signers.Register(DSS.ECDSA_SECP521R1, () => new ECDSA(ECDSAKeyType.SECP521R1));
		Signers.Register(DSS.ECDSA_SECT283K1, () => new ECDSA(ECDSAKeyType.SECT283K1));
		Signers.Register(DSS.SCHNORR_SECP256k1, () => new Schnorr(ECDSAKeyType.SECP256K1));
		Signers.Register(DSS.SCHNORR_SECP384R1, () => new Schnorr(ECDSAKeyType.SECP384R1));
		Signers.Register(DSS.SCHNORR_SECP521R1, () => new Schnorr(ECDSAKeyType.SECP521R1));
		Signers.Register(DSS.SCHNORR_SECT283K1, () => new Schnorr(ECDSAKeyType.SECT283K1));

		if (Tools.Runtime.IsWasmExecutable()) {
			Hashers.Register(CHF.SHA2_512, () => new HashLibAdapter(CreateSHA2_512()));
			Hashers.Register(CHF.SHA2_384, () => new HashLibAdapter(CreateSHA2_384()));
			Hashers.Register(CHF.SHA2_256, () => new HashLibAdapter(CreateSHA2_256()));
			Hashers.Register(CHF.SHA1_160, () => new HashLibAdapter(CreateSHA1()));
		}

		Hashers.Register(CHF.RIPEMD, () => new HashLibAdapter(CreateRIPEMD()));
		Hashers.Register(CHF.RIPEMD_128, () => new HashLibAdapter(CreateRIPEMD128()));
		Hashers.Register(CHF.RIPEMD_160, () => new HashLibAdapter(CreateRIPEMD160()));
		Hashers.Register(CHF.RIPEMD_256, () => new HashLibAdapter(CreateRIPEMD256()));
		Hashers.Register(CHF.RIPEMD_320, () => new HashLibAdapter(CreateRIPEMD320()));
		Hashers.Register(CHF.Blake2b_512, () => new HashLibAdapter(CreateBlake2B_512()));
		Hashers.Register(CHF.Blake2b_384, () => new HashLibAdapter(CreateBlake2B_384()));
		Hashers.Register(CHF.Blake2b_256, () => new HashLibAdapter(CreateBlake2B_256()));
		Hashers.Register(CHF.Blake2b_224, () => new HashLibAdapter(CreateBlake2B_224()));
		Hashers.Register(CHF.Blake2b_160, () => new HashLibAdapter(CreateBlake2B_160()));
		Hashers.Register(CHF.Blake2b_128, () => new HashLibAdapter(CreateBlake2B_128()));
		Hashers.Register(CHF.Blake2s_256, () => new HashLibAdapter(CreateBlake2S_256()));
		Hashers.Register(CHF.Blake2s_224, () => new HashLibAdapter(CreateBlake2S_224()));
		Hashers.Register(CHF.Blake2s_160, () => new HashLibAdapter(CreateBlake2S_160()));
		Hashers.Register(CHF.Blake2s_128, () => new HashLibAdapter(CreateBlake2S_128()));
		Hashers.Register(CHF.Blake2b_512_Fast, () => new Blake2bFastAdapter(64));
		Hashers.Register(CHF.Blake2b_384_Fast, () => new Blake2bFastAdapter(48));
		Hashers.Register(CHF.Blake2b_256_Fast, () => new Blake2bFastAdapter(32));
		Hashers.Register(CHF.Blake2b_128_Fast, () => new Blake2bFastAdapter(16));
		Hashers.Register(CHF.Blake2b_224_Fast, () => new Blake2bFastAdapter(28));
		Hashers.Register(CHF.Blake2b_160_Fast, () => new Blake2bFastAdapter(20));
		Hashers.Register(CHF.Blake2s_256_Fast, () => new Blake2sFastAdapter(32));
		Hashers.Register(CHF.Blake2s_224_Fast, () => new Blake2sFastAdapter(28));
		Hashers.Register(CHF.Blake2s_160_Fast, () => new Blake2sFastAdapter(20));
		Hashers.Register(CHF.Blake2s_128_Fast, () => new Blake2sFastAdapter(16));
		Hashers.Register(CHF.Gost, () => new HashLibAdapter(CreateGost()));
		Hashers.Register(CHF.Gost3411_2012_256, () => new HashLibAdapter(CreateGOST3411_2012_256()));
		Hashers.Register(CHF.Gost3411_2012_512, () => new HashLibAdapter(CreateGOST3411_2012_512()));
		Hashers.Register(CHF.Grindahl256, () => new HashLibAdapter(CreateGrindahl256()));
		Hashers.Register(CHF.Grindahl512, () => new HashLibAdapter(CreateGrindahl512()));
		Hashers.Register(CHF.Has160, () => new HashLibAdapter(CreateHAS160()));
		Hashers.Register(CHF.Haval_3_128, () => new HashLibAdapter(CreateHaval_3_128()));
		Hashers.Register(CHF.Haval_3_160, () => new HashLibAdapter(CreateHaval_3_160()));
		Hashers.Register(CHF.Haval_3_192, () => new HashLibAdapter(CreateHaval_3_192()));
		Hashers.Register(CHF.Haval_3_224, () => new HashLibAdapter(CreateHaval_3_224()));
		Hashers.Register(CHF.Haval_3_256, () => new HashLibAdapter(CreateHaval_3_256()));
		Hashers.Register(CHF.Haval_4_128, () => new HashLibAdapter(CreateHaval_4_128()));
		Hashers.Register(CHF.Haval_4_160, () => new HashLibAdapter(CreateHaval_4_160()));
		Hashers.Register(CHF.Haval_4_192, () => new HashLibAdapter(CreateHaval_4_192()));
		Hashers.Register(CHF.Haval_4_224, () => new HashLibAdapter(CreateHaval_4_224()));
		Hashers.Register(CHF.Haval_4_256, () => new HashLibAdapter(CreateHaval_4_256()));
		Hashers.Register(CHF.Haval_5_128, () => new HashLibAdapter(CreateHaval_5_128()));
		Hashers.Register(CHF.Haval_5_160, () => new HashLibAdapter(CreateHaval_5_160()));
		Hashers.Register(CHF.Haval_5_192, () => new HashLibAdapter(CreateHaval_5_192()));
		Hashers.Register(CHF.Haval_5_224, () => new HashLibAdapter(CreateHaval_5_224()));
		Hashers.Register(CHF.Haval_5_256, () => new HashLibAdapter(CreateHaval_5_256()));
		Hashers.Register(CHF.Keccak_224, () => new HashLibAdapter(CreateKeccak_224()));
		Hashers.Register(CHF.Keccak_256, () => new HashLibAdapter(CreateKeccak_256()));
		Hashers.Register(CHF.Keccak_288, () => new HashLibAdapter(CreateKeccak_288()));
		Hashers.Register(CHF.Keccak_384, () => new HashLibAdapter(CreateKeccak_384()));
		Hashers.Register(CHF.Keccak_512, () => new HashLibAdapter(CreateKeccak_512()));
		Hashers.Register(CHF.MD2, () => new HashLibAdapter(CreateMD2()));
		Hashers.Register(CHF.MD4, () => new HashLibAdapter(CreateMD4()));
		Hashers.Register(CHF.MD5, () => new HashLibAdapter(CreateMD5()));
		Hashers.Register(CHF.SHA0, () => new HashLibAdapter(CreateSHA0()));
		Hashers.Register(CHF.SHA2_224, () => new HashLibAdapter(CreateSHA2_224()));
		Hashers.Register(CHF.SHA2_512_224, () => new HashLibAdapter(CreateSHA2_512_224()));
		Hashers.Register(CHF.SHA2_512_256, () => new HashLibAdapter(CreateSHA2_512_256()));
		Hashers.Register(CHF.SHA3_224, () => new HashLibAdapter(CreateSHA3_224()));
		Hashers.Register(CHF.SHA3_256, () => new HashLibAdapter(CreateSHA3_256()));
		Hashers.Register(CHF.SHA3_384, () => new HashLibAdapter(CreateSHA3_384()));
		Hashers.Register(CHF.SHA3_512, () => new HashLibAdapter(CreateSHA3_512()));
		Hashers.Register(CHF.Snefru_8_128, () => new HashLibAdapter(CreateSnefru_8_128()));
		Hashers.Register(CHF.Snefru_8_256, () => new HashLibAdapter(CreateSnefru_8_256()));
		Hashers.Register(CHF.Tiger_3_128, () => new HashLibAdapter(CreateTiger_3_128()));
		Hashers.Register(CHF.Tiger_3_160, () => new HashLibAdapter(CreateTiger_3_160()));
		Hashers.Register(CHF.Tiger_3_192, () => new HashLibAdapter(CreateTiger_3_192()));
		Hashers.Register(CHF.Tiger_4_128, () => new HashLibAdapter(CreateTiger_4_128()));
		Hashers.Register(CHF.Tiger_4_160, () => new HashLibAdapter(CreateTiger_4_160()));
		Hashers.Register(CHF.Tiger_4_192, () => new HashLibAdapter(CreateTiger_4_192()));
		Hashers.Register(CHF.Tiger_5_128, () => new HashLibAdapter(CreateTiger_5_128()));
		Hashers.Register(CHF.Tiger_5_160, () => new HashLibAdapter(CreateTiger_5_160()));
		Hashers.Register(CHF.Tiger_5_192, () => new HashLibAdapter(CreateTiger_5_192()));
		Hashers.Register(CHF.Tiger2_3_128, () => new HashLibAdapter(CreateTiger2_3_128()));
		Hashers.Register(CHF.Tiger2_3_160, () => new HashLibAdapter(CreateTiger2_3_160()));
		Hashers.Register(CHF.Tiger2_3_192, () => new HashLibAdapter(CreateTiger2_3_192()));
		Hashers.Register(CHF.Tiger2_4_128, () => new HashLibAdapter(CreateTiger2_4_128()));
		Hashers.Register(CHF.Tiger2_4_160, () => new HashLibAdapter(CreateTiger2_4_160()));
		Hashers.Register(CHF.Tiger2_4_192, () => new HashLibAdapter(CreateTiger2_4_192()));
		Hashers.Register(CHF.Tiger2_5_128, () => new HashLibAdapter(CreateTiger2_5_128()));
		Hashers.Register(CHF.Tiger2_5_160, () => new HashLibAdapter(CreateTiger2_5_160()));
		Hashers.Register(CHF.Tiger2_5_192, () => new HashLibAdapter(CreateTiger2_5_192()));
		Hashers.Register(CHF.WhirlPool, () => new HashLibAdapter(CreateWhirlPool()));
		Hashers.Register(CHF.Panama, () => new HashLibAdapter(CreatePanama()));
		Hashers.Register(CHF.RadioGatun32, () => new HashLibAdapter(CreateRadioGatun32()));
	}

	public static void FinalizeInternal() {
	}
}
