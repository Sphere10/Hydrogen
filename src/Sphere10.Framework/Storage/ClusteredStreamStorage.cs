using System;
using System.Collections.Generic;
using System.IO;

namespace Sphere10.Framework {
	/// <summary>
	/// A stream container which interlaces item streams together over a single logical stream using a clustering approach similar to how physical disk drives store data. This
	/// component can be used to store multiple streams over a single file, all of whom can be dynamically sized.  The implementation is optimized for arbitrarily large data scenarios
	/// without space/time/memory complexity issues and no load-time required. It is suitable as a general-purpose file-format for storing an application static and/or dynamic data.
	/// </summary>
	/// <remarks>
	/// [HEADER] Version: 1, Cluster Size: 32, Total Clusters: 10, Listings: 5
	/// [Listings]
	///   0: [StreamListing] Size: 60, Start: 3          ;/ Start means index into Clusters, Size is in bytes
	///   1: [StreamListing] Size: 88, Start: 7
	///   2: [StreamListing] Size: 27, Start: 2
	///   3: [StreamListing] Size: 43, Start: 1
	///   4: [StreamListing] Size: 0, Start: -1
	/// [Clusters]
	///   0: [Cluster] Traits: First, Listing, Prev: -1, Next: 6, Data: 030000003c0000000700000058000000020000001b000000010000002b000000
	///   1: [Cluster] Traits: First, Data, Prev: 3, Next: 5, Data: 894538851b6655bb8d8a4b4517eaab2b22ada63e6e0000000000000000000000
	///   2: [Cluster] Traits: First, Data, Prev: 2, Next: -1, Data: 1e07b1f66b3a237ed9f438ec26093ca50dd05b798baa7de25f093f0000000000
	///   3: [Cluster] Traits: First, Data, Prev: 0, Next: 9, Data: ce178efbff3e3177069101b78453de5ca2d1a7d72c958485306fb400e0efc1f5
	///   4: [Cluster] Traits: Data, Prev: 8, Next: -1, Data: a3058b9856aaf271ab21153c040a05c15042abbf000000000000000000000000
	///   5: [Cluster] Traits: Data, Prev: 1, Next: -1, Data: 0000000000000000000000000000000000000000000000000000000000000000
	///   6: [Cluster] Traits: Listing, Prev: 0, Next: -1, Data: ffffffff00000000000000000000000000000000000000000000000000000000
	///   7: [Cluster] Traits: First, Data, Prev: 1, Next: 8, Data: 5aa2c04b9554fbe9425c2d52aa135ed8107bf9edbf44848326eb92cc9434b828
	///   8: [Cluster] Traits: Data, Prev: 7, Next: 4, Data: c612bcb3e59fd0d7d88240797e649b5020d5090682c0f3151e3c24a9c12e540d
	///   9: [Cluster] Traits: Data, Prev: 3, Next: -1, Data: 594ebf3d9241c837ffa3dea9ab0e550516ad18ed0f7b9c000000000000000000
	///
	///  Notes:
	///  - Header is fixed 256b, and can be expanded to include other data (passwords, merkle-roots, etc)
	///  - Clusters are bi-directionally linked, to allow dynamic expansion/contraction of root stream on-the-fly 
	///  - Listings contain the meta-data of all the streams and the entire listings stream is also serialized over clusters.
	///  - Cluster traits distinguish listing clusters from stream clusters. 
	///  - Cluster 0, when allocated, is always the first listing cluster.
	///  - Listings always link to the (First | Data) cluster of their stream.
	///  - Clusters with traits (First | Data) re-purpose the Prev field to denote the listing.
	/// </remarks>
	public class ClusteredStreamStorage : ClusteredStreamStorage<ClusteredStreamStorageHeader, ClusteredStreamListing>, IStreamStorage {

		public ClusteredStreamStorage(Stream rootStream, int clusterSize, Endianness endianness = Endianness.LittleEndian, ClusteredStreamCachePolicy listingsCachePolicy = ClusteredStreamCachePolicy.None) 
			: base(rootStream, clusterSize, new ClusteredStreamListingSerializer(), endianness, listingsCachePolicy) {
		}

		public static ClusteredStreamStorage Load(Stream rootStream, Endianness endianness = Endianness.LittleEndian, ClusteredStreamCachePolicy listingsCachePolicy = ClusteredStreamCachePolicy.None) {
			if (rootStream.Length < ClusteredStreamStorageHeader.ByteLength)
				throw new CorruptDataException($"Corrupt header (stream was too small {rootStream.Length} bytes)");
			var reader = new EndianBinaryReader(EndianBitConverter.For(endianness), rootStream);
			rootStream.Position = 1;
			var clusterSize = reader.ReadInt32();
			if (clusterSize <= 0)
				throw new CorruptDataException($"Corrupt header (ClusterSize field was {clusterSize} bytes)");
			rootStream.Position = 0;
			return new ClusteredStreamStorage(rootStream, clusterSize, endianness, listingsCachePolicy);
		}

	}

}
