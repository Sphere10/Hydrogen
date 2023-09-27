// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen.DApp.Core.Consensus;

[Serializable]
public class BlockChainTransaction {
	public UInt64 Sequence { get; set; }
	public long TimeStamp { get; set; }
	public string From { get; set; }
	public string To { get; set; }
	public UInt64 Amount { get; set; }
	public UInt64 Fees { get; set; }
	public byte[] Data { get; set; }
}
