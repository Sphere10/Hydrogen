// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Hamish Rose
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen.DApp.Presentation2.UI.Wizard.Examples.NewWallet {

	public class NewWalletModel {
		public WalletType Type { get; set; } = WalletType.Standard;

		public string Password { get; set; }

		public string Name { get; set; } = "wallet_1";

		public string Seed { get; set; }
	}


	public enum WalletType {
		Standard,
		Restore
	}

}
