using System;
using System.Collections.Generic;
using System.Text;

namespace Hydrogen.DApp.Core.Wallets {

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


	
}
