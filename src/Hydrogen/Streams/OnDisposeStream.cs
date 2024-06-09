// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.IO;

namespace Hydrogen;

public class OnDisposeStream<TInner> : StreamDecorator<TInner> where TInner : Stream {
	private readonly Action<TInner> _disposeAction;

	public OnDisposeStream(TInner stream, Action disposeAction)
		: this(stream, _ => disposeAction()) {
	}

	public OnDisposeStream(TInner stream, Action<TInner> disposeAction)
		: base(stream) {
		_disposeAction = disposeAction;
	}

	protected override void Dispose(bool disposing) {
		_disposeAction?.Invoke(InnerStream);
		base.Dispose(disposing);
	}
}

public sealed class OnDisposeStream : OnDisposeStream<Stream> {

	public OnDisposeStream(Stream stream, Action disposeAction)
		: this(stream, _ => disposeAction()) {
	}

	public OnDisposeStream(Stream stream, Action<Stream> disposeAction)
		: base(stream, disposeAction) {
	}

}
