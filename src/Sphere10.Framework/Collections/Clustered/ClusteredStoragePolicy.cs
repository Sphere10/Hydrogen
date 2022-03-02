﻿using System;

namespace Sphere10.Framework {
	
	[Flags]
	public enum ClusteredStoragePolicy : uint {

		/// <summary>
		/// Clusters are pre-allocated when serializing items. This avoids on-the-fly allocation during serialization which can be slow.
		/// </summary>
		FastAllocate = 1 >> 0,
		
		/// <summary>
		/// Clusters for records are cached for faster lookup (can cause memory-bloat when container's stream count in very large scenarios)
		/// </summary>
		CacheRecordClusters = 1 << 1,

		/// <summary>
		/// Clusters for opened stream data are cached for faster traversal (can cause memory-bloat for stream's with very large cluster count)
		/// </summary>
		CacheOpenClusters = 1 << 2,

		/// <summary>
		/// Performs real-time integrity checks when processing. Disabling this may improve performance slightly.
		/// </summary>
		IntegrityChecks = 1 << 3,

		///// <summary>
		///// Tracks a UInt32 checksum of streams in the stream record (used by dictionaries).
		///// </summary>
		TrackChecksums = 1 << 4,


		///// <summary>
		///// Tracks a UInt32 checksum of streams in the stream record (used by dictionaries).
		///// </summary>
		TrackDigests = 1 << 5,

		///// <summary>
		///// Builds a merkle-tree from the streams, stores the root in the header (used for blockchain). 
		///// </summary>
		//Merkleized = 1 << 6,

		///// <summary>
		///// Encrypts the underlying data
		///// </summary>
		//Encrypted,

		/// <summary>
		/// Default policy suitable for most use-cases
		/// </summary>
		Default = FastAllocate | CacheRecordClusters | CacheOpenClusters | IntegrityChecks,

		DictionaryDefault = Default | TrackChecksums,

		BlobOptimized = FastAllocate | CacheRecordClusters | IntegrityChecks,

		Debug = IntegrityChecks 

	}
}
