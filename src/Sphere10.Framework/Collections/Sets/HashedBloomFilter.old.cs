//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace Sphere10.Framework {

//	/// <summary>
//	/// A bloom filter base implementation that uses the bytes of a cryptographic hash of an object to determine the bloom mask for that object.
//	/// </summary>
//	public class HashedBloomFilter<TItem> : HashedBloomFilterBase<TItem> {
//		private readonly Func<TItem, byte[]> _hashFunc;

//		public HashedBloomFilter(decimal targetError, int maximumExpectedItems, int hashRounds, IObjectSerializer<TItem> serializer)
//			: this(targetError, maximumExpectedItems, hashRounds, serializer, SupportedAlgorithm.SHA2_256) {
//		}

//		public HashedBloomFilter(decimal targetError, int maximumExpectedItems, int hashRounds, IObjectSerializer<TItem> serializer, SupportedAlgorithm hashAlgorithm)
//			: this(targetError, maximumExpectedItems, hashRounds, (item) => rawHasher(serializer.SerializeLE(item))) {
//		}

//		public HashedBloomFilter(decimal targetError, int maximumExpectedItems, int hashRounds, Func<TItem, byte[]> itemHashFunc)
//			: base(targetError, maximumExpectedItems, hashRounds) {
//			_hashFunc = itemHashFunc;
//		}

//		public HashedBloomFilter(int messageLength, int hashRounds, Func<TItem, byte[]> hashFunc) 
//			: base(messageLength, hashRounds) {
//			_hashFunc = hashFunc;
//		}

//		protected override byte[] CryptographicallyHash(TItem item) => _hashFunc(item);
//	}
//}
