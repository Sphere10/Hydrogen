// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.IO;

namespace Hydrogen.ObjectSpaces;

/// <summary>
/// In-memory object space that stores data in a <see cref="MemoryStream"/> while reusing the same object space plumbing as file-backed instances.
/// </summary>
public class MemoryObjectSpace : ObjectSpace {

	public MemoryObjectSpace(
		ObjectSpaceDefinition objectSpaceDefinition, 
		SerializerFactory serializerFactory, 
		ComparerFactory comparerFactory, 
		int clusterSize = HydrogenDefaults.ClusterSize, 
		ClusteredStreamsPolicy clusteredStreamsPolicy = HydrogenDefaults.ContainerPolicy, 
		Endianness endianness = HydrogenDefaults.Endianness
	) : this(new MemoryStream(), objectSpaceDefinition, serializerFactory, comparerFactory, clusterSize, clusteredStreamsPolicy, endianness) {
		Streams.OwnsStream = true;
	}

	public MemoryObjectSpace(
		MemoryStream memoryStream,
		ObjectSpaceDefinition objectSpaceDefinition, 
		SerializerFactory serializerFactory, 
		ComparerFactory comparerFactory, 
		int clusterSize = HydrogenDefaults.ClusterSize, 
		ClusteredStreamsPolicy clusteredStreamsPolicy = HydrogenDefaults.ContainerPolicy, 
		Endianness endianness = HydrogenDefaults.Endianness
	) : base(CreateStreams(memoryStream, clusterSize, clusteredStreamsPolicy, endianness, objectSpaceDefinition.Traits.HasFlag(ObjectSpaceTraits.Merkleized)), objectSpaceDefinition, serializerFactory, comparerFactory) {
		Guard.ArgumentNotNull(objectSpaceDefinition, nameof(objectSpaceDefinition));
		Guard.ArgumentNotNull(serializerFactory, nameof(serializerFactory));
		Guard.ArgumentNotNull(comparerFactory, nameof(comparerFactory));
		Stream = memoryStream;
		Load();
	}

	public MemoryStream Stream { get; }

	private static ClusteredStreams CreateStreams(MemoryStream memoryStream, int clusterSize, ClusteredStreamsPolicy clusteredStreamsPolicy, Endianness endianness, bool merkleized) {
		var objectSpaceMetaDataStreamCount = merkleized ? 1 : 0;
		var streams = new ClusteredStreams(
			memoryStream,
			clusterSize,
			clusteredStreamsPolicy,
			objectSpaceMetaDataStreamCount,
			endianness,
			false
		);
		return streams;
	}

	
}
