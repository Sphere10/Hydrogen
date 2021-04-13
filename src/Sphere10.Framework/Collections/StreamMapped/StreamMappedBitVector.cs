using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
			throw new NotImplementedException();
		}

		public override IEnumerable<int> IndexOfRange(IEnumerable<bool> items) {
			throw new NotImplementedException();
		}

		public override void InsertRange(int index, IEnumerable<bool> items) {
			if (index == Count)
				AddRange(items);
			else
				throw new NotSupportedException("This collection can only be inserted from the end");
		}

		public override IEnumerable<bool> ReadRange(int index, int count) {
			throw new NotImplementedException();
		}

		public override void RemoveRange(int index, int count) {
			if (index + count != Count)
				throw new NotSupportedException("This collection can only be removed from the end");
			throw new NotImplementedException();
		}

		public override void UpdateRange(int index, IEnumerable<bool> items) {
			throw new NotImplementedException();
		}
	}
}
