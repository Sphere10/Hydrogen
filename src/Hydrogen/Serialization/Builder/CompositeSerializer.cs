using System;
using System.Collections.Generic;
using System.Linq;

namespace Hydrogen;

public class CompositeSerializer<TItem> : ItemSerializerBase<TItem> {
	
	private Func<TItem> _activator;
	private MemberSerializationBinding[] _memberBindings;
	private bool _isConstantSize;
	private long _constantSize;

	public CompositeSerializer(Func<TItem> activator, MemberSerializationBinding[] memberBindings) {
		Configure(activator, memberBindings);
	}

	internal CompositeSerializer() {
		// This constructor is used by SerializerFactory in conjunction with Confgure method
	}

	internal void Configure(Func<TItem> activator, MemberSerializationBinding[] memberBindings)  {
		// This is used to configure the serializer after it has been created by the serializer builder.
		Guard.ArgumentNotNull(activator, nameof(activator));
		Guard.ArgumentNotNull(memberBindings, nameof(memberBindings));
		_activator = activator;
		_memberBindings = memberBindings;
		_isConstantSize = _memberBindings.All(x => x.Serializer.IsConstantSize);
		_constantSize =  IsConstantSize ? _memberBindings.Sum(x => x.Serializer.ConstantSize) : -1;
	}

	public override bool SupportsNull => false;

	public override bool IsConstantSize => _isConstantSize;

	public override long ConstantSize => _constantSize;

	public override long CalculateTotalSize(SerializationContext context, IEnumerable<TItem> items, bool calculateIndividualItems, out long[] itemSizes) {
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

	public override long CalculateSize(SerializationContext context, TItem item) {
		var size = 0L;
		foreach(var binding in _memberBindings) {
			var memberValue = binding.Member.GetValue(item);
			var memberSerializer = binding.Serializer;
			var itemSize = memberSerializer.CalculateSize(context, memberValue);
			size += itemSize;
		}
		return size;
	}
		
	public override void Serialize(TItem item, EndianBinaryWriter writer, SerializationContext context) {
		foreach (var binding in _memberBindings) {
			var memberValue = binding.Member.GetValue(item);
			binding.Serializer.Serialize(memberValue, writer, context);
		}
	}

	public override TItem Deserialize(EndianBinaryReader reader, SerializationContext context) {
		var item = _activator();
		context.SetDeserializingItem(item);
		foreach (var binding in _memberBindings) {
			var memberValue = binding.Serializer.Deserialize(reader, context);
			binding.Member.SetValue(item, memberValue);
		}
		return item;
	}

}
