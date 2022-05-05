//-----------------------------------------------------------------------
// <copyright file="EndianBitConverter.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (http://www.sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// Acknowledgment: Based on original by Jon Skeet (http://jonskeet.uk/csharp/miscutil/)
//
// <author>Herman Schoenfeld</author>
// <date>2018</date>
// </copyright>
//-----------------------------------------------------------------------


using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Hydrogen {
	/// <summary>
	/// Equivalent of System.BitConverter, but with either endianness.
	/// </summary>
	public abstract class EndianBitConverter {
        #region Endianness of this converter

        /// <summary>
        /// Indicates the byte order ("endianess") in which data is converted using this class.
        /// </summary>
        /// <remarks>
        /// Different computer architectures store data using different byte orders. "Big-endian"
        /// means the most significant byte is on the left end of a word. "Little-endian" means the 
        /// most significant byte is on the right end of a word.
        /// </remarks>
        /// <returns>true if this converter is little-endian, false otherwise.</returns>
        public abstract bool IsLittleEndian();

        /// <summary>
        /// Indicates the byte order ("endianess") in which data is converted using this class.
        /// </summary>
        public abstract Endianness Endianness { get; }

        #endregion

        #region Factory properties

		/// <summary>
        /// Returns a little-endian bit converter instance. The same instance is
        /// always returned.
        /// </summary>
        public static LittleEndianBitConverter Little { get; } = new LittleEndianBitConverter();

		/// <summary>
        /// Returns a big-endian bit converter instance. The same instance is
        /// always returned.
        /// </summary>
        public static BigEndianBitConverter Big { get; } = new BigEndianBitConverter();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static EndianBitConverter For(Endianness endianness) => endianness == Endianness.LittleEndian ? (EndianBitConverter)Little: Big;
        
        #endregion

        #region Double/primitive conversions

        /// <summary>
        /// Converts the specified double-precision floating point number to a 
        /// 64-bit signed integer. Note: the endianness of this converter does not
        /// affect the returned value.
        /// </summary>
        /// <param name="value">The number to convert. </param>
        /// <returns>A 64-bit signed integer whose value is equivalent to value.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long DoubleToInt64Bits(double value) {
            return BitConverter.DoubleToInt64Bits(value);
        }

        /// <summary>
        /// Converts the specified 64-bit signed integer to a double-precision 
        /// floating point number. Note: the endianness of this converter does not
        /// affect the returned value.
        /// </summary>
        /// <param name="value">The number to convert. </param>
        /// <returns>A double-precision floating point number whose value is equivalent to value.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double Int64BitsToDouble(long value) {
            return BitConverter.Int64BitsToDouble(value);
        }

        /// <summary>
        /// Converts the specified single-precision floating point number to a 
        /// 32-bit signed integer. Note: the endianness of this converter does not
        /// affect the returned value.
        /// </summary>
        /// <param name="value">The number to convert. </param>
        /// <returns>A 32-bit signed integer whose value is equivalent to value.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int SingleToInt32Bits(float value) {
            return new Int32SingleUnion(value).AsInt32;
        }

        /// <summary>
        /// Converts the specified 32-bit signed integer to a single-precision floating point 
        /// number. Note: the endianness of this converter does not
        /// affect the returned value.
        /// </summary>
        /// <param name="value">The number to convert. </param>
        /// <returns>A single-precision floating point number whose value is equivalent to value.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Int32BitsToSingle(int value) {
            return new Int32SingleUnion(value).AsSingle;
        }

        #endregion

        #region To(PrimitiveType) conversions

        /// <summary>
        /// Returns a Boolean value converted from one byte at a specified position in a byte array.
        /// </summary>
        /// <param name="value">An array of bytes.</param>
        /// <param name="startIndex">The starting position within value.</param>
        /// <returns>true if the byte at startIndex in value is nonzero; otherwise, false.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ToBoolean(ReadOnlySpan<byte> value, int startIndex = 0) {
            CheckByteArgument(value, startIndex, 1);
            return BitConverter.ToBoolean(value.Slice(startIndex));
        }

        /// <summary>
        /// Returns a Unicode character converted from two bytes at a specified position in a byte array.
        /// </summary>
        /// <param name="value">An array of bytes.</param>
        /// <param name="startIndex">The starting position within value.</param>
        /// <returns>A character formed by two bytes beginning at startIndex.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public char ToChar(ReadOnlySpan<byte> value, int startIndex = 0) {
            return unchecked((char) (CheckedFromBytes(value, startIndex, 2)));
        }

        /// <summary>
        /// Returns a double-precision floating point number converted from eight bytes 
        /// at a specified position in a byte array.
        /// </summary>
        /// <param name="value">An array of bytes.</param>
        /// <param name="startIndex">The starting position within value.</param>
        /// <returns>A double precision floating point number formed by eight bytes beginning at startIndex.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double ToDouble(ReadOnlySpan<byte> value, int startIndex = 0) {
            return Int64BitsToDouble(ToInt64(value, startIndex));
        }

        /// <summary>
        /// Returns a single-precision floating point number converted from four bytes 
        /// at a specified position in a byte array.
        /// </summary>
        /// <param name="value">An array of bytes.</param>
        /// <param name="startIndex">The starting position within value.</param>
        /// <returns>A single precision floating point number formed by four bytes beginning at startIndex.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float ToSingle(ReadOnlySpan<byte> value, int startIndex = 0) {
            return Int32BitsToSingle(ToInt32(value, startIndex));
        }

        /// <summary>
        /// Returns a 16-bit signed integer converted from two bytes at a specified position in a byte array.
        /// </summary>
        /// <param name="value">An array of bytes.</param>
        /// <param name="startIndex">The starting position within value.</param>
        /// <returns>A 16-bit signed integer formed by two bytes beginning at startIndex.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public short ToInt16(ReadOnlySpan<byte> value, int startIndex = 0) {
            return unchecked((short) (CheckedFromBytes(value, startIndex, 2)));
        }

        /// <summary>
        /// Returns a 32-bit signed integer converted from four bytes at a specified position in a byte array.
        /// </summary>
        /// <param name="value">An array of bytes.</param>
        /// <param name="startIndex">The starting position within value.</param>
        /// <returns>A 32-bit signed integer formed by four bytes beginning at startIndex.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ToInt32(ReadOnlySpan<byte> value, int startIndex = 0) {
            return unchecked((int) (CheckedFromBytes(value, startIndex, 4)));
        }

        /// <summary>
        /// Returns a 64-bit signed integer converted from eight bytes at a specified position in a byte array.
        /// </summary>
        /// <param name="value">An array of bytes.</param>
        /// <param name="startIndex">The starting position within value.</param>
        /// <returns>A 64-bit signed integer formed by eight bytes beginning at startIndex.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long ToInt64(ReadOnlySpan<byte> value, int startIndex = 0) {
            return CheckedFromBytes(value, startIndex, 8);
        }

        /// <summary>
        /// Returns a 16-bit unsigned integer converted from two bytes at a specified position in a byte array.
        /// </summary>
        /// <param name="value">An array of bytes.</param>
        /// <param name="startIndex">The starting position within value.</param>
        /// <returns>A 16-bit unsigned integer formed by two bytes beginning at startIndex.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ushort ToUInt16(ReadOnlySpan<byte> value, int startIndex = 0) {
            return unchecked((ushort) (CheckedFromBytes(value, startIndex, 2)));
        }

        /// <summary>
        /// Returns a 32-bit unsigned integer converted from four bytes at a specified position in a byte array.
        /// </summary>
        /// <param name="value">An array of bytes.</param>
        /// <param name="startIndex">The starting position within value.</param>
        /// <returns>A 32-bit unsigned integer formed by four bytes beginning at startIndex.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint ToUInt32(ReadOnlySpan<byte> value, int startIndex = 0) {
            return unchecked((uint) (CheckedFromBytes(value, startIndex, 4)));
        }

        /// <summary>
        /// Returns a 64-bit unsigned integer converted from eight bytes at a specified position in a byte array.
        /// </summary>
        /// <param name="value">An array of bytes.</param>
        /// <param name="startIndex">The starting position within value.</param>
        /// <returns>A 64-bit unsigned integer formed by eight bytes beginning at startIndex.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong ToUInt64(ReadOnlySpan<byte> value, int startIndex = 0) {
            return unchecked((ulong) (CheckedFromBytes(value, startIndex, 8)));
        }

        /// <summary>
        /// Checks the given argument for validity.
        /// </summary>
        /// <param name="value">The byte array passed in</param>
        /// <param name="startIndex">The start index passed in</param>
        /// <param name="bytesRequired">The number of bytes required</param>
        /// <exception cref="ArgumentNullException">value is a null reference</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// startIndex is less than zero or greater than the length of value minus bytesRequired.
        /// </exception>
        private static void CheckByteArgument(ReadOnlySpan<byte> value, int startIndex, int bytesRequired) {
            if (value == null) {
                throw new ArgumentNullException("value");
            }
            if (startIndex < 0 || startIndex > value.Length - bytesRequired) {
                throw new ArgumentOutOfRangeException("startIndex");
            }
        }

        /// <summary>
        /// Checks the arguments for validity before calling FromBytes
        /// (which can therefore assume the arguments are valid).
        /// </summary>
        /// <param name="value">The bytes to convert after checking</param>
        /// <param name="startIndex">The index of the first byte to convert</param>
        /// <param name="bytesToConvert">The number of bytes to convert</param>
        /// <returns></returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
        private long CheckedFromBytes(ReadOnlySpan<byte> value, int startIndex, int bytesToConvert) {
            CheckByteArgument(value, startIndex, bytesToConvert);
            return FromBytes(value, startIndex, bytesToConvert);
        }

        /// <summary>
        /// Convert the given number of bytes from the given array, from the given start
        /// position, into a long, using the bytes as the least significant part of the long.
        /// By the time this is called, the arguments have been checked for validity.
        /// </summary>
        /// <param name="value">The bytes to convert</param>
        /// <param name="startIndex">The index of the first byte to convert</param>
        /// <param name="bytesToConvert">The number of bytes to use in the conversion</param>
        /// <returns>The converted number</returns>
        protected abstract long FromBytes(ReadOnlySpan<byte> value, int startIndex, int bytesToConvert);

        #endregion

        #region ToString conversions

        /// <summary>
        /// Returns a String converted from the elements of a byte array.
        /// </summary>
        /// <param name="value">An array of bytes.</param>
        /// <remarks>All the elements of value are converted.</remarks>
        /// <returns>
        /// A String of hexadecimal pairs separated by hyphens, where each pair 
        /// represents the corresponding element in value; for example, "7F-2C-4A".
        /// </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToString(ReadOnlySpan<byte> value) {
            return BitConverter.ToString(value.ToArray());
        }

        /// <summary>
        /// Returns a String converted from the elements of a byte array starting at a specified array position.
        /// </summary>
        /// <param name="value">An array of bytes.</param>
        /// <param name="startIndex">The starting position within value.</param>
        /// <remarks>The elements from array position startIndex to the end of the array are converted.</remarks>
        /// <returns>
        /// A String of hexadecimal pairs separated by hyphens, where each pair 
        /// represents the corresponding element in value; for example, "7F-2C-4A".
        /// </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToString(ReadOnlySpan<byte> value, int startIndex) {
            return BitConverter.ToString(value.ToArray(), startIndex);
        }

        /// <summary>
        /// Returns a String converted from a specified number of bytes at a specified position in a byte array.
        /// </summary>
        /// <param name="value">An array of bytes.</param>
        /// <param name="startIndex">The starting position within value.</param>
        /// <param name="length">The number of bytes to convert.</param>
        /// <remarks>The length elements from array position startIndex are converted.</remarks>
        /// <returns>
        /// A String of hexadecimal pairs separated by hyphens, where each pair 
        /// represents the corresponding element in value; for example, "7F-2C-4A".
        /// </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToString(ReadOnlySpan<byte> value, int startIndex, int length) {
            return BitConverter.ToString(value.ToArray(), startIndex, length);
        }

        #endregion

        #region	Decimal conversions

        /// <summary>
        /// Returns a decimal value converted from sixteen bytes 
        /// at a specified position in a byte array.
        /// </summary>
        /// <param name="value">An array of bytes.</param>
        /// <param name="startIndex">The starting position within value.</param>
        /// <returns>A decimal  formed by sixteen bytes beginning at startIndex.</returns>
        public decimal ToDecimal(ReadOnlySpan<byte> value, int startIndex) {
            // HACK: This always assumes four parts, each in their own endianness,
            // starting with the first part at the start of the byte array.
            // On the other hand, there's no real format specified...
            int[] parts = new int[4];
            for (int i = 0; i < 4; i++) {
                parts[i] = ToInt32(value, startIndex + i*4);
            }
            return new Decimal(parts);
        }

        /// <summary>
        /// Returns the specified decimal value as an array of bytes.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <returns>An array of bytes with length 16.</returns>
        public byte[] GetBytes(decimal value) {
            var bytes = new byte[16];
            int[] parts = decimal.GetBits(value);
            for (int i = 0; i < 4; i++) {
                WriteToImpl(parts[i], 4, bytes, i*4);
            }
            return bytes;
        }

        /// <summary>
        /// Copies the specified decimal value into the specified byte array,
        /// beginning at the specified index.
        /// </summary>
        /// <param name="value">A character to convert.</param>
        /// <param name="buffer">The byte array to copy the bytes into</param>
        /// <param name="index">The first index into the array to copy the bytes into</param>
        public void WriteTo(decimal value, Span<byte> buffer, int index) {
            int[] parts = decimal.GetBits(value);
            for (int i = 0; i < 4; i++) {
                WriteToImpl(parts[i], 4, buffer, i*4 + index);
            }
        }

        #endregion

        #region GetBytes conversions

        /// <summary>
        /// Returns the specified Boolean value as an array of bytes.
        /// </summary>
        /// <param name="value">A Boolean value.</param>
        /// <returns>An array of bytes with length 1.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte[] GetBytes(bool value) {
            return BitConverter.GetBytes(value);
        }

        /// <summary>
        /// Returns the specified Unicode character value as an array of bytes.
        /// </summary>
        /// <param name="value">A character to convert.</param>
        /// <returns>An array of bytes with length 2.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte[] GetBytes(char value) {
            return GetBytesInternal(value, 2);
        }

        /// <summary>
        /// Returns the specified double-precision floating point value as an array of bytes.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <returns>An array of bytes with length 8.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte[] GetBytes(double value) {
            return GetBytesInternal(DoubleToInt64Bits(value), 8);
        }

        /// <summary>
        /// Returns the specified 16-bit signed integer value as an array of bytes.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <returns>An array of bytes with length 2.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte[] GetBytes(short value) {
            return GetBytesInternal(value, 2);
        }

        /// <summary>
        /// Returns the specified 32-bit signed integer value as an array of bytes.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <returns>An array of bytes with length 4.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte[] GetBytes(int value) {
            return GetBytesInternal(value, 4);
        }

        /// <summary>
        /// Returns the specified 64-bit signed integer value as an array of bytes.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <returns>An array of bytes with length 8.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte[] GetBytes(long value) {
            return GetBytesInternal(value, 8);
        }

        /// <summary>
        /// Returns the specified single-precision floating point value as an array of bytes.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <returns>An array of bytes with length 4.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte[] GetBytes(float value) {
            return GetBytesInternal(SingleToInt32Bits(value), 4);
        }

        /// <summary>
        /// Returns the specified 16-bit unsigned integer value as an array of bytes.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <returns>An array of bytes with length 2.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte[] GetBytes(ushort value) {
            return GetBytesInternal(value, 2);
        }

        /// <summary>
        /// Returns the specified 32-bit unsigned integer value as an array of bytes.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <returns>An array of bytes with length 4.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte[] GetBytes(uint value) {
            return GetBytesInternal(value, 4);
        }

        /// <summary>
        /// Returns the specified 64-bit unsigned integer value as an array of bytes.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <returns>An array of bytes with length 8.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte[] GetBytes(ulong value) {
            return GetBytesInternal(unchecked((long) value), 8);
        }

        /// <summary>
        /// Returns an array with the given number of bytes formed
        /// from the least significant bytes of the specified value.
        /// This is used to implement the other GetBytes methods.
        /// </summary>
        /// <param name="value">The value to get bytes for</param>
        /// <param name="bytes">The number of significant bytes to return</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
        private byte[] GetBytesInternal(long value, int bytes) {
			var buffer = new byte[bytes];
			WriteToImpl(value, bytes, buffer, 0);
			return buffer;
		}

        #endregion

        #region WriteTo conversions

        /// <summary>
        /// Copies the given number of bytes from the least-specific
        /// end of the specified value into the specified byte array, beginning
        /// at the specified index.
        /// This must be implemented in concrete derived classes, but the implementation
        /// may assume that the value will fit into the buffer.
        /// </summary>
        /// <param name="value">The value to copy bytes for</param>
        /// <param name="bytes">The number of significant bytes to copy</param>
        /// <param name="buffer">The byte array to copy the bytes into</param>
        /// <param name="index">The first index into the array to copy the bytes into</param>
        protected abstract void WriteToImpl(long value, int bytes, Span<byte> buffer, int index);

        /// <summary>
        /// Copies the specified Boolean value into the specified byte array,
        /// beginning at the specified index.
        /// </summary>
        /// <param name="value">A Boolean value.</param>
        /// <param name="buffer">The byte array to copy the bytes into</param>
        /// <param name="index">The first index into the array to copy the bytes into</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteTo(bool value, Span<byte> buffer, int index = 0) {
            WriteToInternal(value ? 1 : 0, 1, buffer, index);
        }

        /// <summary>
        /// Copies the specified Unicode character value into the specified byte array,
        /// beginning at the specified index.
        /// </summary>
        /// <param name="value">A character to convert.</param>
        /// <param name="buffer">The byte array to copy the bytes into</param>
        /// <param name="index">The first index into the array to copy the bytes into</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteTo(char value, Span<byte> buffer, int index = 0) {
            WriteToInternal(value, 2, buffer, index);
        }

        /// <summary>
        /// Copies the specified double-precision floating point value into the specified byte array,
        /// beginning at the specified index.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <param name="buffer">The byte array to copy the bytes into</param>
        /// <param name="index">The first index into the array to copy the bytes into</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteTo(double value, Span<byte> buffer, int index = 0) {
            WriteToInternal(DoubleToInt64Bits(value), 8, buffer, index);
        }

        /// <summary>
        /// Copies the specified 16-bit signed integer value into the specified byte array,
        /// beginning at the specified index.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <param name="buffer">The byte array to copy the bytes into</param>
        /// <param name="index">The first index into the array to copy the bytes into</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteTo(short value, Span<byte> buffer, int index = 0) {
            WriteToInternal(value, 2, buffer, index);
        }

        /// <summary>
        /// Copies the specified 32-bit signed integer value into the specified byte array,
        /// beginning at the specified index.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <param name="buffer">The byte array to copy the bytes into</param>
        /// <param name="index">The first index into the array to copy the bytes into</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteTo(int value, Span<byte> buffer, int index = 0) {
            WriteToInternal(value, 4, buffer, index);
        }

        /// <summary>
        /// Copies the specified 64-bit signed integer value into the specified byte array,
        /// beginning at the specified index.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <param name="buffer">The byte array to copy the bytes into</param>
        /// <param name="index">The first index into the array to copy the bytes into</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteTo(long value, Span<byte> buffer, int index = 0) {
            WriteToInternal(value, 8, buffer, index);
        }

        /// <summary>
        /// Copies the specified single-precision floating point value into the specified byte array,
        /// beginning at the specified index.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <param name="buffer">The byte array to copy the bytes into</param>
        /// <param name="index">The first index into the array to copy the bytes into</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteTo(float value, Span<byte> buffer, int index = 0) {
            WriteToInternal(SingleToInt32Bits(value), 4, buffer, index);
        }

        /// <summary>
        /// Copies the specified 16-bit unsigned integer value into the specified byte array,
        /// beginning at the specified index.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <param name="buffer">The byte array to copy the bytes into</param>
        /// <param name="index">The first index into the array to copy the bytes into</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteTo(ushort value, Span<byte> buffer, int index = 0) {
            WriteToInternal(value, 2, buffer, index);
        }

        /// <summary>
        /// Copies the specified 32-bit unsigned integer value into the specified byte array,
        /// beginning at the specified index.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <param name="buffer">The byte array to copy the bytes into</param>
        /// <param name="index">The first index into the array to copy the bytes into</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteTo(uint value, Span<byte> buffer, int index = 0) {
            WriteToInternal(value, 4, buffer, index);
        }

        /// <summary>
        /// Copies the specified 64-bit unsigned integer value into the specified byte array,
        /// beginning at the specified index.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <param name="buffer">The byte array to copy the bytes into</param>
        /// <param name="index">The first index into the array to copy the bytes into</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteTo(ulong value, Span<byte> buffer, int index = 0) {
            WriteToInternal(unchecked((long) value), 8, buffer, index);
        }

		/// <summary>
		/// Copies the given number of bytes from the least-specific
		/// end of the specified value into the specified byte array, beginning
		/// at the specified index.
		/// This is used to implement the other WriteTo methods.
		/// </summary>
		/// <param name="value">The value to copy bytes for</param>
		/// <param name="bytes">The number of significant bytes to copy</param>
		/// <param name="buffer">The byte array to copy the bytes into</param>
		/// <param name="index">The first index into the array to copy the bytes into</param>
		private void WriteToInternal(long value, int bytes, Span<byte> buffer, int index) {
			if (buffer == null) {
				throw new ArgumentNullException(nameof(buffer), "Byte array must not be null");
			}
			if (buffer.Length < index + bytes) {
				throw new ArgumentOutOfRangeException("Buffer not big enough for value");
			}
			WriteToImpl(value, bytes, buffer, index);
		}

        #endregion

        #region Private struct used for Single/Int32 conversions

        /// <summary>
        /// Union used solely for the equivalent of DoubleToInt64Bits and vice versa.
        /// </summary>
        [StructLayout(LayoutKind.Explicit)]
        private struct Int32SingleUnion {
            /// <summary>
            /// Int32 version of the value.
            /// </summary>
            [FieldOffset(0)] private int i;

            /// <summary>
            /// Single version of the value.
            /// </summary>
            [FieldOffset(0)] private float f;

            /// <summary>
            /// Creates an instance representing the given integer.
            /// </summary>
            /// <param name="i">The integer value of the new instance.</param>
            internal Int32SingleUnion(int i) {
                this.f = 0; // Just to keep the compiler happy
                this.i = i;
            }

            /// <summary>
            /// Creates an instance representing the given floating point number.
            /// </summary>
            /// <param name="f">The floating point value of the new instance.</param>
            internal Int32SingleUnion(float f) {
                this.i = 0; // Just to keep the compiler happy
                this.f = f;
            }

            /// <summary>
            /// Returns the value of the instance as an integer.
            /// </summary>
            internal int AsInt32 {
                get { return i; }
            }

            /// <summary>
            /// Returns the value of the instance as a floating point number.
            /// </summary>
            internal float AsSingle {
                get { return f; }
            }
        }

        #endregion
    }
}
