// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;

namespace Hydrogen.Tests;

[TestFixture]
[Parallelizable]
public class TypeSerializerTests {

	[Test]
	public void AssemblySerializer() {
		var serializer = new AssemblySerializer();
		var assembly = Assembly.GetAssembly(typeof(Account));
		var serialized = serializer.SerializeBytesLE(assembly);
		var deserialized = serializer.DeserializeBytesLE(serialized);
		Assert.That(deserialized, Is.EqualTo(assembly));
	}

	[Test]
	public void TypeSerializer_InAssemblyType() {
		var serializer = new TypeSerializer();
		var type = typeof(Account);
		var serialized = serializer.SerializeBytesLE(type);
		var deserialized = serializer.DeserializeBytesLE(serialized);
		Assert.That(deserialized, Is.EqualTo(type));
	}

	[Test]
	public void TypeSerializer_OutOfAssemblyType() {
		var serializer = new TypeSerializer();
		var type = typeof(Hydrogen.Data.HydrogenJsonSerializer);
		var serialized = serializer.SerializeBytesLE(type);
		var deserialized = serializer.DeserializeBytesLE(serialized);
		Assert.That(deserialized, Is.EqualTo(type));
	}

	[Test]
	public void TypeSerializer_PureGenericType() {
		var serializer = new TypeSerializer();
		var type = typeof(Hydrogen.IBijectiveDictionary<,>);
		var serialized = serializer.SerializeBytesLE(type);
		var deserialized = serializer.DeserializeBytesLE(serialized);
		Assert.That(deserialized, Is.EqualTo(type));
	}

	[Test]
	public void TypeSerializer_CrossAssemblyConstructedGenericType() {
		var serializer = new TypeSerializer();
		var type = typeof(IDictionary<IList<KeyValuePair<Account, int>>, IList<Hydrogen.Data.HydrogenJsonSerializer>>);
		var serialized = serializer.SerializeBytesLE(type);
		var deserialized = serializer.DeserializeBytesLE(serialized);
		Assert.That(deserialized, Is.EqualTo(type));
	}


	[Test]
	public void AssemblyTypeSerializer_InAssemblyType() {
		var type = typeof(Account);
		var assembly = Assembly.GetAssembly(type);
		var serializer = new AssemblyTypeSerializer(assembly);
		var serialized = serializer.SerializeBytesLE(type);
		var deserialized = serializer.DeserializeBytesLE(serialized);
		Assert.That(deserialized, Is.EqualTo(type));
	}

	[Test]
	public void AssemblyTypeSerializer_OutOfAssemblyType() {
		var type = typeof(Hydrogen.Data.HydrogenJsonSerializer);
		var assembly = Assembly.GetAssembly(type);
		var serializer = new AssemblyTypeSerializer(assembly);
		var serialized = serializer.SerializeBytesLE(type);
		var deserialized = serializer.DeserializeBytesLE(serialized);
		Assert.That(deserialized, Is.EqualTo(type));
	}


	[Test]
	public void AssemblyTypeSerializer_FailsOnWrongAssembly_Calc() {
		var serializer = new AssemblyTypeSerializer(Assembly.GetAssembly(typeof(Hydrogen.Data.HydrogenJsonSerializer)));
		Assert.That(() => serializer.CalculateSize(typeof(Account)), Throws.InstanceOf<ArgumentException>());
	}

	[Test]
	public void AssemblyTypeSerializer_FailsOnWrongAssembly_Serialize() {
		var serializer = new AssemblyTypeSerializer(Assembly.GetAssembly(typeof(Hydrogen.Data.HydrogenJsonSerializer)));
		Assert.That(() => serializer.SerializeBytesLE(typeof(Account)), Throws.InstanceOf<ArgumentException>());
	}

	[Test]
	public void AssemblyTypeSerializer_FailsOnWrongAssembly_Deserialize() {
		var serializer = new AssemblyTypeSerializer(Assembly.GetAssembly(typeof(Hydrogen.Data.HydrogenJsonSerializer)));
		var serialized = serializer.SerializeBytesLE(typeof(Hydrogen.Data.HydrogenJsonSerializer));
		var serializer2 = new AssemblyTypeSerializer(Assembly.GetAssembly(typeof(Account)));
		Assert.That(() => serializer2.DeserializeBytesLE(serialized), Throws.InstanceOf<InvalidOperationException>());
	}

	[Test]
	public void TypeCollectionSerializer_Empty() {
		var types = Array.Empty<Type>();
		var serializer = new TypeCollectionSerializer();
		var serialized = serializer.SerializeBytesLE(types);
		var deserialized = serializer.DeserializeBytesLE(serialized);
		Assert.That(deserialized, Is.EqualTo(types));
	}

	[Test]
	public void TypeCollectionSerializer_One() {
		var types = new [] { typeof(Account) };
		var serializer = new TypeCollectionSerializer();
		var serialized = serializer.SerializeBytesLE(types);
		var deserialized = serializer.DeserializeBytesLE(serialized);
		Assert.That(deserialized, Is.EqualTo(types));
	}

	[Test]
	public void TypeCollectionSerializer_SingleSingleAssembly() {
		var types = new [] { typeof(Account), typeof(Identity), typeof(TypeSerializerTests) };
		var serializer = new TypeCollectionSerializer();
		var serialized = serializer.SerializeBytesLE(types);
		var deserialized = serializer.DeserializeBytesLE(serialized);
		Assert.That(deserialized, Is.EqualTo(types));
	}

	[Test]
	public void TypeCollectionSerializer_ManySingleAssembly() {
		var types = new [] { typeof(Account), typeof(IBijectiveDictionary<,>), typeof(Identity), typeof(Scope), typeof(TypeSerializerTests) };
		var serializer = new TypeCollectionSerializer();
		var serialized = serializer.SerializeBytesLE(types);
		var deserialized = serializer.DeserializeBytesLE(serialized);
		Assert.That(deserialized, Is.Not.EqualTo(types));
		Assert.That(deserialized, Is.EquivalentTo(types));
	}

	[Test]
	public void TypeCollectionSerializer_ManyCrossAssembly() {
		var types = new [] { typeof(Account), typeof(IBijectiveDictionary<Account,int>), typeof(Identity), typeof(Scope), typeof(TypeSerializerTests), typeof(IExtendedList<KeyValuePair<Scope, Account>>) };
		var serializer = new TypeCollectionSerializer();
		var serialized = serializer.SerializeBytesLE(types);
		var deserialized = serializer.DeserializeBytesLE(serialized);
		Assert.That(deserialized, Is.Not.EqualTo(types));
		Assert.That(deserialized, Is.EquivalentTo(types));
	}
}
