// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen;

/// <summary>
/// Describes where a clustered stream begins and ends within the cluster map, along with its logical size.
/// </summary>
public struct ClusteredStreamDescriptor {

	public ClusteredStreamTraits Traits { get; set; }

	public long StartCluster { get; set; }

	public long EndCluster { get; set; }

	public long Size { get; set; }

	public override string ToString() => $"[{nameof(ClusteredStreamDescriptor)}] {nameof(Size)}: {Size}, {nameof(StartCluster)}: {StartCluster}, {nameof(EndCluster)}: {EndCluster}, {nameof(Traits)}: {Traits}]";
}
