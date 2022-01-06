
using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;

namespace Sphere10.Framework {

	/// <summary>
	/// A memory-paged byte list that maintains a merkle-tree of every page. It is a decorator since the underlying byte list could be a binary file,
	/// transactional file, etc.
	/// </summary>
	public class MerkleBuffer : MemoryPagedBufferDecorator, IMerkleList<byte> {
        private readonly MerkleTreeImpl _merkleTree;
        private readonly BitArray _merklePagesDirty;
        private readonly byte[] _defaultLeafValue;

        public MerkleBuffer(IMemoryPagedBuffer buffer, CHF chf)
            : this(buffer, new FlatMerkleTree(chf)) {
        }

        public MerkleBuffer(IMemoryPagedBuffer buffer, IUpdateableMerkleTree merkleTreeImpl)
            : base(buffer) {
            Guard.ArgumentNotNull(buffer, nameof(buffer));
            Guard.ArgumentNotNull(merkleTreeImpl, nameof(merkleTreeImpl));
            Guard.Argument(buffer.Count == 0, nameof(buffer), "Must be empty");
            Guard.Argument(merkleTreeImpl.Leafs.Count == 0, nameof(buffer), "Must be empty");
           
            _merklePagesDirty = new BitArray(buffer.Pages.Count);
            _merkleTree = new MerkleTreeImpl(this, merkleTreeImpl);
            _defaultLeafValue = Tools.Array.Gen<byte>(Hashers.GetDigestSizeBytes(_merkleTree.HashAlgorithm), 0);
            if (!buffer.RequiresLoad)
                OnLoaded(); // decorated paged list was already loaded before passing here, so call it here
            
        }

        public bool MerkleDirty {
            get {
                foreach (bool bit in _merklePagesDirty)
                    if (bit)
                        return true;
                return false;
            }
        }

        public IMerkleTree MerkleTree => _merkleTree;

        protected override void OnLoading() {
            base.OnLoading();
            _merklePagesDirty.Length = 0;
            _merkleTree.Leafs.Clear();
        }

        protected override void OnLoaded() {
            base.OnLoaded();
            ProcessLoadedPages();
        }

        protected override void OnPageCreated(IPage<byte> page) {
            base.OnPageCreated(page);
            Guard.Ensure(page.Number == _merkleTree.Leafs.Count, "Unexpected page number");
            _merkleTree.Leafs.Add(_defaultLeafValue);
            _merklePagesDirty.Length++;
            MarkMerkleDirty(page, true);
        }

		protected override void OnPageDeleted(IPage<byte> page) {
			base.OnPageDeleted(page);
            Guard.Ensure(page.Number == _merkleTree.Leafs.Count - 1);
            _merkleTree.Leafs.RemoveAt(^1);
		}

		protected override void OnPageWriting(IPage<byte> page) {
            base.OnPageWriting(page);
            MarkMerkleDirty(page, true);
        }

        protected override void OnPageUnloading(IMemoryPage<byte> page) {
            base.OnPageUnloading(page);
            if (page.State != PageState.Deleting && IsMerkleDirty(page))
                ComputePageHash(page);
        }

        private void EnsureMerkleClean() {
            var merkleDirtyPages = base.Pages.Where(IsMerkleDirty).ToArray();
            var merkleDirtyLoadedPages = merkleDirtyPages.Where(x => x.State == PageState.Loaded).ToArray();

            // already loaded pages compute first
            foreach (var page in merkleDirtyLoadedPages)
                ComputePageHash(page);

            // compute rest of dirty pages
            foreach (var page in merkleDirtyPages.Except(merkleDirtyLoadedPages))
                using (EnterOpenPageScope(page))
                    ComputePageHash(page);
        }

        private void ProcessLoadedPages() {
            Guard.Ensure(_merkleTree.Leafs.Count == 0);
            Guard.Ensure(_merklePagesDirty.Count == 0);
            _merklePagesDirty.Length = Pages.Count;
            foreach (var page in Pages) {
                Guard.Ensure(page.Number == _merkleTree.Leafs.Count, "Unexpected page number");
                _merkleTree.Leafs.Add(_defaultLeafValue);
                MarkMerkleDirty(page, true);
            }
        }

        private void ComputePageHash(IPage<byte> page) {
            Guard.Ensure(page.State == PageState.Loaded);
            Debug.Assert(IsMerkleDirty(page)); // shouldn't compute when not dirty
            var buffPage = (IBufferPage)page;
            _merkleTree.Leafs[page.Number] = Hashers.Hash(_merkleTree.HashAlgorithm, buffPage.ReadSpan(page.StartIndex, page.Count));
            MarkMerkleDirty(page, false);
        }

        private bool IsMerkleDirty(IPage<byte> page) => _merklePagesDirty[page.Number];

        private void MarkMerkleDirty(IPage<byte> page, bool dirty) => _merklePagesDirty[page.Number] = dirty;

        private class MerkleTreeImpl : UpdatableMerkleTreeDecorator {
            private readonly MerkleBuffer _parent;
            public MerkleTreeImpl(MerkleBuffer parent, IUpdateableMerkleTree internalMerkleTree)
                : base(internalMerkleTree) {
                _parent = parent;
            }

            public override byte[] Root {
                get {
                    _parent.EnsureMerkleClean();
                    return base.Root;
                }
            }

            public override ReadOnlySpan<byte> GetValue(MerkleCoordinate coordinate) {
                _parent.EnsureMerkleClean();
                return base.GetValue(coordinate);
            }

        }
    }

}