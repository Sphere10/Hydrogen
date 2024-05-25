// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

namespace Hydrogen.Tests;

[TestFixture]
[Parallelizable]
public class ReferenceSerializerTests {
	
	public class TestObject {
		public string Property1 { get; set; }
		public string Property2 { get; set; }
		public string Property3 { get; set; }
	}

	public class SealedMembersObject {
		public string MemberOfSealedType { get; set; }
		public SealedMembersObject MemberOfUnsealedType { get; set; }
	}

	[Test]
	public void NullableDoesNotReuseContextReference() {
		// test object
		var withContextReferenceSerializer = 
			SerializerBuilder
				.For<TestObject>()
					.Serialize(x => x.Property1, new StringSerializer().AsReferenceSerializer())
					.Serialize(x => x.Property2, new StringSerializer().AsReferenceSerializer())
					.Serialize(x => x.Property3, new StringSerializer().AsReferenceSerializer())
				.Build();

		var nullOnlySerializer = 
			SerializerBuilder
				.For<TestObject>()
				.Serialize(x => x.Property1, new StringSerializer().AsNullableSerializer())
				.Serialize(x => x.Property2, new StringSerializer().AsNullableSerializer())
				.Serialize(x => x.Property3, new StringSerializer().AsNullableSerializer())
				.Build();

		var comparer = EqualityComparerBuilder
			.For<TestObject>()
				.By(x => x.Property1)
				.ThenBy(x => x.Property2)
				.ThenBy(x => x.Property3);

		var obj = new TestObject {
			Property1 = "Hello",
			Property2 = "Hello",
			Property3 = null
		};
		
		var size1 = withContextReferenceSerializer.CalculateSize(obj);
		var serialized1 = withContextReferenceSerializer.SerializeBytesLE(obj);
		var deserialized1 = withContextReferenceSerializer.DeserializeBytesLE(serialized1);

		var size2 = nullOnlySerializer.CalculateSize(obj);
		var serialized2 = nullOnlySerializer.SerializeBytesLE(obj);
		var deserialized2 = nullOnlySerializer.DeserializeBytesLE(serialized2);

		Assert.That(serialized1.Length, Is.EqualTo(size1));
		Assert.That(serialized2.Length, Is.EqualTo(size2));

		Assert.That(obj, Is.EqualTo(deserialized1).Using(comparer));
		Assert.That(deserialized1.Property1, Is.SameAs(deserialized1.Property2));

		Assert.That(obj, Is.EqualTo(deserialized2).Using(comparer));
		Assert.That(deserialized2.Property1, Is.Not.SameAs(deserialized2.Property2));

		Assert.That(withContextReferenceSerializer.CalculateSize(obj), Is.LessThan(nullOnlySerializer.CalculateSize(obj)));
	}

	
	[Test]
	public void BugCase_1() {
		var serializer = 
			SerializerBuilder
				.For<TestObject>()
				.Serialize(x => x.Property1, new StringSerializer().AsNullableSerializer())
				.Serialize(x => x.Property2, new StringSerializer().AsNullableSerializer())
				.Serialize(x => x.Property3, new StringSerializer().AsNullableSerializer())
				.Build();

		var obj = new TestObject {
			Property1 = "Hello",
			Property2 = "Hello",
			Property3 = null
		};

		Assert.That(() => serializer.CalculateSize(obj), Throws.Nothing);
	}


	[Test]
	public void SealedMembersDoNotUsePolymorphicSerializers() {

		//var unsealedSerializer = SerializerBuilder.FactoryAssemble<UnsealedMembersObject>();
		var sealedSerializer = SerializerBuilder.FactoryAssemble<SealedMembersObject>();
		Assert.That(sealedSerializer, Is.InstanceOf<ReferenceSerializer<SealedMembersObject>>());

		var asReferenceSerializer = (ReferenceSerializer<SealedMembersObject>)sealedSerializer;
		Assert.That(asReferenceSerializer.Internal, Is.InstanceOf<PolymorphicSerializer<SealedMembersObject>>());

		var asPolymorphic = (PolymorphicSerializer<SealedMembersObject>)asReferenceSerializer.Internal;
		Assert.That(asPolymorphic.PureSerializer, Is.InstanceOf<CompositeSerializer<SealedMembersObject>>());


		var asCompositeSerializer = (CompositeSerializer<SealedMembersObject>)asPolymorphic.PureSerializer;
		Assert.That(asCompositeSerializer.MemberBindings.Length, Is.EqualTo(2));
		Assert.That(asCompositeSerializer.MemberBindings[0].Serializer, Is.InstanceOf<ReferenceSerializer<string>>());
		Assert.That(asCompositeSerializer.MemberBindings[1].Serializer, Is.InstanceOf<ReferenceSerializer<SealedMembersObject>>());

		// check sealed member does not have polymorphic serializer
		var sealedMemberSerializer = (ReferenceSerializer<string>)asCompositeSerializer.MemberBindings[0].Serializer;
		Assert.That(sealedMemberSerializer.Internal, Is.InstanceOf<StringSerializer>());
		
		// check unsealed member has polymorphic serializer
		var unsealedMemberSerializer = (ReferenceSerializer<SealedMembersObject>)asCompositeSerializer.MemberBindings[1].Serializer;
		Assert.That(unsealedMemberSerializer.Internal, Is.InstanceOf<PolymorphicSerializer<SealedMembersObject>>());
	}
	
}
