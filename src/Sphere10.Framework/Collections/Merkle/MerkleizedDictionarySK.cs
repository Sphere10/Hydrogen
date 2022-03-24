using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Sphere10.Framework {

	internal class MerkleizedStore<TValue> : ClusteredDictionarySK<byte[], TValue>, IMerkleTree {

		public MerkleizedStore(Stream rootStream, int clusterSize, int hashLength, IItemSerializer<TValue> serializer, IEqualityComparer<TValue> comparer = null, ClusteredStoragePolicy policy = ClusteredStoragePolicy.DictionaryDefault, Endianness endianness = Endianness.LittleEndian)
			: base(
				  rootStream,
				  clusterSize,
				  new StaticSizeByteArraySerializer(hashLength),
				  serializer,
				  new HashChecksum(),
				  new ByteArrayEqualityComparer(),
				  comparer,
				  policy | ClusteredStoragePolicy.TrackChecksums | ClusteredStoragePolicy.TrackKey,
				  endianness
				) {
			base.Storage.RecordUpdated += StorageOnRecordUpdated;
		}

		private void StorageOnRecordUpdated(int index, ClusteredStreamRecord record) {
			throw new NotImplementedException();
		}

		public CHF HashAlgorithm { get; }

		public byte[] Root { get; }

		public MerkleSize Size { get; }

		public ReadOnlySpan<byte> GetValue(MerkleCoordinate coordinate) {
			throw new NotImplementedException();
		}
	}
}

