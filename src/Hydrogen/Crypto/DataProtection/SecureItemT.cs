using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;

namespace Hydrogen {

	public sealed class SecureItem<T> : ISecureItem<T> {
		private SecureBytes _secureBytes;
		private IItemSerializer<T> _serializer;
		private T _unencrypted;

		public SecureItem(T item, IItemSerializer<T> serializer) {
			Guard.ArgumentNotNull(item, nameof(item));
			Guard.ArgumentNotNull(serializer, nameof(serializer));
			_serializer = serializer;
			_secureBytes = new SecureBytes(_serializer.SerializeLE(item));
			_secureBytes.Encrypted += () => {
				_unencrypted = default;
			};
			_secureBytes.Decrypted += () => {
				_unencrypted = _serializer.DeserializeLE(_secureBytes.Item);
			};
		}

		public bool Protected => _secureBytes.Protected;
			
		public T Item {
			get {
				if (Protected)
					throw new InvalidOperationException("No open scope is present, bytes are encrypted");
				return _unencrypted;
			}
		}

		public IScope EnterUnprotectedScope() {
			var scope =_secureBytes.EnterUnprotectedScope();
			scope.ScopeEnd += () => {

			};
			return scope;
		}

		public void Dispose() {
			_secureBytes.Dispose();
		}

	
	}
}
