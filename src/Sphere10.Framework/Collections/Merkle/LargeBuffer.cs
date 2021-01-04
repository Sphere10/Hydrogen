using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sphere10.Framework {

	public class MerkleizedLargeBuffer : LargeBuffer, IMerkleTree {
		private IUpdateableMerkleTree _merkleTreeImpl;

		public MerkleizedLargeBuffer(int pageSize, int inMemoryPages, IUpdateableMerkleTree merkleTreeImpl)
			: base(pageSize, inMemoryPages) {
			_merkleTreeImpl = merkleTreeImpl;
		}

		public bool MerkleDirty => base.Pages.Cast<MerklizedBufferPage>().Any(x => x.MerkleDirty);

		protected override BufferPage NewPageInstance(int pageNumber) {
			return new MerklizedBufferPage(PageSize, _merkleTreeImpl.HashAlgorithm);
		}

		protected override void OnPageLoaded(BufferPage page) {
			base.OnPageLoaded(page);
			// compute merkle node
			// note: only computes when client updates page, not during file swapping
			((MerklizedBufferPage)page).ComputeHash();
		}
		
		protected override void OnPageWriting(BufferPage page) {
			base.OnPageWriting(page);
			// invalidate merkle node
			// note: only computes when client updates page, not during file swapping
			((MerklizedBufferPage)page).MerkleDirty = true;

		}

		protected override void OnPageUnloading(BufferPage page) {
			base.OnPageUnloading(page);
			// note: only computes when client updates page, not during file swapping
			((MerklizedBufferPage)page).ComputeHash();
		}


		public CHF HashAlgorithm => _merkleTreeImpl.HashAlgorithm;

		public byte[] Root {
			get {
				EnsureMerkleClean();
				return _merkleTreeImpl.Root;
			}
		}

		MerkleSize IMerkleTree.Size => _merkleTreeImpl.Size;

		public ReadOnlySpan<byte> GetValue(MerkleCoordinate coordinate) {
			EnsureMerkleClean();
			return _merkleTreeImpl.GetValue(coordinate);
		}

		private void EnsureMerkleClean() {
			var merkleDirtyPages = base.Pages.Cast<MerklizedBufferPage>().Where(x => x.MerkleDirty).ToArray();
			var merkleDirtyLoadedPages = merkleDirtyPages.Where(x => x.State == PageState.Loaded).ToArray();
			// already loaded pages compute first
			foreach(var page in merkleDirtyLoadedPages)
				page.ComputeHash();

			// compute rest of dirty pages
			foreach (var page in merkleDirtyPages.Except(merkleDirtyLoadedPages)) {
				using (EnterOpenPageScope(page)) {
					page.ComputeHash();
				}
			}
		}

		public class MerklizedBufferPage : BufferPage {
			private byte[] _hash;

			public MerklizedBufferPage(int pageSize, CHF chf)
				: base(pageSize) {
				HashAlgorithm = chf;
				MerkleDirty = true;
			}

			public CHF HashAlgorithm { get;  }

			public bool MerkleDirty { get; internal set; }

			public byte[] Hash {
				get {
					ComputeHash();
					return _hash;
				}
			}

			public override void Unload() {
				ComputeHash();
				base.Unload();
			}

			internal void ComputeHash() {
				if (MerkleDirty) {
					_hash = Hashers.Hash(HashAlgorithm, base.MemoryStore.ReadSpan(0, MemoryStore.Count));
					MerkleDirty = false;
				}
			}
		}
	}

}