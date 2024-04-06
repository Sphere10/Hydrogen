
//namespace Hydrogen;

///// <summary>
///// Serializer wrapper for handling reference types within an ObjectSpace. If a type is tracked within a dimension of the object-space,
///// then this serializer will save it within that dimension rather than within the object itself. 
///// </summary>
//public sealed class ObjectSpaceReferenceSerializer<TItem> : ReferenceSerializer<TItem> {

//	public ObjectSpaceReferenceSerializer(IItemSerializer<TItem> valueSerializer) : base(valueSerializer) {
//	}
//	public ObjectSpaceReferenceSerializer(IItemSerializer<TItem> valueSerializer, ReferenceSerializerMode mode) : base(valueSerializer, mode) {
//	}
//}