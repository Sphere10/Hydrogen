using System;

namespace Sphere10.Framework.ObjectSpace {

	// TODO:
	// - ObjectSpace registers object mappers, that maps objects to their boxes
	// - ObjectSpace maps object to their object boxes
	// - ObjectBox is a StreamMappedList for F
	// - Composite objects re

	public abstract class FixedSizeObject {
		public ulong ID { get; protected set; }  // typically is the offset in the stored stream
		public abstract int Size { get; }

	}

	//public class ObjectSpace {

	//    IDictionary<Type, object> _objectBoxes;

	//    public void SaveObject
	//}

	public class ObjectBox<T> : ExtendedListDecorator<T> where T : FixedSizeObject {

		public ObjectBox(string filename, Guid fileID, string pageDir, int logicalPageSize, int merklePageSize, IItemSerializer<T> serializer, CHF merkleCHF, int memoryCacheBytes, bool readOnly)
			: base(CreateInternalList(filename, fileID, pageDir, logicalPageSize, merklePageSize, serializer, merkleCHF, memoryCacheBytes, readOnly, out var txnFile, out var merkleTree)) {
			TransactionalFile = txnFile;
			StorageMerkleTree = merkleTree;
		}

		public ITransactionalFile TransactionalFile { get; }

		public IMerkleTree StorageMerkleTree { get; }


		public static IExtendedList<T> CreateInternalList(string filename, Guid fileID, string pageDir, int logicalPageSize, int merklePageSize, IItemSerializer<T> serializer, CHF merkleCHF, int memoryCacheBytes, bool readOnly, out ITransactionalFile txnFile, out IMerkleTree storageMerkleTree) {
			var file = new TransactionalFileMappedBuffer(
				filename,
				pageDir,
				merklePageSize,
				(memoryCacheBytes / merklePageSize).ClipTo(1, int.MaxValue),
				readOnly
			);

			var merkleFile = new MerkleBuffer(file, merkleCHF);
			var stream = new ExtendedMemoryStream(file);
			txnFile = file;
			storageMerkleTree = merkleFile.MerkleTree;
			return new StreamPagedList<T>(serializer, stream, logicalPageSize);
		}

	}
}
