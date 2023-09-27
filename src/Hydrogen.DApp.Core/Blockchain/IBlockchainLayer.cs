// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.IO;

namespace Hydrogen.DApp.Core.Blockchain;

public interface IBlockchainLayer<TBlock> {

	byte[] StateRoot { get; }

	ApplyBlockResult ApplyTipBlock(TBlock block);

	UndoBlockResult UndoTipBlock(TBlock block);
}


public interface IBlockRepository<TBlock> {

	void Initialize(Progress<int> progressCallback);

	IBlockTree BlockTree { get; }

	bool TryLoadBlock(long blockID, out IFuture<Stream> blockData);

	bool TrySaveBlock(Stream blockData, out long blockID);

}


public interface IBlockchainRepository {
	long Height { get; }

	bool TryGetBlock(long blockNo, out long blockID);
}


public abstract class BlockchainLayerBase<TBlock> : IBlockchainLayer<TBlock> {
	public abstract byte[] StateRoot { get; }

	public ApplyBlockResult ApplyTipBlock(TBlock block) {
		throw new NotImplementedException();
	}

	public UndoBlockResult UndoTipBlock(TBlock block) {
		throw new NotImplementedException();
	}
}
