using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Sphere10.Framework {

	// NOTES: use Bits as a guide


	/*
	 

	Bit-ordering within `ReadBits` and `WriteBits` are such that the 
   least-significant bit (LSB) is the left-most bit of that byte. 
   
   For example, consider an array of two bytes C = [A,B]:
   
   Memory layout of C=[a,b] with their in-byte indexes marked.
   A:[7][6][5][4][3][2][1][0]  B:[7][6][5][4][3][2][1][0]
   C:[0][1][2][3][4][5][6][7]    [8][9]...
   
    The bit indexes of the 16-bit array C are such that:
     Bit 0 maps to A[7]
     Bit 1 maps to A[6]
     Bit 7 maps to A[0]
     Bit 8 maps to B[7]
     Bit 16 maps to B[0]

	 */
	/// <summary>
	/// A replacement for <see cref="BitArray"/>
	/// </summary>
	public class StreamMappedBitVector : RangedListBase<bool> {

		private readonly Stream _stream;
		private int _count;

		public StreamMappedBitVector(IExtendedList<byte> bytes, int? initialBitCount = null)
			: this(new ExtendedMemoryStream(bytes), initialBitCount) {
		}

		public StreamMappedBitVector(Stream stream, int? initialBitCount = null) {
			_stream = stream;
			_count = initialBitCount ?? (int)(stream.Length / 8);
		}


		public override int Count => _count;

		public override void AddRange(IEnumerable<bool> items) {
			var itemsArray = items as bool[] ?? items.ToArray();
			if (!itemsArray.Any())
				return;

			int bytesCount = (int)Math.Ceiling((decimal)itemsArray.Length / 8);
			byte[] writeBuffer;

			if (_count == 0) {
				writeBuffer = Tools.Array.Gen(bytesCount, (byte)0);

				for (int i = 0; i < itemsArray.Length; i++) {
					Bits.SetBit(writeBuffer, i, itemsArray[i]);
				}

				_stream.Seek(0, SeekOrigin.Begin);
				_stream.Write(writeBuffer);
			} else {
				int writeBitIndex = 0;
				if (_count > 0) {
					writeBitIndex = _count % 8;
				}

				long streamByteOffsetFromBegin = writeBitIndex > 0 ? _stream.Length - 1 : _stream.Length;

				if (writeBitIndex > 0) {
					// determine whether an additional byte is required, or if existing byte being added to is enough.
					int withoutOffsetByteCount = (int)Math.Ceiling(((decimal)itemsArray.Length - (8 - writeBitIndex)) / 8);

					if (withoutOffsetByteCount == bytesCount) {
						bytesCount++;
					}

					writeBuffer = Tools.Array.Gen(bytesCount, (byte)0);

					_stream.Seek(streamByteOffsetFromBegin, SeekOrigin.Begin);
					_stream.Read(writeBuffer, 0, 1);
				} else {
					writeBuffer = Tools.Array.Gen(bytesCount, (byte)0);
				}

				for (int i = 0; i < itemsArray.Length; i++) {
					Bits.SetBit(writeBuffer, i + writeBitIndex, itemsArray[i]);
				}

				_stream.Seek(streamByteOffsetFromBegin, SeekOrigin.Begin);
				_stream.Write(writeBuffer);

			}

			_count += itemsArray.Length;
		}

		public override IEnumerable<int> IndexOfRange(IEnumerable<bool> items) {
			var itemsArray = items as bool[] ?? items.ToArray();
			if (!itemsArray.Any()) {
				return new List<int>();
			}

			var results = new int[itemsArray.Length];
			_stream.Seek(0, SeekOrigin.Begin);

			for (int i = 0; i < _stream.Length; i++) {
				byte[] current = _stream.ReadBytes(1);
				int bitsLength = Math.Min(8, _count - i * 8);

				for (int j = 0; j < bitsLength; j++) {
					bool value = Bits.ReadBit(current, j);
					foreach (var (t, index) in itemsArray.WithIndex()) {
						if (value == t) {
							results[index] = i * 8 + j;
						}
					}
				}
			}

			return results;
		}

		public override void InsertRange(int index, IEnumerable<bool> items) {
			var itemsArray = items as bool[] ?? items.ToArray();
			if (!itemsArray.Any())
				return;

			if (index == Count)
				AddRange(itemsArray);
			else
				throw new NotSupportedException("This collection can only be inserted from the end");
		}

		public override IEnumerable<bool> ReadRange(int index, int count) {
			int offset = index % 8;

			int byteCount = Math.Max(1, (int)Math.Ceiling(((decimal)count - (8 - offset)) / 8));
			int byteIndex = (int)Math.Floor((decimal)index / 8);

			if (offset > 0) {
				byteCount++;
			}

			_stream.Seek(byteIndex, SeekOrigin.Begin);
			byte[] bytes = _stream.ReadBytes(byteCount);

			for (int i = 0; i < count; i++) {
				yield return Bits.ReadBit(bytes, i + offset);
			}
		}

		public override void RemoveRange(int index, int count) {
			if (index + count != Count)
				throw new NotSupportedException("This collection can only be removed from the end");

			long bytesToRemove = _stream.Length - (int)Math.Ceiling((decimal)(_count - count) / 8);
			_stream.SetLength(_stream.Length - bytesToRemove);
			_count -= count;
		}

		public override void UpdateRange(int index, IEnumerable<bool> items) {
			var itemsArray = items as bool[] ?? items.ToArray();
			var endIndex = index + itemsArray.Length;
			if (endIndex > _count)
				throw new ArgumentOutOfRangeException(nameof(index), "Update range is out of bounds");

			int byteIndex = (int)Math.Floor((decimal)index / 8);
			int bitIndex = index % 8;
			int bytesCount = (int)Math.Ceiling((decimal)itemsArray.Length / 8);

			byte[] buffer;
			if (bitIndex > 0) {
				int afterOffsetBytes = (int)Math.Ceiling(((decimal)itemsArray.Length - (8 - bitIndex)) / 8);

				if (afterOffsetBytes == bytesCount) {
					bytesCount++;
				}

				buffer = Tools.Array.Gen(bytesCount, (byte)0);

				_stream.Seek(byteIndex, SeekOrigin.Begin);
				_stream.Read(buffer, 0, 1);

				_stream.Seek(byteIndex + bytesCount - 1, SeekOrigin.Begin);
				_stream.Read(buffer, buffer.Length - 1, 1);

			} else {
				buffer = Tools.Array.Gen(bytesCount, (byte)0);
			}

			for (int i = 0; i < itemsArray.Length; i++) {
				Bits.SetBit(buffer, i + bitIndex, itemsArray[i]);
			}

			_stream.Seek(byteIndex, SeekOrigin.Begin);
			_stream.Write(buffer);
		}

		public override void Clear() {
			_count = 0;
			_stream.SetLength(0);
			base.Clear();
		}
	}
}
