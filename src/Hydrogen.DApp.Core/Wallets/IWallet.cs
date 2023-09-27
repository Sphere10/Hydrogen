// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;

namespace Hydrogen.DApp.Core.Wallets;

public interface IWallet {
	bool IsLocked { get; }

	void Unlock(string password);

	void Lock();

	void ChangePassword(string newPassword);

	void AddPrivateKey(string name, byte[] privateKey);

	void AddWatchOnlyPublicKey(string name, byte[] publicKey);

	WalletKeyCapability ClassifyKey(byte[] publicKey);

	IEnumerable<IWalletKey> Keys { get; }


}
