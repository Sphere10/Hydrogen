
using System;
using System.Collections.Generic;
using System.Reflection;
using Hydrogen.Mapping;

namespace Hydrogen;

internal static class IndexFactory {
	
	#region Member Index

	internal static IClusteredStreamsAttachment CreateMemberIndexAttachment(ObjectStream objectStream, long streamIndex, Member member, IItemSerializer keySerializer = null, object keyComparer = null) {
		Guard.Ensure(objectStream.GetType() == typeof(ObjectStream<>).MakeGenericType(member.DeclaringType));
		
		var projection = member.AsDelegate();

		// Assuming the objectStream can be cast to the appropriate generic type based on Member's DeclaringType
		var genericContainerType = typeof(ObjectStream<>).MakeGenericType(member.DeclaringType);
		var genericContainer = Convert.ChangeType(objectStream, genericContainerType);


		// Use MakeGenericMethod to call the generic method with the specific types
		var method = typeof(IndexFactory)
			.GetMethod(nameof(CreateMemberIndex), BindingFlags.NonPublic | BindingFlags.Static)
			.MakeGenericMethod(member.DeclaringType, member.PropertyType);

		return (IClusteredStreamsAttachment)method.Invoke(null, new object[] { genericContainer, streamIndex, projection, keySerializer, keyComparer });

	}

	internal static MemberIndex<TItem, TKey> CreateMemberIndex<TItem, TKey>(ObjectStream<TItem> objectStream, long streamIndex, Func<TItem, TKey> projection, IItemSerializer<TKey> keySerializer = null, IEqualityComparer<TKey> keyComparer= null) {
		keySerializer ??= ItemSerializer<TKey>.Default;
		keyComparer ??= EqualityComparer<TKey>.Default;
		var keyChecksumKeyIndex = new MemberIndex<TItem, TKey>(objectStream, streamIndex, projection, keyComparer, keySerializer);
		return keyChecksumKeyIndex;
	}

	#endregion

	#region Unique Memeber Index

	internal static IClusteredStreamsAttachment CreateUniqueMemberIndexAttachment(ObjectStream objectStream, long streamIndex, Member member, IItemSerializer keySerializer = null, object keyComparer = null) {
		Guard.Ensure(objectStream.GetType() == typeof(ObjectStream<>).MakeGenericType(member.DeclaringType));
		
		var projection = member.AsDelegate();

		// Assuming the objectStream can be cast to the appropriate generic type based on Member's DeclaringType
		var genericContainerType = typeof(ObjectStream<>).MakeGenericType(member.DeclaringType);
		var genericContainer = Convert.ChangeType(objectStream, genericContainerType);


		// Use MakeGenericMethod to call the generic method with the specific types
		var method = typeof(IndexFactory)
			.GetMethod(nameof(CreateUniqueMemberIndex), BindingFlags.NonPublic | BindingFlags.Static)
			.MakeGenericMethod(member.DeclaringType, member.PropertyType);

		return (IClusteredStreamsAttachment)method.Invoke(null, new object[] { genericContainer, streamIndex, projection, keySerializer, keyComparer });

	}

	internal static UniqueMemberIndex<TItem, TKey> CreateUniqueMemberIndex<TItem, TKey>(ObjectStream<TItem> objectStream, long streamIndex, Func<TItem, TKey> projection, IItemSerializer<TKey> keySerializer = null, IEqualityComparer<TKey> keyComparer= null) {
		keySerializer ??= ItemSerializer<TKey>.Default;
		keyComparer ??= EqualityComparer<TKey>.Default;
		var keyChecksumKeyIndex = new UniqueMemberIndex<TItem, TKey>(objectStream, streamIndex, projection, keyComparer, keySerializer);
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

	internal static MemberChecksumIndex<TItem, TKey> CreateChecksumKeyIndex<TItem, TKey>(ObjectStream<TItem> objectStream, long streamIndex, Func<TItem, TKey> projection, IItemSerializer<TKey> keySerializer = null, IItemChecksummer<TKey> keyChecksummer= null, Func<long, TKey> keyFetcher= null, IEqualityComparer<TKey> keyComparer= null) {
		keySerializer ??= ItemSerializer<TKey>.Default;
		keyChecksummer ??= new ItemDigestor<TKey>(keySerializer, objectStream.Streams.Endianness);
		keyFetcher ??= x => projection(objectStream.LoadItem(x));
		keyComparer ??= EqualityComparer<TKey>.Default;
		var keyChecksumKeyIndex = new MemberChecksumIndex<TItem, TKey>(
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

	internal static IClusteredStreamsAttachment CreateUniqueMemberChecksumIndexAttachment(ObjectStream objectStream, long streamIndex, Member member, IItemSerializer keySerializer = null, object keyChecksummer = null, object keyFetcher = null, object keyComparer = null) {
		Guard.Ensure(objectStream.GetType() == typeof(ObjectStream<>).MakeGenericType(member.DeclaringType));
		
		var projection = member.AsDelegate();

		// Assuming the objectStream can be cast to the appropriate generic type based on Member's DeclaringType
		var genericContainerType = typeof(ObjectStream<>).MakeGenericType(member.DeclaringType);
		var genericContainer = Convert.ChangeType(objectStream, genericContainerType);

		// Use MakeGenericMethod to call the generic method with the specific types
		var method = typeof(IndexFactory)
			.GetMethod(nameof(CreateUniqueMemberChecksumIndex), BindingFlags.NonPublic | BindingFlags.Static)
			.MakeGenericMethod(member.DeclaringType, member.PropertyType);

		return (IClusteredStreamsAttachment)method.Invoke(null, new object[] { genericContainer, streamIndex, projection, keySerializer, keyChecksummer, keyFetcher, keyComparer });

	}

	internal static UniqueMemberChecksumIndex<TItem, TKey> CreateUniqueMemberChecksumIndex<TItem, TKey>(ObjectStream<TItem> objectStream, long streamIndex, Func<TItem, TKey> projection, IItemSerializer<TKey> keySerializer = null, IItemChecksummer<TKey> keyChecksummer= null, Func<long, TKey> keyFetcher= null, IEqualityComparer<TKey> keyComparer= null) {
		keySerializer ??= ItemSerializer<TKey>.Default;
		keyChecksummer ??= new ItemDigestor<TKey>(keySerializer, objectStream.Streams.Endianness);
		keyFetcher ??= x => projection(objectStream.LoadItem(x));
		keyComparer ??= EqualityComparer<TKey>.Default;
		var uniqueKeyChecksumIndex = new UniqueMemberChecksumIndex<TItem, TKey>(
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
