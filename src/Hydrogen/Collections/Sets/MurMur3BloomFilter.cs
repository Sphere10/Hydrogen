using System;

namespace Hydrogen {

	/// <summary>
	/// A bloom filter implementation that uses MurMur3 checksums (with varying seeds) of serialized objects for the bloom hashing rounds.
	/// </summary>
	public sealed class MurMur3BloomFilter<TItem> : BloomFilterBase<TItem> {

		private readonly IItemSerializer<TItem> _objectSerializer;

		public MurMur3BloomFilter(decimal targetError, int maximumExpectedItems, int hashRounds, IItemSerializer<TItem> objectSerializer)
			: base(targetError, maximumExpectedItems, hashRounds) {
			_objectSerializer = objectSerializer;
		}

		public MurMur3BloomFilter(int messageLength, int hashRounds, IItemSerializer<TItem> objectSerializer) 
			: base(messageLength, hashRounds) {
			_objectSerializer = objectSerializer;
		}

		protected override int[] Hash(TItem item) {
			var objectBytes = _objectSerializer.SerializeLE(item);
			var result = new int[HashRounds];
			for (var i = 0; i < HashRounds; i++) {
				result[i] = Math.Abs(MURMUR3_32.Execute(objectBytes, i) % FilterLength);
			}
			return result;
		}

	}
}
