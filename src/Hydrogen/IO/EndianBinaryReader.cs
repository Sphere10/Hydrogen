// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Jon Skeet, Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.
// Based from original: http://jonskeet.uk/csharp/miscutil/

using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace Hydrogen;

/// <summary>
/// Equivalent of System.IO.BinaryReader, but with either endianness, depending on
/// the EndianBitConverter it is constructed with. No data is buffered in the
/// reader; the client may seek within the stream at will.
/// </summary>
public class EndianBinaryReader : IDisposable {

	#region Fields not directly related to properties

	/// <summary>
	/// Whether or not this reader has been disposed yet.
	/// </summary>
	private bool _disposed = false;

	/// <summary>
	/// Decoder to use for string conversions.
	/// </summary>
	private readonly Decoder _decoder;

	/// <summary>
	/// Buffer used for temporary streams before conversion into primitives
	/// </summary>
	private byte[] _buffer = new byte[16];

	/// <summary>
	/// Buffer used for temporary streams when reading a single character
	/// </summary>
	private readonly char[] _charBuffer = new char[1];

	/// <summary>
	/// Minimum number of bytes used to encode a character
	/// </summary>
	private readonly int _minBytesPerChar;

	#endregion

	#region Constructors

	/// <summary>
	/// Equivalent of System.IO.BinaryWriter, but with either endianness, depending on
	/// the EndianBitConverter it is constructed with.
	/// </summary>
	/// <param name="bitConverter">Converter to use when reading data</param>
	/// <param name="stream">Stream to read data from</param>
	public EndianBinaryReader(EndianBitConverter bitConverter,
	                          Stream stream) : this(bitConverter, stream, Encoding.UTF8) {
	}

	/// <summary>
	/// Constructs a new binary reader with the given bit converter, reading
	/// to the given stream, using the given encoding.
	/// </summary>
	/// <param name="bitConverter">Converter to use when reading data</param>
	/// <param name="stream">Stream to read data from</param>
	/// <param name="encoding">Encoding to use when reading character data</param>
	public EndianBinaryReader(EndianBitConverter bitConverter, Stream stream, Encoding encoding) {
		if (bitConverter == null) {
			throw new ArgumentNullException("bitConverter");
		}
		if (stream == null) {
			throw new ArgumentNullException("stream");
		}
		if (encoding == null) {
			throw new ArgumentNullException("encoding");
		}
		if (!stream.CanRead) {
			throw new ArgumentException("Stream isn't writable", "stream");
		}
		this._stream = stream;
		this._bitConverter = bitConverter;
		this._encoding = encoding;
		this._decoder = encoding.GetDecoder();
		this._minBytesPerChar = 1;

		if (encoding is UnicodeEncoding) {
			_minBytesPerChar = 2;
		}
	}

	#endregion

	#region Properties

	private readonly EndianBitConverter _bitConverter;

	/// <summary>
	/// The bit converter used to read values from the stream
	/// </summary>
	public EndianBitConverter BitConverter {
		get { return _bitConverter; }
	}

	private readonly Encoding _encoding;

	/// <summary>
	/// The encoding used to read strings
	/// </summary>
	public Encoding Encoding {
		get { return _encoding; }
	}

	private readonly Stream _stream;

	/// <summary>
	/// Gets the underlying stream of the EndianBinaryReader.
	/// </summary>
	public Stream BaseStream {
		get { return _stream; }
	}

	#endregion

	#region Public methods

	/// <summary>
	/// Closes the reader, including the underlying stream..
	/// </summary>
	public void Close() {
		Dispose();
	}

	/// <summary>
	/// Seeks within the stream.
	/// </summary>
	/// <param name="offset">Offset to seek to.</param>
	/// <param name="origin">Origin of seek operation.</param>
	public void Seek(int offset, SeekOrigin origin) {
		CheckDisposed();
		_stream.Seek(offset, origin);
	}

	/// <summary>
	/// Reads a single byte from the stream.
	/// </summary>
	/// <returns>The byte read</returns>
	public byte ReadByte() {
		ReadInternal(_buffer, 1);
		return _buffer[0];
	}

