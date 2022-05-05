using Hydrogen;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hydrogen.DApp.Core.Blockchain {
	public interface IBlockchainLayer<TBlock> {

		byte[] StateRoot { get; }

		ApplyBlockResult ApplyTipBlock(TBlock block);

		UndoBlockResult UndoTipBlock(TBlock block);
	}


	public interface IBlockRepository<TBlock> {

		void Initialize(Progress<int> progressCallback);

		IBlockTree BlockTree { get; }

		bool TryLoadBlock(long blockID, out Future<Stream> blockData);

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
}
