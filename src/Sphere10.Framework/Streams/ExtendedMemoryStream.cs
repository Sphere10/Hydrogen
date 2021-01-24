//-----------------------------------------------------------------------
// <copyright file="BitStream.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (http://www.sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// <author>Herman Schoenfeld</author>
// <date>2018</date>
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sphere10.Framework {
	/// <summary>
	/// A memory stream that writes to an IExtendedList of bytes.
	/// </summary>
	public class ExtendedMemoryStream : Stream {

		private long _position;
		private readonly IExtendedList<byte> _source;

		public ExtendedMemoryStream() 
			: this(new MemoryBuffer()) {
		}

		public ExtendedMemoryStream(IExtendedList<byte> source) {
			_source = source;
			_position = 0;
		}

		public override bool CanRead => true;

		public override bool CanSeek => true;

		public override bool CanWrite => true;

		public override void Flush() {
		}

		public override long Length => _source.Count;

		public override long Position {
			get => _position;
			set {
				Guard.ArgumentInRange(value, 0, Length, "Value");
				_position = value;
			} 
		}

		public override int Read(byte[] buffer, int offset, int count) {
			Guard.ArgumentNotNull(buffer, nameof(buffer));
			Guard.ArgumentInRange(offset, 0, buffer.Length - 1, nameof(offset));
			Guard.ArgumentInRange(count, 0, int.MaxValue, nameof(count));

			if (Length == 0)
				return 0;

			var remainingSourceBytes = _source.Count - Position;
			var remainingBufferBytes = buffer.Length - offset;
			var bytesRead = Math.Max(0, Math.Min(count, Math.Min(remainingBufferBytes, remainingSourceBytes)));
			var bytes = _source.ReadRange((int)Position, (int)bytesRead).ToArray();
            System.Buffer.BlockCopy(bytes, 0, buffer, offset, (int)bytesRead);
			Position += bytesRead;
			Debug.Assert(0 <= Position && Position <= Length);
			return (int)bytesRead;
		}

		/// <summary>
		/// Set up the stream position
		/// </summary>
		/// <param name="offset">Position</param>
		/// <param name="origin">Position origin</param>
		/// <returns>Position after setup</returns>
		public override long Seek(long offset, SeekOrigin origin) {
			switch (origin) {
				case (SeekOrigin.Begin):
					Guard.ArgumentInRange(offset, 0, Math.Max(0, Length), nameof(offset));
					Position = offset;
					break;

				case (SeekOrigin.Current):
					Guard.ArgumentInRange(offset, -Position, Math.Max(0, Length - Position), nameof(offset));
					Position = Position + offset;
					break;

				case (SeekOrigin.End):
					Guard.ArgumentInRange(offset, -Length, 0, nameof(offset));
					Position = Length + offset;
					break;

			}
			return Position;
		}

		public override void SetLength(long value) {
			Guard.ArgumentInRange(value, 0, int.MaxValue, nameof(value));
			if (value < Length) {
				_source.RemoveRange((int)value, (int)(Length - value));
				if (Position > value)
					Position = value;
			} else if (value > Length) {
				_source.AddRange(Tools.Array.Gen((int)(value - Length), (byte)0)); 
			}
		}

		public override void Write(byte[] buffer, int offset, int count) {
			Guard.ArgumentNotNull(buffer, nameof(buffer));
			Guard.ArgumentInRange(offset, 0, buffer.Length - 1, nameof(offset));
			Guard.ArgumentInRange(count, 0, buffer.Length - offset, nameof(count));

			var updateAmount = (int)Math.Min(Length - Position, count);
			var addingAmount = (int)Math.Max(0, count - updateAmount);
			Debug.Assert(updateAmount + addingAmount == count);

			if (updateAmount > 0) {
				var updatedBytes = new byte[updateAmount];
                System.Buffer.BlockCopy(buffer, offset, updatedBytes, 0, updateAmount);
				_source.UpdateRange((int)Position, updatedBytes);
			}

			if (addingAmount > 0) {
				var addedBytes = new byte[addingAmount];
                System.Buffer.BlockCopy(buffer, offset + updateAmount, addedBytes, 0, addingAmount);
				_source.AddRange(addedBytes);
			}
			Position += updateAmount + addingAmount;
			Debug.Assert(Position <= Length);
		}

		public virtual byte[] ToArray() {
			return _source.ToArray();
		}
	}
}