	/// <summary>
	/// Reads a single signed byte from the stream.
	/// </summary>
	/// <returns>The byte read</returns>
	public sbyte ReadSByte() {
		ReadInternal(_buffer, 1);
		return unchecked((sbyte)_buffer[0]);
	}

	/// <summary>
	/// Reads a boolean from the stream. 1 byte is read.
	/// </summary>
	/// <returns>The boolean read</returns>
	public bool ReadBoolean() {
		ReadInternal(_buffer, 1);
		return _bitConverter.ToBoolean(_buffer, 0);
	}

	/// <summary>
	/// Reads a 16-bit signed integer from the stream, using the bit converter
	/// for this reader. 2 bytes are read.
	/// </summary>
	/// <returns>The 16-bit integer read</returns>
	public short ReadInt16() {
		ReadInternal(_buffer, 2);
		return _bitConverter.ToInt16(_buffer, 0);
	}

	/// <summary>
	/// Reads a 32-bit signed integer from the stream, using the bit converter
	/// for this reader. 4 bytes are read.
	/// </summary>
	/// <returns>The 32-bit integer read</returns>
	public int ReadInt32() {
		ReadInternal(_buffer, 4);
		return _bitConverter.ToInt32(_buffer, 0);
	}

	/// <summary>
	/// Reads a 64-bit signed integer from the stream, using the bit converter
	/// for this reader. 8 bytes are read.
	/// </summary>
	/// <returns>The 64-bit integer read</returns>
	public long ReadInt64() {
		ReadInternal(_buffer, 8);
		return _bitConverter.ToInt64(_buffer, 0);
	}

	/// <summary>
	/// Reads a 16-bit unsigned integer from the stream, using the bit converter
	/// for this reader. 2 bytes are read.
	/// </summary>
	/// <returns>The 16-bit unsigned integer read</returns>
	public ushort ReadUInt16() {
		ReadInternal(_buffer, 2);
		return _bitConverter.ToUInt16(_buffer, 0);
	}

	/// <summary>
	/// Reads a 32-bit unsigned integer from the stream, using the bit converter
	/// for this reader. 4 bytes are read.
	/// </summary>
	/// <returns>The 32-bit unsigned integer read</returns>
	public uint ReadUInt32() {
		ReadInternal(_buffer, 4);
		return _bitConverter.ToUInt32(_buffer, 0);
	}

	/// <summary>
	/// Reads a 64-bit unsigned integer from the stream, using the bit converter
	/// for this reader. 8 bytes are read.
	/// </summary>
	/// <returns>The 64-bit unsigned integer read</returns>
	public ulong ReadUInt64() {
		ReadInternal(_buffer, 8);
		return _bitConverter.ToUInt64(_buffer, 0);
	}


	/// <summary>
	/// Reads a VarInt
	/// </summary>
	public VarInt ReadVarInt()
		=> VarInt.Read(_stream);

	/// <summary>
	/// Reads a Compact-encoded integer
	/// </summary>
	public CVarInt ReadCVarInt()
		=> CVarInt.Read(_stream);


	/// <summary>
	/// Reads a single-precision floating-point value from the stream, using the bit converter
	/// for this reader. 4 bytes are read.
	/// </summary>
	/// <returns>The floating point value read</returns>
	public float ReadSingle() {
		ReadInternal(_buffer, 4);
		return _bitConverter.ToSingle(_buffer, 0);
	}

	/// <summary>
	/// Reads a double-precision floating-point value from the stream, using the bit converter
	/// for this reader. 8 bytes are read.
	/// </summary>
	/// <returns>The floating point value read</returns>
	public double ReadDouble() {
		ReadInternal(_buffer, 8);
		return _bitConverter.ToDouble(_buffer, 0);
	}

	/// <summary>
	/// Reads a decimal value from the stream, using the bit converter
	/// for this reader. 16 bytes are read.
	/// </summary>
	/// <returns>The decimal value read</returns>
	public decimal ReadDecimal() {
		ReadInternal(_buffer, 16);
		return _bitConverter.ToDecimal(_buffer, 0);
	}

