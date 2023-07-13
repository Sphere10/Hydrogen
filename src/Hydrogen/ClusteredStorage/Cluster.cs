// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen;

internal class Cluster {
	public const int TraitsLength = sizeof(byte);
	public const int PrevLength = sizeof(long);
	public const int NextLength = sizeof(long);

	public const long TraitsOffset = 0;
	public const long PrevOffset = TraitsOffset + TraitsLength;
	public const long NextOffset = PrevOffset + PrevLength;
	public const long DataOffset = NextOffset + NextLength;

	public ClusterTraits Traits { get; set; }
	public long Prev { get; set; }
	public long Next { get; set; }
	public byte[] Data { get; set; }

	public override string ToString() => $"[{nameof(Cluster)}] {nameof(Traits)}: {Traits}, {nameof(Prev)}: {Prev}, {nameof(Next)}: {Next}, {nameof(Data)}: {Data.ToHexString(true)}";
}
