// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

[Flags]
public enum ClusteredStreamsPolicy : uint {

	/// <summary>
	/// ClusterMap are pre-allocated when serializing items. This avoids on-the-fly allocation during serialization which can be slow.
	/// </summary>
	FastAllocate = 1 << 0,

	/// <summary>
	/// Cache cluster next/previous pointers (fast traversal of cluster map)
	/// </summary>
	CacheClusterHeaders = 1 << 1,

	/// <summary>
	/// Cache stream descriptors (saves traversing the descriptor stream)
	/// </summary>
	CacheDescriptors = 1 << 2,

	/// <summary>
	/// Performs real-time integrity checks when processing. Disabling this may improve performance slightly.
	/// </summary>
	IntegrityChecks = 1 << 3,

	///// <summary>
	///// Encrypts the underlying data
	///// </summary>
	//Encrypted,

	/// <summary>
	/// Default policy suitable for most use-cases
	/// </summary>
	Default = CacheDescriptors | CacheClusterHeaders | IntegrityChecks,

	Performance = FastAllocate | CacheDescriptors | CacheClusterHeaders,

	Debug = IntegrityChecks

}
