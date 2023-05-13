// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Runtime.InteropServices;

namespace Hydrogen {

	//[StructLayout(LayoutKind.Sequential)]
	public struct ClusteredStreamRecord {

		public ClusteredStreamTraits Traits { get; set; }

		public int Size { get; set; }

		public int StartCluster { get; set; }

		public int KeyChecksum { get; set; }

		public byte[] Key { get; set; }

		public override string ToString() => $"[{nameof(ClusteredStreamRecord)}] {nameof(Size)}: {Size}, {nameof(StartCluster)}: {StartCluster}, {nameof(Traits)}: {Traits}, {nameof(KeyChecksum)}: {KeyChecksum}, {nameof(Key)}: {Key?.ToHexString(true)}";
	}
	
}
