
using System;
using System.Collections.Generic;
using System.Reflection;
using Hydrogen.Mapping;

namespace Hydrogen;

internal static class IndexFactory {
	
	#region UniqueKey Index

	internal static IClusteredStreamsAttachment CreateKeyIndexAttachment(ObjectStream objectStream, long streamIndex, Member member, IItemSerializer keySerializer = null, object keyComparer = null) {
		Guard.Ensure(objectStream.GetType() == typeof(ObjectStream<>).MakeGenericType(member.DeclaringType));
		
		var projection = member.AsDelegate();

		// Assuming the objectStream can be cast to the appropriate generic type based on Member's DeclaringType
		var genericContainerType = typeof(ObjectStream<>).MakeGenericType(member.DeclaringType);
		var genericContainer = Convert.ChangeType(objectStream, genericContainerType);


		// Use MakeGenericMethod to call the generic method with the specific types
		var method = typeof(IndexFactory)
			.GetMethod(nameof(CreateKeyIndex), BindingFlags.NonPublic | BindingFlags.Static)
			.MakeGenericMethod(member.DeclaringType, member.PropertyType);

		return (IClusteredStreamsAttachment)method.Invoke(null, new object[] { genericContainer, streamIndex, projection, keySerializer, keyComparer });

	}

	internal static KeyIndex<TItem, TKey> CreateKeyIndex<TItem, TKey>(ObjectStream<TItem> objectStream, long streamIndex, Func<TItem, TKey> projection, IItemSerializer<TKey> keySerializer = null, IEqualityComparer<TKey> keyComparer= null) {
		keySerializer ??= ItemSerializer<TKey>.Default;
		keyComparer ??= EqualityComparer<TKey>.Default;
		var keyChecksumKeyIndex = new KeyIndex<TItem, TKey>(objectStream, streamIndex, projection, keyComparer, keySerializer);
		return keyChecksumKeyIndex;
	}

	#endregion

	#region UniqueKey Index

	internal static IClusteredStreamsAttachment CreateUniqueKeyIndexAttachment(ObjectStream objectStream, long streamIndex, Member member, IItemSerializer keySerializer = null, object keyComparer = null) {
		Guard.Ensure(objectStream.GetType() == typeof(ObjectStream<>).MakeGenericType(member.DeclaringType));
		
		var projection = member.AsDelegate();

		// Assuming the objectStream can be cast to the appropriate generic type based on Member's DeclaringType
		var genericContainerType = typeof(ObjectStream<>).MakeGenericType(member.DeclaringType);
		var genericContainer = Convert.ChangeType(objectStream, genericContainerType);


		// Use MakeGenericMethod to call the generic method with the specific types
		var method = typeof(IndexFactory)
			.GetMethod(nameof(CreateUniqueKeyIndex), BindingFlags.NonPublic | BindingFlags.Static)
			.MakeGenericMethod(member.DeclaringType, member.PropertyType);

		return (IClusteredStreamsAttachment)method.Invoke(null, new object[] { genericContainer, streamIndex, projection, keySerializer, keyComparer });

	}

	internal static UniqueKeyIndex<TItem, TKey> CreateUniqueKeyIndex<TItem, TKey>(ObjectStream<TItem> objectStream, long streamIndex, Func<TItem, TKey> projection, IItemSerializer<TKey> keySerializer = null, IEqualityComparer<TKey> keyComparer= null) {
		keySerializer ??= ItemSerializer<TKey>.Default;
		keyComparer ??= EqualityComparer<TKey>.Default;
		var keyChecksumKeyIndex = new UniqueKeyIndex<TItem, TKey>(objectStream, streamIndex, projection, keyComparer, keySerializer);
		return keyChecksumKeyIndex;
	}

	#endregion

	#region Checksum Index

