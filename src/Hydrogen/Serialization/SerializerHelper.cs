using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Hydrogen.Mapping;

namespace Hydrogen;

internal static class SerializerHelper {
	public static Member[] GetSerializableMembers(Type type)
		=> type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
			.Where(x => x.CanRead && x.CanWrite)
			.Where(x => !x.HasAttribute<TransientAttribute>(false))
			.Select(x => x.ToMember())
			.ToArray();


	public static IItemSerializer AssembleSerializer(SerializerFactory serializerFactory, Type itemType, bool retainRegisteredTypesInFactory, long typeCodeStart) {

		// During the construction, a factory is required to store generated serializers.
		var factoryToUse = retainRegisteredTypesInFactory ? serializerFactory : new SerializerFactory(serializerFactory) { MinimumGeneratedTypeCode = typeCodeStart };

		var assembledSerializer = AssembleRecursively(factoryToUse, itemType);

		return assembledSerializer;

		IItemSerializer AssembleRecursively(SerializerFactory factory, Type itemType) {

			// Ensure serializers for component types are registered
			// (i.e. resolving serializer for List<UnregisteredType> serializer requires a serializer for UnregisteredType)
			foreach (var genericType in GetUnregisteredComponentTypes(factory, itemType))
				AssembleRecursively(factory, genericType);

			// If serializer already exists for this type in factory, use that
			if (factory.HasSerializer(itemType)) {
				var typeSerializer = factory.GetCachedSerializer(itemType);
				if (!itemType.IsValueType && !typeSerializer.SupportsNull) {
					// We only use a ReferenceSerializer if the given serializer does not support null values
					return (IItemSerializer)typeof(ReferenceSerializer<>)
						.MakeGenericType(itemType)
						.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(IItemSerializer<>)
						.MakeGenericType(itemType) }, null).Invoke(new object[] { typeSerializer });
				}
				return typeSerializer;
			}

			// Special Case: if we're serializing an enum (or nullable enum), we register it with the factory now and return
			if (itemType.IsEnum || itemType.IsConstructedGenericTypeOf(typeof(Nullable<>)) && itemType.GenericTypeArguments[0].IsEnum) {
				factory.RegisterEnum(itemType.IsEnum ? itemType : itemType.GenericTypeArguments[0]);
				return factory.GetCachedSerializer(itemType);
			}

			// No serializer registered so we need to assemble one as a CompositeSerializer. First, we need to 
			// register the serializer (before it is assembled) so that it may recursively refer to itself. So we 
			// activate a CompositeSerializer with no members (we'll configure it later)
			var compositeSerializer =
				(IItemSerializer)typeof(CompositeSerializer<>)
				.MakeGenericType(itemType)
				.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null)
				.Invoke(null);

			var serializer =
				itemType.IsValueType ?
				compositeSerializer :
				(IItemSerializer)typeof(ReferenceSerializer<>).MakeGenericType(itemType).GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(IItemSerializer<>).MakeGenericType(itemType) }, null).Invoke(new object[] { compositeSerializer });


			// register serializer instance now as it may be re-used in component serializers (recursive types)
			if (itemType != typeof(object))
				factory.RegisterInternal(factory.GenerateTypeCode(), itemType, compositeSerializer.GetType(), compositeSerializer, null);

			// Create the member serializers
			var members = SerializerHelper.GetSerializableMembers(itemType);
			var memberBindings = new List<MemberSerializationBinding>(members.Length);
			foreach (var member in members) {
				var propertyType = member.PropertyType;

				// Ensure we have a serializer for the member type
				if (propertyType != typeof(object) && !factory.HasSerializer(propertyType))
					AssembleRecursively(factory, propertyType);

				// We don't use the member type serializer but instead use a FactorySerializer to ensure cyclic/polymorphic references are handled correctly
				var memberSerializer = (IItemSerializer)typeof(FactorySerializer<>).MakeGenericType(propertyType).ActivateWithCompatibleArgs(factory);
				memberBindings.Add(new(member, memberSerializer.AsReferenceSerializer()));
			}

			// AddDimension the composite serializer instance (which is already registered)
			var capturedItemType = itemType;
			compositeSerializer
				.GetType()
				.GetMethod(nameof(CompositeSerializer<object>.Configure), BindingFlags.Instance | BindingFlags.NonPublic)
				.Invoke(compositeSerializer, [Tools.Lambda.CastFunc( () => capturedItemType.ActivateWithCompatibleArgs(), capturedItemType), memberBindings.ToArray()]);

			return serializer;
		}

		IEnumerable<Type> GetUnregisteredComponentTypes(SerializerFactory factory, Type type, HashSet<Type> alreadyVisited = null) {
			alreadyVisited ??= new HashSet<Type>();

			// List<Type>
			// Type[]
			// Type1<Type2, Type3>

			// Avoid recursive loops
			if (alreadyVisited.Contains(type))
				yield break;
			alreadyVisited.Add(type);

			// Case 1: There is an explicit serializer for this type, no component types need to be assembled
			if (factory.HasSerializer(type))
				yield break;


			// Case 2: Array element type may need assembling
			if (type.IsArray) {
				var elementType = type.GetElementType();
				if (!factory.HasSerializer(elementType)) {
					foreach (var elementTypeUnregisteredComponentTypes in GetUnregisteredComponentTypes(factory, elementType, alreadyVisited))
						yield return elementTypeUnregisteredComponentTypes;
					yield return elementType;
				}
			}

			// Case 4: Serializer for generic type definition exists but not for generic type arguments 
			// e.g. List<UnregType>, Dictionary<UnregType1, UnregType2>, etc
			if (type.IsConstructedGenericType && factory.HasSerializer(type.GetGenericTypeDefinition())) {
				foreach (var genericArgumentType in type.GetGenericArguments().Where(x => !factory.HasSerializer(x))) {
					foreach (var subType in GetUnregisteredComponentTypes(factory, genericArgumentType, alreadyVisited))
						yield return subType;
					yield return genericArgumentType;
				}
			}
		}
	}
}

