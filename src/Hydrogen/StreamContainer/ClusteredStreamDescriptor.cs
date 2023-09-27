// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen;

public struct ClusteredStreamDescriptor {

	public ClusteredStreamTraits Traits { get; set; }

	public long StartCluster { get; set; }

	public long EndCluster { get; set; }

	public long Size { get; set; }

	public override string ToString() => $"[{nameof(ClusteredStreamDescriptor)}] {nameof(Size)}: {Size}, {nameof(StartCluster)}: {StartCluster}, {nameof(EndCluster)}: {EndCluster}, {nameof(Traits)}: {Traits}]";
}
