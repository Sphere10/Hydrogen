
using System;
using System.Collections.Generic;
using System.Reflection;
using Hydrogen.Mapping;

namespace Hydrogen.ObjectSpaces;

internal static class IndexFactory {
	
	#region UniqueKey Index

	internal static IStreamContainerAttachment CreateKeyIndexAttachment(ObjectContainer container, long streamIndex, Member member, IItemSerializer keySerializer = null, object keyComparer = null) {
		Guard.Ensure(container.GetType() == typeof(ObjectContainer<>).MakeGenericType(member.DeclaringType));
		
		var projection = member.AsDelegate();

		// Assuming the container can be cast to the appropriate generic type based on Member's DeclaringType
		var genericContainerType = typeof(ObjectContainer<>).MakeGenericType(member.DeclaringType);
		var genericContainer = Convert.ChangeType(container, genericContainerType);


		// Use MakeGenericMethod to call the generic method with the specific types
		var method = typeof(IndexFactory)
			.GetMethod(nameof(CreateKeyIndex), BindingFlags.NonPublic | BindingFlags.Static)
			.MakeGenericMethod(member.DeclaringType, member.PropertyType);

		return (IStreamContainerAttachment)method.Invoke(null, new object[] { genericContainer, streamIndex, projection, keySerializer, keyComparer });

	}

	internal static KeyIndex<TItem, TKey> CreateKeyIndex<TItem, TKey>(ObjectContainer<TItem> container, long streamIndex, Func<TItem, TKey> projection, IItemSerializer<TKey> keySerializer = null, IEqualityComparer<TKey> keyComparer= null) {
		keySerializer ??= ItemSerializer<TKey>.Default;
		keyComparer ??= EqualityComparer<TKey>.Default;
		var keyChecksumKeyIndex = new KeyIndex<TItem, TKey>(container, streamIndex, projection, keyComparer, keySerializer);
		return keyChecksumKeyIndex;
	}

	#endregion

	#region UniqueKey Index

	internal static IStreamContainerAttachment CreateUniqueKeyIndexAttachment(ObjectContainer container, long streamIndex, Member member, IItemSerializer keySerializer = null, object keyComparer = null) {
		Guard.Ensure(container.GetType() == typeof(ObjectContainer<>).MakeGenericType(member.DeclaringType));
		
		var projection = member.AsDelegate();

		// Assuming the container can be cast to the appropriate generic type based on Member's DeclaringType
		var genericContainerType = typeof(ObjectContainer<>).MakeGenericType(member.DeclaringType);
		var genericContainer = Convert.ChangeType(container, genericContainerType);


		// Use MakeGenericMethod to call the generic method with the specific types
		var method = typeof(IndexFactory)
			.GetMethod(nameof(CreateUniqueKeyIndex), BindingFlags.NonPublic | BindingFlags.Static)
			.MakeGenericMethod(member.DeclaringType, member.PropertyType);

		return (IStreamContainerAttachment)method.Invoke(null, new object[] { genericContainer, streamIndex, projection, keySerializer, keyComparer });

	}

	internal static UniqueKeyIndex<TItem, TKey> CreateUniqueKeyIndex<TItem, TKey>(ObjectContainer<TItem> container, long streamIndex, Func<TItem, TKey> projection, IItemSerializer<TKey> keySerializer = null, IEqualityComparer<TKey> keyComparer= null) {
		keySerializer ??= ItemSerializer<TKey>.Default;
		keyComparer ??= EqualityComparer<TKey>.Default;
		var keyChecksumKeyIndex = new UniqueKeyIndex<TItem, TKey>(container, streamIndex, projection, keyComparer, keySerializer);
		return keyChecksumKeyIndex;
	}

	#endregion

	#region Checksum Index

