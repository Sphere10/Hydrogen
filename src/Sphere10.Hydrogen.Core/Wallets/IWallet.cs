using System;
using System.Collections.Generic;
using System.Text;
using Sphere10.Hydrogen.Core.Keys;

namespace Sphere10.Hydrogen.Core.Wallets {

	public enum WalletKeyCapability {
		None = 0,
		CanWatch = 1,
		CanSign
	}

	public class WalletKey {
		public KeyType KeyType { get; set; }
		public byte[] PrivateKey { get; set; }
		public byte[] PublicKey { get; set; }
		public bool HasPrivateKey => PrivateKey != null;
	}

	public interface IWallet {

		void AddPrivateKey(byte[] privateKey);

		void AddWatchOnlyPublicKey(byte[] publicKey);

		WalletKeyCapability ExamineKey(byte[] publicKey);

		IEnumerable<WalletKey> Keys { get; }

		void ChangePassword(string oldPassword, string newPassword);

		void Lock();

		void Unlock(string password);

	}


	
}
