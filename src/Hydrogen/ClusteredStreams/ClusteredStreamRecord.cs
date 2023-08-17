// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen;

//[StructLayout(LayoutKind.Sequential)]
public struct ClusteredStreamRecord {
	internal const int TraitsLength = sizeof(byte);
	internal const int StartClusterLength = sizeof(long);
	internal const int EndClusterLength = sizeof(long);
	internal const int SizeLength = sizeof(long);
	internal const int KeyChecksumLength = sizeof(int);

	internal const long TraitsOffset = 0;
	internal const long StartClusterOffset = TraitsOffset + TraitsLength;
	internal const long EndClusterOffset = StartClusterOffset + StartClusterLength;
	internal const long SizeOffset = EndClusterOffset + EndClusterLength;
	internal const long KeyChecksumOffset = SizeOffset + SizeLength;
	internal const long KeyOffset = KeyChecksumOffset + KeyChecksumLength;

	public ClusteredStreamTraits Traits { get; set; }

	public long StartCluster { get; set; }

	public long EndCluster { get; set; }

	public long Size { get; set; }

	public int KeyChecksum { get; set; }

	public byte[] Key { get; set; }

	public override string ToString() => $"[{nameof(ClusteredStreamRecord)}] {nameof(Size)}: {Size}, {nameof(StartCluster)}: {StartCluster}, {nameof(EndCluster)}: {EndCluster}, {nameof(Traits)}: {Traits}, {nameof(KeyChecksum)}: {KeyChecksum}, {nameof(Key)}: {Key?.ToHexString(true)}";
}