	internal static IStreamContainerAttachment CreateKeyChecksumIndexAttachment(ObjectContainer container, long streamIndex, Member member, IItemSerializer keySerializer = null, object keyChecksummer = null, object keyFetcher = null, object keyComparer = null) {
		Guard.Ensure(container.GetType() == typeof(ObjectContainer<>).MakeGenericType(member.DeclaringType));
		
		var projection = member.AsDelegate();

		// Assuming the container can be cast to the appropriate generic type based on Member's DeclaringType
		var genericContainerType = typeof(ObjectContainer<>).MakeGenericType(member.DeclaringType);
		var genericContainer = Convert.ChangeType(container, genericContainerType);

		// Use MakeGenericMethod to call the generic method with the specific types
		var method = typeof(IndexFactory)
			.GetMethod(nameof(CreateChecksumKeyIndex), BindingFlags.NonPublic | BindingFlags.Static)
			.MakeGenericMethod(member.DeclaringType, member.PropertyType);

		return (IStreamContainerAttachment)method.Invoke(null, new object[] { genericContainer, streamIndex, projection, keySerializer, keyChecksummer, keyFetcher, keyComparer });

	}

	internal static KeyChecksumIndex<TItem, TKey> CreateChecksumKeyIndex<TItem, TKey>(ObjectContainer<TItem> container, long streamIndex, Func<TItem, TKey> projection, IItemSerializer<TKey> keySerializer = null, IItemChecksummer<TKey> keyChecksummer= null, Func<long, TKey> keyFetcher= null, IEqualityComparer<TKey> keyComparer= null) {
		keySerializer ??= ItemSerializer<TKey>.Default;
		keyChecksummer ??= new ItemDigestor<TKey>(keySerializer, container.Streams.Endianness);
		keyFetcher ??= x => projection(container.LoadItem(x));
		keyComparer ??= EqualityComparer<TKey>.Default;
		var keyChecksumKeyIndex = new KeyChecksumIndex<TItem, TKey>(
			container,
			streamIndex,
			projection,
			keyChecksummer,
			keyFetcher,
			keyComparer
		);

		return keyChecksumKeyIndex;
	}

	#endregion

	#region Checksum Unique Key

	internal static IStreamContainerAttachment CreateUniqueKeyChecksumIndexAttachment(ObjectContainer container, long streamIndex, Member member, IItemSerializer keySerializer = null, object keyChecksummer = null, object keyFetcher = null, object keyComparer = null) {
		Guard.Ensure(container.GetType() == typeof(ObjectContainer<>).MakeGenericType(member.DeclaringType));
		
		var projection = member.AsDelegate();

		// Assuming the container can be cast to the appropriate generic type based on Member's DeclaringType
		var genericContainerType = typeof(ObjectContainer<>).MakeGenericType(member.DeclaringType);
		var genericContainer = Convert.ChangeType(container, genericContainerType);

		// Use MakeGenericMethod to call the generic method with the specific types
		var method = typeof(IndexFactory)
			.GetMethod(nameof(CreateUniqueChecksumKeyIndex), BindingFlags.NonPublic | BindingFlags.Static)
			.MakeGenericMethod(member.DeclaringType, member.PropertyType);

		return (IStreamContainerAttachment)method.Invoke(null, new object[] { genericContainer, streamIndex, projection, keySerializer, keyChecksummer, keyFetcher, keyComparer });

	}

	internal static UniqueKeyChecksumIndex<TItem, TKey> CreateUniqueChecksumKeyIndex<TItem, TKey>(ObjectContainer<TItem> container, long streamIndex, Func<TItem, TKey> projection, IItemSerializer<TKey> keySerializer = null, IItemChecksummer<TKey> keyChecksummer= null, Func<long, TKey> keyFetcher= null, IEqualityComparer<TKey> keyComparer= null) {
		keySerializer ??= ItemSerializer<TKey>.Default;
		keyChecksummer ??= new ItemDigestor<TKey>(keySerializer, container.Streams.Endianness);
		keyFetcher ??= x => projection(container.LoadItem(x));
		keyComparer ??= EqualityComparer<TKey>.Default;
		var uniqueKeyChecksumIndex = new UniqueKeyChecksumIndex<TItem, TKey>(
			container,
			streamIndex,
			projection,
			keyChecksummer,
			keyFetcher,
			keyComparer
		);

		return uniqueKeyChecksumIndex;
	}

	#endregion

	#region RecyclableIndex Index

	internal static RecyclableIndexIndex CreateRecyclableIndexIndex(ObjectContainer container, long streamIndex) {
		var keyChecksumKeyIndex = new RecyclableIndexIndex(container, streamIndex);
		return keyChecksumKeyIndex;
	}

	#endregion
}
