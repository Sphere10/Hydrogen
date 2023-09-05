//// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
//// Author: Herman Schoenfeld
////
//// Distributed under the MIT software license, see the accompanying file
//// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
////
//// This notice must not be removed when duplicating this file or its contents, in whole or in part.

//using System;
//using System.Collections.Generic;


//namespace Hydrogen.Collections;

///// <summary>
///// Maps a <see cref="IDynamicMerkleTree"/> onto a <see cref="Stream"/> for use within a <see cref="IClusteredStorage"/>. The tree data 
///// is stored as a <see cref="FlatMerkleTree"/> and mapped to reserved <see cref="ClusteredStreamDescriptor"/> within the <see cref="IClusteredStorage"/>.
///// </summary>
///// <remarks>This class is intended for building <see cref="IMerkleTree"/>'s of items stored in a <see cref="IClusteredStorage"/>-based containers.</remarks>
//internal sealed class StreamContainerMerkleTreeStream : IDynamicMerkleTree {

//	private readonly StreamContainer _streams;
//	private readonly int _streamIndex;

//	private StreamMappedProperty<byte[]> _merkleRootProperty;

//	public StreamContainerMerkleTreeStream(StreamContainer streamContainer, int streamIndex, CHF hashAlgorithm) {
//		_streams = streamContainer;
//		_streamIndex = streamIndex;
//		HashAlgorithm = hashAlgorithm;
//		Leafs = new LeafList(this);
//		_merkleRootProperty = null; // set on initialization
//		streamContainer.RegisterInitAction(Initialize);
//	}

//	private void Initialize() {
//		var hashSize = Hashers.GetDigestSizeBytes(HashAlgorithm);
//		using (_streams.EnterAccessScope()) {
//			_merkleRootProperty = _streams.Header.CreateExtensionProperty(0, hashSize, new StaticSizeByteArraySerializer(hashSize).WithNullSubstitution(Hashers.ZeroHash(HashAlgorithm), ByteArrayEqualityComparer.Instance));
//		}
//		_streams.Cleared += () => Root = null;
//	}

//	public CHF HashAlgorithm { get; }

//	public byte[] Root {
//		get => _merkleRootProperty.Value;
//		set => _merkleRootProperty.Value = value;
//	}

//	public MerkleSize Size => MerkleSize.FromLeafCount(_streams.Count - _streams.Header.ReservedStreams);

//	public ReadOnlySpan<byte> GetValue(MerkleCoordinate coordinate) {
//		using (EnterAccessMerkleTreeScope(out var merkleTree)) {
//			return merkleTree.GetValue(coordinate);
//		}
//	}
//	public IExtendedList<byte[]> Leafs { get; }

//	private IDisposable EnterAccessMerkleTreeScope(out IDynamicMerkleTree merkleTree) {
//		var disposables = new Disposables(false);
//		var stream = _streams.OpenWrite(_streamIndex);
//		var flatTreeData = new StreamMappedBuffer(stream);
//		var flatMerkleTree = new FlatMerkleTree(HashAlgorithm, flatTreeData, _streams.Count - _streams.Header.ReservedStreams);
//		merkleTree = flatMerkleTree;
//		disposables.Add(() => Root = flatMerkleTree.Root); // When scope finishes, update streams merkle-root in header
//		disposables.Add(stream); // when scope finishes, dispose the stream scope
//		return disposables;
//	}


//	private sealed class LeafList : RangedListBase<byte[]> {
//		private readonly StreamContainerMerkleTreeStream _parent;
//		public LeafList(StreamContainerMerkleTreeStream parent) {
//			_parent = parent;
//		}

//		public override long Count => _parent._streams.Count - _parent._streams.Header.ReservedStreams;

//		public override void AddRange(IEnumerable<byte[]> items) {
//			using (_parent.EnterAccessMerkleTreeScope(out var merkleTree)) {
//				merkleTree.Leafs.AddRange(items);
//			}
//		}

//		public override IEnumerable<long> IndexOfRange(IEnumerable<byte[]> items) {
//			using (_parent.EnterAccessMerkleTreeScope(out var merkleTree)) {
//				return merkleTree.Leafs.IndexOfRange(items);
//			}
//		}

//		public override IEnumerable<byte[]> ReadRange(long index, long count) {
//			using (_parent.EnterAccessMerkleTreeScope(out var merkleTree)) {
//				return merkleTree.Leafs.ReadRange(index, count);
//			}
//		}

//		public override void UpdateRange(long index, IEnumerable<byte[]> items) {
//			using (_parent.EnterAccessMerkleTreeScope(out var merkleTree)) {
//				merkleTree.Leafs.UpdateRange(index, items);
//			}
//		}

//		public override void InsertRange(long index, IEnumerable<byte[]> items) {
//			using (_parent.EnterAccessMerkleTreeScope(out var merkleTree)) {
//				merkleTree.Leafs.InsertRange(index, items);
//			}
//		}

//		public override void RemoveRange(long index, long count) {
//			using (_parent.EnterAccessMerkleTreeScope(out var merkleTree)) {
//				merkleTree.Leafs.RemoveRange(index, count);
//			}
//		}
//	}

//}
