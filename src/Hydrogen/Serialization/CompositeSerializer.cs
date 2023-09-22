using System;
using System.Collections.Generic;
using System.Linq;

namespace Hydrogen;

public class CompositeSerializer<TItem> : ItemSerializer<TItem> {
	private readonly Func<TItem> _activator;
	private readonly MemberSerializationBinding[] _memberBindings;

	public CompositeSerializer(Func<TItem> activator, MemberSerializationBinding[] memberBindings) 
		: base(SizeDescriptorStrategy.UseCVarInt) {
		Guard.ArgumentNotNull(activator, nameof(activator));
		Guard.ArgumentNotNull(memberBindings, nameof(memberBindings));
		_activator = activator;
		_memberBindings = memberBindings;
		IsConstantSize = _memberBindings.All(x => x.Serializer.IsConstantSize);
		ConstantSize =  IsConstantSize ? _memberBindings.Sum(x => x.Serializer.ConstantSize) : -1;
	}

	public override bool SupportsNull => false;

	public override bool IsConstantSize { get; }
	
	public override long ConstantSize { get; }

	public override long CalculateTotalSize(IEnumerable<TItem> items, bool calculateIndividualItems, out long[] itemSizes) {
		var itemsArr = items as TItem[] ?? items.ToArray();
		itemSizes = null;
		var totalSize = 0L;
		foreach(var (member, serializer) in _memberBindings) {
			var itemMemberValues = itemsArr.Select(item => member.GetValue(item)).ToArray();
			var itemMemberSizes = itemMemberValues.Select(serializer.CalculateSize).ToArray();
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

	public override long CalculateSize(TItem item) {
		var size = 0L;
		foreach(var binding in _memberBindings) {
			var memberValue = binding.Member.GetValue(item);
			var memberSerializer = binding.Serializer;
			var itemSize = memberSerializer.CalculateSize(memberValue);
			size += itemSize;
		}
		return size;
	}
		
	public override void SerializeInternal(TItem item, EndianBinaryWriter writer) {
		foreach (var binding in _memberBindings) {
			var memberValue = binding.Member.GetValue(item);
			binding.Serializer.SerializeInternal(memberValue, writer);
		}
	}

	public override TItem DeserializeInternal(EndianBinaryReader reader) {
		var item = _activator();
		foreach (var binding in _memberBindings) {
			var memberValue = binding.Serializer.DeserializeInternal(reader);
			binding.Member.SetValue(item, memberValue);
		}
		return item;
	}
	
}
