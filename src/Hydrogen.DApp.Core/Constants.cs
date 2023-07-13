// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen.DApp.Core;

public static class Constants {

	public static readonly Func<byte[], byte[]> Hasher = (b) => Hashers.Hash(CHF.SHA2_256, b);

	public static readonly byte[] ZeroHash = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

	public static readonly Func<byte[], byte[]> SealHash = (x) => Hashers.Hash(CHF.Blake2b_160, Hashers.Hash(CHF.SHA2_256, x));

	public static readonly byte[] SealGenesis = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

	public static readonly byte[] SealNull = new byte[0];

	public const int HashLength = 32;

	public const int MinerTagSize = 20;

	public const int BlockHeaderPaddingSize = 66;

	public const int Winternitz_W_Param = 8;

}
