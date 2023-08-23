using System.Collections.Generic;

namespace Hydrogen;

public class PackedSerializer : IItemSerializer<object> {
	private readonly object _serializer;
	private readonly IItemSerializer<object> _projectedSerializer;

	private PackedSerializer(object serializer, IItemSerializer<object> projectedSerializer) {
		Guard.ArgumentNotNull(serializer, nameof(serializer));
		Guard.ArgumentNotNull(projectedSerializer, nameof(projectedSerializer));
		Guard.Argument(serializer.GetType().IsSubtypeOfGenericType(typeof(IItemSerializer<>)), nameof(projectedSerializer), $"Must be an ItemSerializer<>");	
		Guard.Argument(projectedSerializer.GetType().IsSubtypeOfGenericType(typeof(ProjectedSerializer<,>)), nameof(projectedSerializer), "Must be an ProjectedSerializer<,>");	
		_serializer = serializer;
		_projectedSerializer = projectedSerializer;
	}

	public bool IsStaticSize => _projectedSerializer.IsStaticSize;
	public long StaticSize => _projectedSerializer.StaticSize;

	public long CalculateTotalSize(IEnumerable<object> items, bool calculateIndividualItems, out long[] itemSizes) 
		=> _projectedSerializer.CalculateTotalSize(items, calculateIndividualItems, out itemSizes);

	public long CalculateSize(object item) 
		=> _projectedSerializer.CalculateSize(item);

	public void SerializeInternal(object item, EndianBinaryWriter writer) 
		=> _projectedSerializer.SerializeInternal(item, writer);

	public object DeserializeInternal(long byteSize, EndianBinaryReader reader) 
		=> _projectedSerializer.DeserializeInternal(byteSize, reader);
	
	public static PackedSerializer Pack<TItem>(IItemSerializer<TItem> serializer) {
		Guard.ArgumentNotNull(serializer, nameof(serializer));
		return new PackedSerializer(serializer, serializer.AsProjection(x => (object)x, x => (TItem)x));
	}

	public IItemSerializer<TItem> Unpack<TItem>() {
		var unpacked = _serializer as IItemSerializer<TItem>;
		Guard.Ensure(unpacked != null, $"Cannot unpack {_serializer.GetType().Name} as is not an {nameof(IItemSerializer<TItem>)}");
		return unpacked;
	}

}
