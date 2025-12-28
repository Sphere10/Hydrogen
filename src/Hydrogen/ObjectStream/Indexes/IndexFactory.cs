// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Reflection;
using Hydrogen.Mapping;

namespace Hydrogen;

/// <summary>
/// Utility helpers for constructing projection and checksum indexes over <see cref="ObjectStream"/> instances.
/// </summary>
internal static class IndexFactory {
	
	#region Prjection Index

	internal static IClusteredStreamsAttachment CreateMemberIndex(
		ObjectStream objectStream, 
		string indexName,
		Member member, 
		IItemSerializer keySerializer = null,
		object keyComparer = null) {
		Guard.Ensure(objectStream.GetType() == typeof(ObjectStream<>).MakeGenericType(member.DeclaringType));
		
		var projection = member.AsDelegate();

		// Assuming the objectStream can be cast to the appropriate generic type based on Member's DeclaringType
		var genericContainerType = typeof(ObjectStream<>).MakeGenericType(member.DeclaringType);
		var genericContainer = Convert.ChangeType(objectStream, genericContainerType);


		// Use MakeGenericMethod to call the generic method with the specific types
		var method = typeof(IndexFactory)
			.GetMethod(nameof(CreateProjectionIndex), BindingFlags.NonPublic | BindingFlags.Static)
			.MakeGenericMethod(member.DeclaringType, member.PropertyType);

		return (IClusteredStreamsAttachment)method.Invoke(null, new object[] { genericContainer, indexName, projection, keySerializer, keyComparer });

	}

	internal static ProjectionIndex<TItem, TKey> CreateProjectionIndex<TItem, TKey>(
		ObjectStream<TItem> objectStream,
		string indexName,
		Func<TItem, TKey> projection,
		IItemSerializer<TKey> keySerializer = null,
		IEqualityComparer<TKey> keyComparer = null) {
		keySerializer ??= ItemSerializer<TKey>.Default;
		var keyChecksumKeyIndex = new ProjectionIndex<TItem, TKey>(objectStream, indexName, projection, keySerializer, keyComparer);
		return keyChecksumKeyIndex;
	}

	#endregion

	#region Unique Projection Index

	internal static IClusteredStreamsAttachment CreateUniqueMemberIndex(ObjectStream objectStream, string indexName, Member member, IItemSerializer keySerializer = null, object keyComparer = null) {
		Guard.Ensure(objectStream.GetType() == typeof(ObjectStream<>).MakeGenericType(member.DeclaringType));
		
		var projection = member.AsDelegate();

		// Assuming the objectStream can be cast to the appropriate generic type based on Member's DeclaringType
		var genericContainerType = typeof(ObjectStream<>).MakeGenericType(member.DeclaringType);
		var genericContainer = Convert.ChangeType(objectStream, genericContainerType);


		// Use MakeGenericMethod to call the generic method with the specific types
		var method = typeof(IndexFactory)
			.GetMethod(nameof(CreateUniqueProjectionIndex), BindingFlags.NonPublic | BindingFlags.Static)
			.MakeGenericMethod(member.DeclaringType, member.PropertyType);

		return (IClusteredStreamsAttachment)method.Invoke(null, new object[] { genericContainer, indexName, projection, keySerializer, keyComparer });

	}

	internal static UniqueProjectionIndex<TItem, TKey> CreateUniqueProjectionIndex<TItem, TKey>(ObjectStream<TItem> objectStream, string indexName, Func<TItem, TKey> projection, IItemSerializer<TKey> keySerializer = null, IEqualityComparer<TKey> keyComparer = null) {
		keySerializer ??= ItemSerializer<TKey>.Default;
		keyComparer ??= EqualityComparer<TKey>.Default; // TODO: should this use a ComparerFactory?
		var keyChecksumKeyIndex = new UniqueProjectionIndex<TItem, TKey>(objectStream, indexName, projection, keySerializer, keyComparer);
		return keyChecksumKeyIndex;
	}

	#endregion

	#region Projection Checksum Index

	internal static IClusteredStreamsAttachment CreateMemberChecksumIndex(
		ObjectStream objectStream, 
		string indexName, 
		Member member, 
		IItemSerializer keySerializer = null, 
		object keyChecksummer = null, 
		object keyFetcher = null, 
		object keyComparer = null,
		IndexNullPolicy indexNullPolicy = IndexNullPolicy.IgnoreNull
	) {
		Guard.Ensure(objectStream.GetType() == typeof(ObjectStream<>).MakeGenericType(member.DeclaringType));
		
		var projection = member.AsDelegate();

		// Assuming the objectStream can be cast to the appropriate generic type based on Member's DeclaringType
		var genericContainerType = typeof(ObjectStream<>).MakeGenericType(member.DeclaringType);
		var genericContainer = Convert.ChangeType(objectStream, genericContainerType);

		// Use MakeGenericMethod to call the generic method with the specific types
		var method = typeof(IndexFactory)
			.GetMethod(nameof(CreateProjectionChecksumIndex), BindingFlags.NonPublic | BindingFlags.Static)
			.MakeGenericMethod(member.DeclaringType, member.PropertyType);

		return (IClusteredStreamsAttachment)method.Invoke(null, new object[] { genericContainer, indexName, projection, keySerializer, keyChecksummer, keyFetcher, keyComparer, indexNullPolicy });

	}