	internal static IClusteredStreamsAttachment CreateKeyChecksumIndexAttachment(ObjectStream objectStream, long streamIndex, Member member, IItemSerializer keySerializer = null, object keyChecksummer = null, object keyFetcher = null, object keyComparer = null) {
		Guard.Ensure(objectStream.GetType() == typeof(ObjectStream<>).MakeGenericType(member.DeclaringType));
		
		var projection = member.AsDelegate();

		// Assuming the objectStream can be cast to the appropriate generic type based on Member's DeclaringType
		var genericContainerType = typeof(ObjectStream<>).MakeGenericType(member.DeclaringType);
		var genericContainer = Convert.ChangeType(objectStream, genericContainerType);

		// Use MakeGenericMethod to call the generic method with the specific types
		var method = typeof(IndexFactory)
			.GetMethod(nameof(CreateChecksumKeyIndex), BindingFlags.NonPublic | BindingFlags.Static)
			.MakeGenericMethod(member.DeclaringType, member.PropertyType);

		return (IClusteredStreamsAttachment)method.Invoke(null, new object[] { genericContainer, streamIndex, projection, keySerializer, keyChecksummer, keyFetcher, keyComparer });

	}

	internal static KeyChecksumIndex<TItem, TKey> CreateChecksumKeyIndex<TItem, TKey>(ObjectStream<TItem> objectStream, long streamIndex, Func<TItem, TKey> projection, IItemSerializer<TKey> keySerializer = null, IItemChecksummer<TKey> keyChecksummer= null, Func<long, TKey> keyFetcher= null, IEqualityComparer<TKey> keyComparer= null) {
		keySerializer ??= ItemSerializer<TKey>.Default;
		keyChecksummer ??= new ItemDigestor<TKey>(keySerializer, objectStream.Streams.Endianness);
		keyFetcher ??= x => projection(objectStream.LoadItem(x));
		keyComparer ??= EqualityComparer<TKey>.Default;
		var keyChecksumKeyIndex = new KeyChecksumIndex<TItem, TKey>(
			objectStream,
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

	internal static IClusteredStreamsAttachment CreateUniqueKeyChecksumIndexAttachment(ObjectStream objectStream, long streamIndex, Member member, IItemSerializer keySerializer = null, object keyChecksummer = null, object keyFetcher = null, object keyComparer = null) {
		Guard.Ensure(objectStream.GetType() == typeof(ObjectStream<>).MakeGenericType(member.DeclaringType));
		
		var projection = member.AsDelegate();

		// Assuming the objectStream can be cast to the appropriate generic type based on Member's DeclaringType
		var genericContainerType = typeof(ObjectStream<>).MakeGenericType(member.DeclaringType);
		var genericContainer = Convert.ChangeType(objectStream, genericContainerType);

		// Use MakeGenericMethod to call the generic method with the specific types
		var method = typeof(IndexFactory)
			.GetMethod(nameof(CreateUniqueChecksumKeyIndex), BindingFlags.NonPublic | BindingFlags.Static)
			.MakeGenericMethod(member.DeclaringType, member.PropertyType);

		return (IClusteredStreamsAttachment)method.Invoke(null, new object[] { genericContainer, streamIndex, projection, keySerializer, keyChecksummer, keyFetcher, keyComparer });

	}

	internal static UniqueKeyChecksumIndex<TItem, TKey> CreateUniqueChecksumKeyIndex<TItem, TKey>(ObjectStream<TItem> objectStream, long streamIndex, Func<TItem, TKey> projection, IItemSerializer<TKey> keySerializer = null, IItemChecksummer<TKey> keyChecksummer= null, Func<long, TKey> keyFetcher= null, IEqualityComparer<TKey> keyComparer= null) {
		keySerializer ??= ItemSerializer<TKey>.Default;
		keyChecksummer ??= new ItemDigestor<TKey>(keySerializer, objectStream.Streams.Endianness);
		keyFetcher ??= x => projection(objectStream.LoadItem(x));
		keyComparer ??= EqualityComparer<TKey>.Default;
		var uniqueKeyChecksumIndex = new UniqueKeyChecksumIndex<TItem, TKey>(
			objectStream,
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

	internal static RecyclableIndexIndex CreateRecyclableIndexIndex(ObjectStream objectStream, long streamIndex) {
		var keyChecksumKeyIndex = new RecyclableIndexIndex(objectStream, streamIndex);
		return keyChecksumKeyIndex;
	}

	#endregion
}