	/// <summary>
	/// Reads a single character from the stream, using the character encoding for
	/// this reader. If no characters have been fully read by the time the stream ends,
	/// -1 is returned.
	/// </summary>
	/// <returns>The character read, or -1 for end of stream.</returns>
	public int Read() {
		int charsRead = Read(_charBuffer, 0, 1);
		if (charsRead == 0) {
			return -1;
		} else {
			return _charBuffer[0];
		}
	}

	public T ReadOrThrow<T>(Func<Exception> exceptionActivator)
		=> (T)ReadOrThrow(typeof(T), exceptionActivator);

	public object ReadOrThrow(Type type, Func<Exception> exceptionActivator) {
		Guard.ArgumentNotNull(exceptionActivator, nameof(exceptionActivator));
		if (type == typeof(char)) {
			if (!_stream.CanRead(sizeof(char)))
				throw exceptionActivator();
			return Read();
		} else if (type == typeof(byte)) {
			if (!_stream.CanRead(sizeof(byte)))
				throw exceptionActivator();
			return ReadByte();
		} else if (type == typeof(ushort)) {
			if (!_stream.CanRead(sizeof(ushort)))
				throw exceptionActivator();
			return ReadUInt16();
		} else if (type == typeof(short)) {
			if (!_stream.CanRead(sizeof(short)))
				throw exceptionActivator();
			return ReadInt16();
		} else if (type == typeof(uint)) {
			if (!_stream.CanRead(sizeof(uint)))
				throw exceptionActivator();
			return ReadUInt32();
		} else if (type == typeof(int)) {
			if (!_stream.CanRead(sizeof(int)))
				throw exceptionActivator();
			return ReadInt32();
		} else if (type == typeof(ulong)) {
			if (!_stream.CanRead(sizeof(ulong)))
				throw exceptionActivator();
			return ReadUInt64();
		} else if (type == typeof(long)) {
			if (!_stream.CanRead(sizeof(long)))
				throw exceptionActivator();
			return ReadInt64();
		} else if (type == typeof(float)) {
			if (!_stream.CanRead(sizeof(float)))
				throw exceptionActivator();
			return ReadSingle();
		} else if (type == typeof(double)) {
			if (!_stream.CanRead(sizeof(double)))
				throw exceptionActivator();
			return ReadDouble();
		} else if (type == typeof(decimal)) {
			if (!_stream.CanRead(sizeof(decimal)))
				throw exceptionActivator();
			return ReadDecimal();
		} else if (type.HasSubType(typeof(Enum))) {
			var enumNumericType = type.GetEnumUnderlyingType();
			var result = ReadOrThrow(enumNumericType, exceptionActivator);
			return Enum.Parse(type, result.ToString());
		}
		throw new InvalidOperationException($"Cannot read type '{type.Name}'");
	}

	/// <summary>
	/// Reads the specified number of characters into the given buffer, starting at
	/// the given index.
	/// </summary>
	/// <param name="data">The buffer to copy data into</param>
	/// <param name="index">The first index to copy data into</param>
	/// <param name="count">The number of characters to read</param>
	/// <returns>The number of characters actually read. This will only be less than
	/// the requested number of characters if the end of the stream is reached.
	/// </returns>
	public int Read(char[] data, int index, int count) {
		CheckDisposed();
		if (_buffer == null) {
			throw new ArgumentNullException(nameof(_buffer));
		}
		if (index < 0) {
			throw new ArgumentOutOfRangeException(nameof(index));
		}
		if (count < 0) {
			throw new ArgumentOutOfRangeException(nameof(index));
		}
		if (count + index > data.Length) {
			throw new ArgumentException
				("Not enough space in buffer for specified number of characters starting at specified index");
		}

		int read = 0;
		bool firstTime = true;

		// Use the normal buffer if we're only reading a small amount, otherwise
		// use at most 4K at a time.
		byte[] byteBuffer = _buffer;

		if (byteBuffer.Length < count * _minBytesPerChar) {
			byteBuffer = new byte[4096];
		}

		while (read < count) {
			int amountToRead;
			// First time through we know we haven't previously read any data
			if (firstTime) {
				amountToRead = count * _minBytesPerChar;
				firstTime = false;
			}
			// After that we can only assume we need to fully read "chars left -1" characters
			// and a single byte of the character we may be in the middle of
			else {
				amountToRead = ((count - read - 1) * _minBytesPerChar) + 1;
			}
			if (amountToRead > byteBuffer.Length) {
				amountToRead = byteBuffer.Length;
			}
			int bytesRead = TryReadInternal(byteBuffer, amountToRead);
			if (bytesRead == 0) {
				return read;
			}
			int decoded = _decoder.GetChars(byteBuffer, 0, bytesRead, data, index);
			read += decoded;
			index += decoded;
		}
		return read;
	}

