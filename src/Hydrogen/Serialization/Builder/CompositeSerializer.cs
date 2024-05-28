using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Hydrogen;

public class CompositeSerializer<TItem> : ItemSerializerBase<TItem> {
	private Func<TItem> _activator;
	private MemberSerializationBinding[] _memberBindings;
	private bool _isConstantSize;
	private long _constantSize;
	private bool _configured;

	public CompositeSerializer(Func<TItem> activator, MemberSerializationBinding[] memberBindings) : this() {
		Configure(activator, memberBindings);
	}

	internal CompositeSerializer() {
		// This constructor is used by SerializerFactory in conjunction with Configure method
		_configured = false;
	}

	internal void Configure(Delegate activator, MemberSerializationBinding[] memberBindings)  {
		// This is used to configure the serializer after it has been created by the serializer builder.
		Guard.ArgumentNotNull(activator, nameof(activator));
		Guard.ArgumentNotNull(memberBindings, nameof(memberBindings));
		Guard.Ensure(!_configured, "Serializer has already been configured.");
		_activator = (Func<TItem>)activator;
		_memberBindings = memberBindings;
		_isConstantSize = _memberBindings.All(x => x.Serializer.IsConstantSize);
		_constantSize =  IsConstantSize ? _memberBindings.Sum(x => x.Serializer.ConstantSize) : -1;
		_configured = true;
	}

	public override bool SupportsNull => false;

	public override bool IsConstantSize => _isConstantSize;

	public override long ConstantSize => _constantSize;

	public MemberSerializationBinding[] MemberBindings => _memberBindings;

	public override long CalculateTotalSize(SerializationContext context, IEnumerable<TItem> items, bool calculateIndividualItems, out long[] itemSizes) {
		var itemsArr = items as TItem[] ?? items.ToArray();
		itemSizes = null;
		var totalSize = 0L;
		foreach(var (member, serializer) in _memberBindings) {
			var itemMemberValues = itemsArr.Select(item => member.GetValue(item)).ToArray();
			var itemMemberSizes = itemMemberValues.Select(serializer.PackedCalculateSize).ToArray();
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
			var itemSize = memberSerializer.PackedCalculateSize(context, memberValue);
			size += itemSize;
		}
		return size;
	}
		
	public override void Serialize(TItem item, EndianBinaryWriter writer, SerializationContext context) {
		foreach (var binding in _memberBindings) {
			var memberValue = binding.Member.GetValue(item);
			binding.Serializer.PackedSerialize(memberValue, writer, context);
		}
	}

	public override TItem Deserialize(EndianBinaryReader reader, SerializationContext context) {
		var item = _activator();
		context.SetDeserializingItem(item);
		foreach (var binding in _memberBindings) {
			var memberValue = binding.Serializer.PackedDeserialize(reader, context);
			binding.Member.SetValue(item, memberValue);
		}
		return item;
	}

}

public static class CompositeSerializer {
	public static CompositeSerializer<TItem> Create<TItem>(Func<TItem> activator, MemberSerializationBinding[] memberBindings) 
		=> new CompositeSerializer<TItem>(activator, memberBindings);
	
	public static IItemSerializer Create(Type itemType, Delegate activator, MemberSerializationBinding[] memberBindings) {
		var serializer = Create(itemType);
		Configure(serializer, activator, memberBindings);
		return serializer;
	}

	public static IItemSerializer Create(Type itemType) 
		=> (IItemSerializer)typeof(CompositeSerializer<>)
			.MakeGenericType(itemType)
			.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null)
			.Invoke(null);

	public static void Configure(IItemSerializer serializer, Type itemType, IEnumerable<MemberSerializationBinding> memberBindings) {
		var constructor = itemType.FindCompatibleConstructor(Array.Empty<Type>());
		Guard.Ensure(constructor is not null, $"Unable to compose a serializer for type '{itemType.ToStringCS()}' as it did not have a public parameterless constructor");
		Configure(serializer, Tools.Lambda.CastFunc(() => constructor.Invoke(null), itemType), memberBindings);
	}

	public static void Configure(IItemSerializer serializer, Delegate activator, IEnumerable<MemberSerializationBinding> memberBindings) {
		Guard.Ensure(serializer.GetType().IsConstructedGenericTypeOf(typeof(CompositeSerializer<>)), $"Serializer must be a {typeof(CompositeSerializer<>).ToStringCS()}");
		serializer
			.GetType()
			.GetMethod(nameof(CompositeSerializer<object>.Configure), BindingFlags.Instance | BindingFlags.NonPublic)
			.Invoke(serializer, [ activator, memberBindings.ToArray() ]);
	}


}
