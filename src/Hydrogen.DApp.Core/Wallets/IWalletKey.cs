// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using Hydrogen.DApp.Core.Keys;

namespace Hydrogen.DApp.Core.Wallets;

public interface IWalletKey {
	public string Name { get; set; }
	public KeyType KeyType { get; }
	public SecureBytes PrivateKey { get; }
	public byte[] PublicKey { get; }
	public WalletKeyCapability Capability { get; }
}