	/// <summary>
	/// Reads the specified number of bytes into the given buffer, starting at
	/// the given index.
	/// </summary>
	/// <param name="buffer">The buffer to copy data into</param>
	/// <param name="index">The first index to copy data into</param>
	/// <param name="count">The number of bytes to read</param>
	/// <returns>The number of bytes actually read. This will only be less than
	/// the requested number of bytes if the end of the stream is reached.
	/// </returns>
	public int Read(byte[] buffer, int index, int count) {
		CheckDisposed();
		if (buffer == null) {
			throw new ArgumentNullException(nameof(buffer));
		}
		if (index < 0) {
			throw new ArgumentOutOfRangeException(nameof(index));
		}
		if (count < 0) {
			throw new ArgumentOutOfRangeException(nameof(index));
		}
		if (count + index > buffer.Length) {
			throw new ArgumentException("Not enough space in buffer for specified number of bytes starting at specified index");
		}
		int read = 0;
		while (count > 0) {
			int block = _stream.Read(buffer, index, count);
			if (block == 0) {
				return read;
			}
			index += block;
			read += block;
			count -= block;
		}
		return read;
	}


	/// <summary>
	/// Fills the span with bytes
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int Read(Span<byte> buffer) {
		CheckDisposed();
		if (buffer == null) {
			throw new ArgumentNullException(nameof(buffer));
		}
		return _stream.Read(buffer);
	}


	/// <summary>
	/// Reads the specified number of bytes, returning them in a new byte array.
	/// If not enough bytes are available before the end of the stream, this
	/// method will return what is available.
	/// </summary>
	/// <param name="count">The number of bytes to read</param>
	/// <returns>The bytes read</returns>
	public byte[] ReadBytes(long count) {
		CheckDisposed();
		if (count < 0) {
			throw new ArgumentOutOfRangeException(nameof(count));
		}
		var countI = Tools.Collection.CheckNotImplemented64bitAddressingLength(count);
		byte[] ret = new byte[count];
		var index = 0;
		while (index < count) {
			int read = _stream.Read(ret, index, countI - index);
			// Stream has finished half way through. That's fine, return what we've got.
			if (read == 0) {
				byte[] copy = new byte[index];
				System.Buffer.BlockCopy(ret, 0, copy, 0, index);
				return copy;
			}
			index += read;
		}
		return ret;
	}

	/// <summary>
	/// Reads the specified number of bytes, returning them in a new byte array.
	/// If not enough bytes are available before the end of the stream, this
	/// method will throw an IOException.
	/// </summary>
	/// <param name="count">The number of bytes to read</param>
	/// <returns>The bytes read</returns>
	public byte[] ReadBytesOrThrow(int count) {
		byte[] ret = new byte[count];
		ReadInternal(ret, count);
		return ret;
	}

	/// <summary>
	/// Reads a 2D array of bytes.
	/// If not enough bytes are available before the end of the stream, this
	/// method will throw an IOException.
	/// </summary>
	/// <param name="rows">The first dimension of the 2d-array</param>
	/// <param name="columns">The second dimension of the 2d-array</param>
	/// <returns>The bytes read</returns>
	public byte[,] ReadArray2D(int rows, int columns) {
		byte[,] ret = new byte[rows, columns];
		for (var i = 0; i < rows; i++)
			ReadInternal(ret.GetRow(i), columns);
		return ret;
	}