	internal static ProjectionChecksumIndex<TItem, TKey> CreateProjectionChecksumIndex<TItem, TKey>(
		ObjectStream<TItem> objectStream,
		string indexName, 
		Func<TItem, TKey> projection, 
		IItemSerializer<TKey> keySerializer = null, 
		IItemChecksummer<TKey> keyChecksummer = null, 
		Func<long, TKey> keyFetcher = null, 
		IEqualityComparer<TKey> keyComparer= null,
		IndexNullPolicy indexNullPolicy = IndexNullPolicy.IgnoreNull
	) {

		keySerializer ??= ItemSerializer<TKey>.Default;
		keyChecksummer ??= new ItemDigestor<TKey>(keySerializer, objectStream.Streams.Endianness);
		keyFetcher ??= x => projection(objectStream.LoadItem(x));
		keyComparer ??= EqualityComparer<TKey>.Default;
		var keyChecksumKeyIndex = new ProjectionChecksumIndex<TItem, TKey>(
			objectStream,
			indexName,
			projection,
			keyChecksummer,
			keyFetcher,
			keyComparer,
			indexNullPolicy
		);

		return keyChecksumKeyIndex;
	}

	#endregion

	#region Unique Projection Checksum Index

	internal static IClusteredStreamsAttachment CreateUniqueMemberChecksumIndex(
		ObjectStream objectStream, 
		string indexName, 
		Member member, 
		IItemSerializer keySerializer = null,
		object keyChecksummer = null, 
		object keyFetcher = null, 
		object keyComparer = null,
		IndexNullPolicy indexNullPolicy = IndexNullPolicy.IgnoreNull
	) {
		Guard.Ensure(objectStream.GetType() == typeof(ObjectStream<>).MakeGenericType(member.DeclaringType));
		
		var projection = member.AsDelegate();

		// Assuming the objectStream can be cast to the appropriate generic type based on Member's DeclaringType
		var genericContainerType = typeof(ObjectStream<>).MakeGenericType(member.DeclaringType);
		var genericContainer = Convert.ChangeType(objectStream, genericContainerType);

		// Use MakeGenericMethod to call the generic method with the specific types
		var method = typeof(IndexFactory)
			.GetMethod(nameof(CreateUniqueProjectionChecksumIndex), BindingFlags.NonPublic | BindingFlags.Static)
			.MakeGenericMethod(member.DeclaringType, member.PropertyType);

		return (IClusteredStreamsAttachment)method.Invoke(null, new object[] { genericContainer, indexName, projection, keySerializer, keyChecksummer, keyFetcher, keyComparer, indexNullPolicy });

	}

	internal static UniqueProjectionChecksumIndex<TItem, TKey> CreateUniqueProjectionChecksumIndex<TItem, TKey>(
		ObjectStream<TItem> objectStream,
		string indexName, 
		Func<TItem, TKey> projection, 
		IItemSerializer<TKey> keySerializer = null, 
		IItemChecksummer<TKey> keyChecksummer = null, 
		Func<long, TKey> keyFetcher = null, 
		IEqualityComparer<TKey> keyComparer = null,
		IndexNullPolicy indexNullPolicy = IndexNullPolicy.IgnoreNull
	) {
		keySerializer ??= ItemSerializer<TKey>.Default;
		keyChecksummer ??= new ItemDigestor<TKey>(keySerializer, objectStream.Streams.Endianness);
		keyFetcher ??= x => projection(objectStream.LoadItem(x));
		keyComparer ??= EqualityComparer<TKey>.Default;
		var uniqueKeyChecksumIndex = new UniqueProjectionChecksumIndex<TItem, TKey>(
			objectStream,
			indexName,
			projection,
			keyChecksummer,
			keyFetcher,
			keyComparer,
			indexNullPolicy
		);

		return uniqueKeyChecksumIndex;
	}

	#endregion

	#region RecyclableIndex Index

	internal static RecyclableIndexIndex CreateRecyclableIndexIndex(ObjectStream objectStream, string indexName) {
		var keyChecksumKeyIndex = new RecyclableIndexIndex(objectStream, indexName);
		return keyChecksumKeyIndex;
	}

	#endregion
}
