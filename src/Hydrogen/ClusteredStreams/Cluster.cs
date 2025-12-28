// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen;

/// <summary>
/// Represents a single cluster within a cluster chain, storing links to neighboring clusters and the raw data payload.
/// </summary>
public class Cluster {
	public const long Null = -1L;

	public ClusterTraits Traits { get; set; }
	public long Prev { get; set; }
	public long Next { get; set; }
	public byte[] Data { get; set; }

	public override string ToString() => $"[{nameof(Cluster)}] {nameof(Traits)}: {Traits}, {nameof(Prev)}: {Prev}, {nameof(Next)}: {Next}, {nameof(Data)}: {Data.ToHexString(true)}";
}
