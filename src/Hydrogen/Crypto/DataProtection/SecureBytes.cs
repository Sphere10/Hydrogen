using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;

namespace Hydrogen {

	public sealed class SecureBytes : ISecureItem<byte[]> {
		internal EventHandlerEx Encrypted;
		internal EventHandlerEx Decrypted;

		private byte[] _unencrypted;
		private SecureString _holder;
		private Synchronized<int> _scopes;

		public SecureBytes(byte[] sensitiveBytes) {
			Guard.ArgumentNotNull(sensitiveBytes, nameof(sensitiveBytes));
			_holder = new SecureString();
			_unencrypted = sensitiveBytes;
			_scopes = new Synchronized<int>(0);
			for (var i = 0; i < _unencrypted.Length; i++)
				_holder.AppendChar((char)_unencrypted[i]);
			Encrypt();
		}

		public bool Protected {
			get {
				using (_scopes.EnterReadScope()) {
					return _scopes.Value > 0;
				}
			}
		}

		public byte[] Item {
			get {
				if (Protected)
					throw new InvalidOperationException("No open scope is present, bytes are encrypted");
				return _unencrypted;
			}
		}

		public IScope EnterUnprotectedScope() {
			using (_scopes.EnterWriteScope()) {
				if (_scopes.Value++ == 0)
					Decrypt();
				return new ActionScope(() => {
					using (_scopes.EnterWriteScope()) {
						if (_scopes.Value-- == 0)
							Encrypt();

					}
				});
			}
		}

		public void Dispose() {
			_holder.Dispose();
		}

		private void Encrypt() {
			Tools.Crypto.SecureErase(_unencrypted);
			Encrypted?.Invoke();
		}

		private void Decrypt() {
			var unencryptedString = _holder.ToString();  // WARNING: immutable unencrypted string potentially unsafe until collected
			for (var i = 0; i < unencryptedString.Length; i++)
				_unencrypted[i] = (byte)unencryptedString[i];
			unencryptedString = null;
			Decrypted?.Invoke();
			GC.Collect();
		}
	
	}
}
