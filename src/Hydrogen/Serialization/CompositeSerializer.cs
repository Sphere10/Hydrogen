using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Hydrogen;

public class CompositeSerializer<TItem> : ItemSerializer<TItem> {
	private Func<TItem> _activator;
	private MemberSerializationBinding[] _memberBindings;
	private bool _isConstantSize;
	private long _constantSize;

	internal CompositeSerializer() : base(SizeDescriptorStrategy.UseCVarInt) {
		// NOTE: this constructor is needed by SerializationFacotry for assembling recursive serializers
		// that involve member's that use this very same serializer. The serializer is constructed and registered
		// uninitialized, and then initialized later via ConfigurePacked method.
	}

	public CompositeSerializer(Func<TItem> activator, MemberSerializationBinding[] memberBindings) 
		: this() {
		Configure(activator, memberBindings);
	}

	internal void ConfigureInternal(Func<object> activator, MemberSerializationBinding[] memberBindings) 
		=> Configure(() => (TItem)activator(), memberBindings);

	internal void Configure(Func<TItem> activator, MemberSerializationBinding[] memberBindings) {
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
		using var scope = EnterCompositeScope(item, SerializationTask.Sizing , out _);
		foreach(var binding in _memberBindings) {
			var memberValue = binding.Member.GetValue(item);
			var memberSerializer = binding.Serializer;
			var itemSize = memberSerializer.CalculateSize(memberValue);
			size += itemSize;
		}
		return size;
	}
		
	public override void Serialize(TItem item, EndianBinaryWriter writer) {
		using var scope = EnterCompositeScope(item, SerializationTask.Serializing, out _);
		System.Console.WriteLine($"Serializing item {item.GetType().ToStringCS()}");
		foreach (var binding in _memberBindings) {
			System.Console.WriteLine($"Serializing member {binding.Member.PropertyType.ToStringCS()} {binding.Member.Name}");
			var memberValue = binding.Member.GetValue(item);
			binding.Serializer.Serialize(memberValue, writer);
		}
	}

	public override TItem Deserialize(EndianBinaryReader reader) {
		var item = _activator();
		using var scope = EnterCompositeScope(item, SerializationTask.Deserializing, out var index);
		System.Console.WriteLine($"Deserializing item {index}");
		
		foreach (var binding in _memberBindings) {
			System.Console.WriteLine($"Deserializing member {binding.Member.PropertyType.ToStringCS()} {binding.Member.Name}");
			var memberValue = binding.Serializer.Deserialize(reader);
			if (memberValue is SerializationScope.PlaceHolder placeHolder) {
				// Member value was a cyclic reference placeholder, so set it at the end
				// This means the value it is referencing hasn't finished deserializing yet. 
				scope.RegisterFinalizationAction(() => binding.Member.SetValue(item, placeHolder.GetValue()));
			} else {
				// Member value was deserialized, so set it on the item
				binding.Member.SetValue(item, memberValue);
			}
			
		}
		if (index >= 0) {
			scope.NotifyDeserializedObject(item, index);
			System.Console.WriteLine($"Deserialized item {index} - {item.GetType().ToStringCS()}");
		}
		return item;
	}
	

	private SerializationScope EnterCompositeScope(TItem item, SerializationTask serializationTask, out long index) {
		// Due to how the serializer factory works, if this is the root-serializer, we need to initiate
		// a SerializationScope so that cyclic references to root object can be detected. If it's not
		// a root-level serializer, then the CyclicReferenceAwareSerializer wrapper will take care of everything.
		index = -1;
		var scope = new SerializationScope();
		if (scope.IsRootScope) {
			switch(serializationTask) {
				case SerializationTask.Sizing:
					scope.NotifySerializingObject(item, true);
					break;
				case SerializationTask.Serializing:
					scope.NotifySerializingObject(item, false);
					break;
				case SerializationTask.Deserializing:
					scope.NotifyDeserializingObject(out index);
					break;
			}
		}
		return scope;

	}

	private enum SerializationTask  {
		Sizing,
		Serializing,
		Deserializing
	}
}
