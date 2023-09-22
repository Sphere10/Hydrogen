using System;
using System.Collections.Generic;
using System.Linq;

namespace Hydrogen;

public class CompositeSerializer<TItem> : IItemSerializer<TItem> {

	private readonly Func<TItem> _activator;
	private readonly MemberSerializationBinding[] _memberBindings;
	private readonly SizeDescriptorSerializer _sizeDescriptorSerializer;

	public CompositeSerializer(Func<TItem> activator, MemberSerializationBinding[] memberBindings) {
		Guard.ArgumentNotNull(activator, nameof(activator));
		Guard.ArgumentNotNull(memberBindings, nameof(memberBindings));
		_activator = activator;
		_memberBindings = memberBindings;
		_sizeDescriptorSerializer = new SizeDescriptorSerializer(SizeDescriptorStrategy.UseCVarInt);
	}

	public bool SupportsNull => false;

	public bool IsConstantLength => _memberBindings.All(x => x.Serializer.IsConstantLength);
	
	public long ConstantLength => _memberBindings.Sum(x => x.Serializer.ConstantLength);

	public long CalculateTotalSize(IEnumerable<TItem> items, bool calculateIndividualItems, out long[] itemSizes) {
		var itemsArr = items as TItem[] ?? items.ToArray();
		itemSizes = null;
		var totalSize = 0L;
		foreach(var (member, serializer) in _memberBindings) {
			var itemMemberValues = itemsArr.Select(item => member.GetValue(item)).ToArray();
			var itemMemberSizes = itemMemberValues.Select(serializer.CalculateSize).ToArray();
			if (serializer is not IAutoSizedSerializer) {
				for(var  i = 0; i < itemMemberSizes.Length; i++) {
					itemMemberSizes[i] += _sizeDescriptorSerializer.CalculateSize(itemMemberSizes[i]);
				}
			} 
			totalSize += itemMemberSizes.Sum();

			if (calculateIndividualItems) {
				if (itemSizes == null) {
					itemSizes = itemMemberSizes;
				} else {
					Guard.Ensure(itemMemberSizes.Length == itemSizes.Length, "Member item size arrays were inconsistent (serializer bug).");
					for (var i = 0; i < itemSizes.Length; i++) {
						itemSizes[i] += itemMemberSizes[i];
					}
				}
			}
		}
		return totalSize;
	}

	public long CalculateSize(TItem item) {
		var size = 0L;
		foreach(var binding in _memberBindings) {
			var memberValue = binding.Member.GetValue(item);
			var memberSerializer = binding.Serializer;
			var itemSize = memberSerializer.CalculateSize(memberValue);
			if (binding.Serializer is not IAutoSizedSerializer) 
				size += _sizeDescriptorSerializer.CalculateSize(itemSize);
			size += itemSize;
		}
		return size;
	}
		
	public void SerializeInternal(TItem item, EndianBinaryWriter writer) {
		foreach (var binding in _memberBindings) {
			var memberValue = binding.Member.GetValue(item);
			if (binding.Serializer is not IAutoSizedSerializer) {
				var memberSize = binding.Serializer.CalculateSize(memberValue);
				_sizeDescriptorSerializer.SerializeInternal(memberSize, writer);
			}
			binding.Serializer.SerializeInternal(memberValue, writer);
		}
	}

	public TItem DeserializeInternal(long byteSize, EndianBinaryReader reader) {
		var item = _activator();
		foreach (var binding in _memberBindings) {
			object memberValue;
			if (binding.Serializer is IAutoSizedSerializer autoSizedSerializer) {
				memberValue = autoSizedSerializer.DeserializeInternal(reader);
			} else {
				var memberSize = _sizeDescriptorSerializer.Deserialize(reader);
				memberValue = binding.Serializer.DeserializeInternal(memberSize, reader);
			}
			binding.Member.SetValue(item, memberValue);
		}
		return item;
	}
	
}