	/// <summary>
	/// Reads a 7-bit encoded integer from the stream. This is stored with the least significant
	/// information first, with 7 bits of information per byte of value, and the top
	/// bit as a continuation flag. This method is not affected by the endianness
	/// of the bit converter.
	/// </summary>
	/// <returns>The 7-bit encoded integer read from the stream.</returns>
	public int Read7BitEncodedInt() {
		CheckDisposed();

		int ret = 0;
		for (int shift = 0; shift < 35; shift += 7) {
			int b = _stream.ReadByte();
			if (b == -1) {
				throw new EndOfStreamException();
			}
			ret = ret | ((b & 0x7f) << shift);
			if ((b & 0x80) == 0) {
				return ret;
			}
		}
		// Still haven't seen a byte with the high bit unset? Dodgy data.
		throw new IOException("Invalid 7-bit encoded integer in stream.");
	}

	/// <summary>
	/// Reads a 7-bit encoded integer from the stream. This is stored with the most significant
	/// information first, with 7 bits of information per byte of value, and the top
	/// bit as a continuation flag. This method is not affected by the endianness
	/// of the bit converter.
	/// </summary>
	/// <returns>The 7-bit encoded integer read from the stream.</returns>
	public int ReadBigEndian7BitEncodedInt() {
		CheckDisposed();

		int ret = 0;
		for (int i = 0; i < 5; i++) {
			int b = _stream.ReadByte();
			if (b == -1) {
				throw new EndOfStreamException();
			}
			ret = (ret << 7) | (b & 0x7f);
			if ((b & 0x80) == 0) {
				return ret;
			}
		}
		// Still haven't seen a byte with the high bit unset? Dodgy data.
		throw new IOException("Invalid 7-bit encoded integer in stream.");
	}

	/// <summary>
	/// Reads a length-prefixed string from the stream, using the encoding for this reader.
	/// A 7-bit encoded integer is first read, which specifies the number of bytes 
	/// to read from the stream. These bytes are then converted into a string with
	/// the encoding for this reader.
	/// </summary>
	/// <returns>The string read from the stream.</returns>
	public string ReadString() {
		int bytesToRead = Read7BitEncodedInt();

		byte[] data = new byte[bytesToRead];
		ReadInternal(data, bytesToRead);
		return _encoding.GetString(data, 0, data.Length);
	}

	#endregion

	#region Private methods

	/// <summary>
	/// Checks whether or not the reader has been disposed, throwing an exception if so.
	/// </summary>
	private void CheckDisposed() {
		if (_disposed) {
			throw new ObjectDisposedException(nameof(EndianBinaryReader));
		}
	}

	/// <summary>
	/// Reads the given number of bytes from the stream, throwing an exception
	/// if they can't all be read.
	/// </summary>
	/// <param name="data">Buffer to read into</param>
	/// <param name="size">Number of bytes to read</param>
	private void ReadInternal(Span<byte> data, int size) {
		CheckDisposed();
		int index = 0;
		while (index < size) {
			// int read = stream.Read(data, index, size - index);
			int read = _stream.Read(data.Slice(index, size - index));
			if (read == 0) {
				throw new EndOfStreamException($"End of stream reached with {size - index} byte{(size - index == 1 ? "s" : "")} left to read.");
			}
			index += read;
		}
	}

	/// <summary>
	/// Reads the given number of bytes from the stream if possible, returning
	/// the number of bytes actually read, which may be less than requested if
	/// (and only if) the end of the stream is reached.
	/// </summary>
	/// <param name="data">Buffer to read into</param>
	/// <param name="size">Number of bytes to read</param>
	/// <returns>Number of bytes actually read</returns>
	private int TryReadInternal(byte[] data, int size) {
		CheckDisposed();
		int index = 0;
		while (index < size) {
			int read = _stream.Read(data, index, size - index);
			if (read == 0) {
				return index;
			}
			index += read;
		}
		return index;
	}

	#endregion

	#region IDisposable Members

	/// <summary>
	/// Disposes of the underlying stream.
	/// </summary>
	public void Dispose() {
		if (!_disposed) {
			_disposed = true;
			((IDisposable)_stream).Dispose();
		}
	}

	#endregion

}
