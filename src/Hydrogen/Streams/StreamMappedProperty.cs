// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.IO;

namespace Hydrogen;

public class StreamMappedProperty<T> {
	private readonly Stream _stream;
	private readonly long _offset;
	private readonly long _size;
	private readonly IItemSerializer<T> _serializer;
	private readonly ICriticalObject _lock;
	private readonly IEqualityComparer<T> _equalityComparer;
	private readonly EndianBinaryReader _reader;
	private readonly EndianBinaryWriter _writer;

	private bool _hasValue;
		
	private T _lastValue;

	public StreamMappedProperty(Stream stream, long offset, long size, IItemSerializer<T> serializer, IEqualityComparer<T> comparer = null, ICriticalObject @lock = null, Endianness endianess = Endianness.LittleEndian) 
		: this(stream, offset, size, serializer, new EndianBinaryReader(EndianBitConverter.For(endianess), stream), new EndianBinaryWriter(EndianBitConverter.For(endianess), stream), comparer, @lock) {
	}

	public StreamMappedProperty(Stream stream, long offset, long size, IItemSerializer<T> serializer, EndianBinaryReader reader, EndianBinaryWriter writer, IEqualityComparer<T> comparer = null, ICriticalObject @lock = null) {
		Guard.ArgumentNotNull(stream, nameof(stream));
		Guard.ArgumentNotNull(serializer, nameof(serializer));
		_stream = stream;
		_offset = offset;
		_size = size;
		_serializer = serializer;
		_lock = @lock ?? new CriticalObject(); 
		_equalityComparer = comparer ?? EqualityComparer<T>.Default;
		_reader = reader;
		_writer = writer;
		_hasValue = false;
		_lastValue = default;
	}

	public T Value {
		get {
			using var _ = _lock.EnterAccessScope();
			if (!_hasValue) {
				_stream.Seek(_offset, SeekOrigin.Begin);
				_lastValue = _serializer.Deserialize(_reader);
				_hasValue = true;
			}
			return _lastValue;
		}
		set {
			using var _ = _lock.EnterAccessScope();
			if (_hasValue && _equalityComparer.Equals(_lastValue, value))
				return;
			_stream.Seek(_offset, SeekOrigin.Begin);
			_serializer.Serialize(value, _writer);
			_lastValue = value;
			_hasValue = true;
		}
	}

}
