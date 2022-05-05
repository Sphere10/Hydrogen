using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Hydrogen {

	/// <summary>
	/// A fast serializer for structs which uses <see cref="Marshal"/> class to take an unmanaged in-memory snapshot on the struct.
	/// WARNING: not suitable for structures containing <see cref="Char"/> or <see cref="String"/> fields and not endian neutral, see remarks.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <remarks>
	/// Serialization inconsistencies can arise for structures containing <see cref="Char"/> and <see cref="Strings"/> fields due to the marshalling behaviour for those types.
	/// The way such values are marshalled depends on various factors including the <see cref="CharSet"/> being applied (whether by default or via by an explicit <see cref="StructLayoutAttribute "/>),
	/// by the presence of <see cref="MarshalAsAttribute"/> on such fields and by the values in <see cref="Char"/> fields. For example, without a Unicode <see cref="CharSet"/>, a struct containing
	/// a <see cref="Char"/> field with a unicode value will be considered an ANSI char by <see cref="Marshal.SizeOf"/> but Unicode for the application. Also, an odd
	/// offset of a <see cref="Char"/> field can lead to inconsistent behaviour due in differences in the managed/unmanaged memory representations. 
	///
	/// Thus unless a structure is explicitly tested and confirmed to work, it is generally recommended to avoid using this class for such structures. An un-attributed
	/// structure containing such fields will generally fail to serialize correctly otherwise.
	/// </remarks>
	public class FastMarshalStructSerializer<T> : StaticSizeItemSerializerBase<T> where T : struct {

		public FastMarshalStructSerializer() 
			: base(Marshal.SizeOf(typeof(T))) {
		}

		public override bool TrySerialize(T item, EndianBinaryWriter writer) {
			var bytes = new byte[StaticSize];
			var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
			try {
				var bytesPtr = handle.AddrOfPinnedObject();
				Marshal.StructureToPtr(item, bytesPtr, false);
				writer.Write(bytes);
				return true;
			} finally {
				handle.Free();
			}
		}

		public override bool TryDeserialize(EndianBinaryReader reader, out T item) {
			var bytes = reader.ReadBytes(StaticSize);
			var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
			try {
				var bytesPtr = handle.AddrOfPinnedObject();
				item = (T)Marshal.PtrToStructure(bytesPtr, typeof(T));
				return true;
			} finally {
				handle.Free();
			}
		}
	}
}
