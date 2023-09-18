using System;
using System.Collections.Generic;
using System.Linq;

namespace Hydrogen;

public class CompositeSerializer<TItem> : IItemSerializer<TItem> {

	private readonly Func<TItem> _activator;
	private readonly MemberSerializationBinding[] _memberBindings;

	public CompositeSerializer(Func<TItem> activator, MemberSerializationBinding[] memberBindings) {
		Guard.ArgumentNotNull(activator, nameof(activator));
		Guard.ArgumentNotNull(memberBindings, nameof(memberBindings));
		_activator = activator;
		_memberBindings = memberBindings;
	}

	public bool IsConstantLength => _memberBindings.All(x => x.Serializer.IsConstantLength);
	public long ConstantLength => _memberBindings.Sum(x => x.Serializer.ConstantLength);

	public long CalculateTotalSize(IEnumerable<TItem> items, bool calculateIndividualItems, out long[] itemSizes) {
		var itemsArr = items as TItem[] ?? items.ToArray();
		itemSizes = null;
		var totalSize = 0L;
		foreach(var binding in _memberBindings) {
			totalSize += binding.Serializer.CalculateTotalSize(itemsArr.Select(x => binding.Member.GetValue(x)), calculateIndividualItems, out var memberItemSizes);
			if (calculateIndividualItems) {
				if (itemSizes == null) {
					itemSizes = memberItemSizes;
				} else {
					Guard.Ensure(memberItemSizes.Length == itemSizes.Length, "Member item size arrays were inconsistent (serializer bug).");
					for (var i = 0; i < itemSizes.Length; i++) {
						itemSizes[i] += memberItemSizes[i];
					}
				}
			}
		}
		return totalSize;
	}

	public long CalculateSize(TItem item) => _memberBindings.Sum(x => x.Serializer.CalculateSize(x.Member.GetValue(item)));

	public void SerializeInternal(TItem item, EndianBinaryWriter writer) {
		foreach (var binding in _memberBindings) {
			var memberValue = binding.Member.GetValue(item);
			binding.Serializer.SerializeInternal(memberValue, writer);
		}
	}

	public TItem DeserializeInternal(long byteSize, EndianBinaryReader reader) {
		var item = _activator();
		foreach (var binding in _memberBindings) {
			var memberValue = binding.Serializer.DeserializeInternal(byteSize, reader);
			binding.Member.SetValue(item, memberValue);
		}
		return item;
	}
}
