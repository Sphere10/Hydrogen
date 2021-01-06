//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;

//namespace Sphere10.Framework {

//	/// <summary>
//	/// A memory-paged byte list that maintains a merkle-tree of every page. It is a decorator since the underlying byte list could be a binary file,
//	/// transactional file, etc.
//	/// </summary>
//	public class MerkleBuffer<TPage> : MemoryPagedListDecorator<byte, TPage> where TPage : IMemoryPage<byte> {
//		private IUpdateableMerkleTree _merkleTreeImpl;
//		private IDictionary<TPage, bool> _merkleDirtyPages;

//		public MerkleBuffer(IMemoryPagedList<byte, TPage> pagedList, IUpdateableMerkleTree merkleTreeImpl)
//			: base(pagedList) {
//			_merkleTreeImpl = merkleTreeImpl;
//			_merkleDirtyPages = new Dictionary<TPage, bool>();

//			pagedList.PageLoaded += (c, p) => ComputeHash(p);
//			pagedList.PageWriting += (c, p) => MarkDirty(p);
//			pagedList.PageUnloading += (c, p) => ComputeHash(p);
//		}

//		public bool MerkleDirty => base.Pages.Cast<MerklizedBufferPage>().Any(x => x.MerkleDirty);



//		public CHF HashAlgorithm => _merkleTreeImpl.HashAlgorithm;

//		public byte[] Root {
//			get {
//				EnsureMerkleClean();
//				return _merkleTreeImpl.Root;
//			}
//		}

//		MerkleSize IMerkleTree.Size => _merkleTreeImpl.Size;

//		public ReadOnlySpan<byte> GetValue(MerkleCoordinate coordinate) {
//			EnsureMerkleClean();
//			return _merkleTreeImpl.GetValue(coordinate);
//		}

//		private void EnsureMerkleClean() {
//			var merkleDirtyPages = base.Pages.Cast<MerklizedBufferPage>().Where(x => x.MerkleDirty).ToArray();
//			var merkleDirtyLoadedPages = merkleDirtyPages.Where(x => x.State == PageState.Loaded).ToArray();
//			// already loaded pages compute first
//			foreach (var page in merkleDirtyLoadedPages)
//				page.ComputeHash();

//			// compute rest of dirty pages
//			foreach (var page in merkleDirtyPages.Except(merkleDirtyLoadedPages)) {
//				using (EnterOpenPageScope(page)) {
//					page.ComputeHash();
//				}
//			}
//		}

//		public class MerklizedBufferPage : BufferPage {
//			private byte[] _hash;

//			public MerklizedBufferPage(int pageSize, CHF chf)
//				: base(pageSize) {
//				HashAlgorithm = chf;
//				MerkleDirty = true;
//			}

//			public CHF HashAlgorithm { get; }

//			public bool MerkleDirty { get; internal set; }

//			public byte[] Hash {
//				get {
//					ComputeHash();
//					return _hash;
//				}
//			}

//			public override void Unload() {
//				ComputeHash();
//				base.Unload();
//			}

//			internal void ComputeHash() {
//				if (MerkleDirty) {
//					_hash = Hashers.Hash(HashAlgorithm, base.MemoryStore.ReadSpan(0, MemoryStore.Count));
//					MerkleDirty = false;
//				}
//			}
//		}
//	}

//}