using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Sphere10.Framework {

    /// <summary>
    /// A memory-paged byte list that maintains a merkle-tree of every page. It is a decorator since the underlying byte list could be a binary file,
    /// transactional file, etc.
    /// </summary>
    public class MerkleBuffer<TPage> : MemoryPagedListDecorator<byte, TPage>, IMerkleList<byte> where TPage : IMemoryPage<byte> {
        private MerkleTreeImpl _merkleTree;
        private BitArray _merklePagesDirty;
        private readonly byte[] _defaultLeafValue;

        public MerkleBuffer(MemoryPagedList<byte, TPage> pagedList, CHF chf)
            : this(pagedList, new FlatMerkleTree(chf)) {
        }

        public MerkleBuffer(IMemoryPagedList<byte, TPage> pagedList, IUpdateableMerkleTree merkleTreeImpl)
            : base(pagedList) {
            Guard.ArgumentNotNull(pagedList, nameof(pagedList));
            Guard.ArgumentNotNull(merkleTreeImpl, nameof(merkleTreeImpl));
            Guard.Argument(pagedList.Count == 0, nameof(pagedList), "Must be empty");
            Guard.Argument(merkleTreeImpl.Leafs.Count == 0, nameof(pagedList), "Must be empty");
           
            _merklePagesDirty = new BitArray(pagedList.Pages.Count);
            _merkleTree = new MerkleTreeImpl(this, merkleTreeImpl);
            _defaultLeafValue = Tools.Array.Gen<byte>(Hashers.GetDigestSizeBytes(_merkleTree.HashAlgorithm), 0);
            if (!pagedList.RequiresLoad)
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

        protected override void OnPageCreated(TPage page) {
            base.OnPageCreated(page);
            Guard.Ensure(page.Number == _merkleTree.Leafs.Count, "Unexpected page number");
            _merkleTree.Leafs.Add(_defaultLeafValue);
            _merklePagesDirty.Length++;
            MarkMerkleDirty(page, true);
        }

        protected override void OnPageWriting(TPage page) {
            base.OnPageWriting(page);
            MarkMerkleDirty(page, true);
        }

        protected override void OnPageUnloading(TPage page) {
            base.OnPageUnloading(page);
            if (IsMerkleDirty(page))
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

        private void ComputePageHash(TPage page) {
            Guard.Ensure(page.State == PageState.Loaded);
            Debug.Assert(IsMerkleDirty(page)); // shouldn't compute when not dirty
            if (page is IBinaryPage binPage) {
                _merkleTree.Leafs[page.Number] = Hashers.Hash(_merkleTree.HashAlgorithm, binPage.ReadSpan(0, page.Count));
            } else {
                _merkleTree.Leafs[page.Number] = Hashers.Hash(_merkleTree.HashAlgorithm, page.Read(0, page.Count).ToArray());
            }
            MarkMerkleDirty(page, false);
        }

        private bool IsMerkleDirty(TPage page) => _merklePagesDirty[page.Number];

        private void MarkMerkleDirty(TPage page, bool dirty) => _merklePagesDirty[page.Number] = dirty;

        private class MerkleTreeImpl : UpdatableMerkleTreeDecorator {
            private readonly MerkleBuffer<TPage> _parent;
            public MerkleTreeImpl(MerkleBuffer<TPage> parent, IUpdateableMerkleTree internalMerkleTree)
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