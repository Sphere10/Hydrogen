// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen;

public class Cluster {
	public const long Null = -1L;

	public ClusterTraits Traits { get; set; }
	public long Prev { get; set; }
	public long Next { get; set; }
	public byte[] Data { get; set; }

	public override string ToString() => $"[{nameof(Cluster)}] {nameof(Traits)}: {Traits}, {nameof(Prev)}: {Prev}, {nameof(Next)}: {Next}, {nameof(Data)}: {Data.ToHexString(true)}";
}
