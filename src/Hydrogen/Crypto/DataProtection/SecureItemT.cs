// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

public sealed class SecureItem<T> : ISecureItem<T> {
	private SecureBytes _secureBytes;
	private IItemSerializer<T> _serializer;
	private T _unencrypted;

	public SecureItem(T item, IItemSerializer<T> serializer) {
		Guard.ArgumentNotNull(item, nameof(item));
		Guard.ArgumentNotNull(serializer, nameof(serializer));
		_serializer = serializer;
		_secureBytes = new SecureBytes(_serializer.SerializeBytesLE(item));
		_secureBytes.Encrypted += () => { _unencrypted = default; };
		_secureBytes.Decrypted += () => { _unencrypted = _serializer.DeserializeBytesLE(_secureBytes.Item); };
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
		var scope = _secureBytes.EnterUnprotectedScope();
		scope.ScopeEnd += () => { };
		return scope;
	}

	public void Dispose() {
		_secureBytes.Dispose();
	}


}
