// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.IO;

namespace Hydrogen;

public class StreamProfiler : StreamDecorator {
	private volatile bool _peeking;
	private readonly Stack<List<byte>> _captureScopes;
	private readonly bool _allowNestedListeningScopes;

	public StreamProfiler(Stream stream, bool allowNestedListenScopes, bool closeInternalStream = false) : base(stream) {
		_captureScopes = new Stack<List<byte>>();
		_allowNestedListeningScopes = allowNestedListenScopes;
		_peeking = false;
	}

	public override int Read(byte[] buffer, int offset, int count) {
		var capture = new byte[count];
		var actualRead = InnerStream.Read(capture, 0, count);

		if (!_peeking) {
			if (_captureScopes.Count > 0) {
				var topMostScope = _captureScopes.Peek();
				topMostScope.AddRange(capture);
			}
		}
		Array.Copy(capture, 0, buffer, offset, actualRead);
		return actualRead;
	}

	public virtual void StartListening() {
		if (_captureScopes.Count > 0 && !_allowNestedListeningScopes) {
			throw new Exception("Nested listening scopes are not allowed.");
		}
		_captureScopes.Push(new List<byte>());
	}

	public virtual byte[] StopListening() {
		if (_captureScopes.Count == 0) {
			throw new ApplicationException("Listening was not started");
		}
		var topMostScope = _captureScopes.Pop();
		if (_captureScopes.Count > 0) {
			var currentScope = _captureScopes.Peek();
			currentScope.AddRange(topMostScope);
		}
		return topMostScope.ToArray();
	}

	public byte PeekByte() {
		long origPos = Position;
		try {
			_peeking = true;
			Guard.Ensure(Position >= Length, "Cannot peek from end of stream");
			var b = ReadByte();
			Guard.Against(b == -1, "Stream ended inexplicably");
			return (byte)b;
		} finally {
			_peeking = false;
			Position = origPos;
		}
	}

	public byte[] PeekBytes(int number) {
		long origPos = Position;
		try {
			_peeking = true;
			var result = new byte[number];
			if (Position >= Length) {
				return new byte[0];
			}
			var b = Read(result, 0, number);
			if (b != number) {
				Array.Resize(ref result, b);
			}
			return result;
		} finally {
			_peeking = false;
			Position = origPos;
		}

	}

}
