using System;

namespace Sphere10.Framework {
	public interface IBuffer : IExtendedList<byte> {

		ReadOnlySpan<byte> ReadSpan(int index, int count);

		void AddRange(ReadOnlySpan<byte> span);

		void UpdateRange(int index, ReadOnlySpan<byte> items);

		void InsertRange(int index, ReadOnlySpan<byte> items);

		Span<byte> AsSpan(int index, int count);
	}


	public static class IBufferExtensions {

		public static ReadOnlySpan<byte> ReadSpan(this IBuffer memoryBuffer, Range range) {
			var (start, count) = range.GetOffsetAndLength(memoryBuffer.Count);
			return memoryBuffer.ReadSpan(start, count);
		}


		public static Span<byte> AsSpan(this IBuffer memoryBuffer, Range range) {
			var (start, count) = range.GetOffsetAndLength(memoryBuffer.Count);
			return memoryBuffer.AsSpan(start, count);
		}

		public static Span<byte> AsSpan(this IBuffer memoryBuffer, Index startIndex)
			=> memoryBuffer.AsSpan(Range.StartAt(startIndex));
	}
}
