// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using FastSerialization;
using System.Drawing;
using NUnit.Framework;

namespace Hydrogen.Tests;

[TestFixture]
[Parallelizable]
public class SerializationAnnotationTests {

	[Test]
	public void TransientMembers_1() {
		// test object
		var serializer = SerializerBuilder.FactoryAssemble<TransientOnlyMembersClass>();
		
		var testObj = new TransientOnlyMembersClass { NonTransientExemption = "alpha", TransientInt = 11, TransientString = "beta" };
		testObj.Nested = testObj;
		
		var size = serializer.CalculateSize(testObj);
		var serialized = serializer.SerializeBytesLE(testObj);
		var deserialized = serializer.DeserializeBytesLE(serialized);

		Assert.That(serialized.Length, Is.EqualTo(size));
		Assert.That(deserialized.NonTransientExemption, Is.EqualTo("alpha"));
		Assert.That(deserialized.TransientField, Is.EqualTo(default(decimal)));
		Assert.That(deserialized.TransientInt, Is.EqualTo(default(int)));
		Assert.That(deserialized.TransientString, Is.EqualTo(default(string)));
		Assert.That(deserialized.Nested, Is.EqualTo(default(string)));
	}

	[Test]
	public void GetOnlyMembersAreNotSerializable() {
		var members = SerializerBuilder.GetSerializableMembers(typeof(GetOnlyMemberClass));
		Assert.That(members.Length, Is.EqualTo(1));
	}

	[Test]
	public void TransientMembersAreNotSerializable() {
		var members = SerializerBuilder.GetSerializableMembers(typeof(TransientOnlyMembersClass));
		Assert.That(members.Length, Is.EqualTo(1));
	}

	[Test]
	public void NullableMembersSerialize() {
		var serializer = SerializerBuilder.FactoryAssemble<NotNullableMembersClass>();
		var testObj = new NotNullableMembersClass { AllowsNull = null, NotAllowsNull = "alpha" };
		
		var size = serializer.CalculateSize(testObj);
		var serialized = serializer.SerializeBytesLE(testObj);
		var deserialized = serializer.DeserializeBytesLE(serialized);

		Assert.That(serialized.Length, Is.EqualTo(size));
		Assert.That(deserialized.AllowsNull, Is.Null);
		Assert.That(deserialized.NotAllowsNull, Is.EqualTo("alpha"));
	}

	[Test]
	public void NotNullableMembersThrowOnNullSerialization() {
		var serializer = SerializerBuilder.FactoryAssemble<NotNullableMembersClass>();
		var testObj = new NotNullableMembersClass { AllowsNull = null, NotAllowsNull = "alpha" };
		testObj = new NotNullableMembersClass { AllowsNull = null, NotAllowsNull = null };
		Assert.That(() => serializer.SerializeBytesLE(testObj), Throws.InvalidOperationException);
	}
	
	[Test]
	public void NoContextReferenceWhenDisallowed() {
		var serializer = SerializerBuilder.FactoryAssemble<NoContextReferences>();
		var testObj = new NoContextReferences { StringA = "alpha", StringB = "alpha" };
		
		Assert.That(testObj.StringA, Is.SameAs(testObj.StringB));
		
		var size = serializer.CalculateSize(testObj);
		var serialized = serializer.SerializeBytesLE(testObj);
		var deserialized = serializer.DeserializeBytesLE(serialized);

		Assert.That(serialized.Length, Is.EqualTo(size));
		Assert.That(deserialized.StringA, Is.Not.SameAs(testObj.StringB));
	}

	[Test]
	public void RecursiveNoContextReferenceThrowsOnInfiniteLoop_1() {
		var serializer = SerializerBuilder.FactoryAssemble<RecursiveNoContextReference>();
		var testObj = new RecursiveNoContextReference();
		testObj.Item = testObj;
		Assert.That(testObj.Item, Is.SameAs(testObj));
		Assert.That(() => serializer.CalculateSize(testObj), Throws.InvalidOperationException);
		Assert.That(() => serializer.SerializeBytesLE(testObj), Throws.InvalidOperationException);
	}

	[Test]
	public void RecursiveNoContextReferenceThrowsOnInfiniteLoop_2() {
		var serializer = SerializerBuilder.FactoryAssemble<RecursiveNoContextReference>();
		var testObj = new RecursiveNoContextReference();
		testObj.Item = new RecursiveNoContextReference();
		testObj.Item.Item = testObj;
		Assert.That(() => serializer.CalculateSize(testObj), Throws.InvalidOperationException);
		Assert.That(() => serializer.SerializeBytesLE(testObj), Throws.InvalidOperationException);
	}

	private class GetOnlyMemberClass {

		public string GetSetterExemption { get; set; }

		public int GetOnlyInt => 1;

		public string GetOnlyString => "Beta";

		public GetOnlyMemberClass GetOnlyClass => new GetOnlyMemberClass();
	}

	public class TransientOnlyMembersClass {

		public string NonTransientExemption { get; set; } = "Alpha";
		  
		[Transient]
		public decimal TransientField;

		[Transient]
		public int TransientInt { get; set; }

		[Transient]
		public string TransientString { get; set ; }

		[Transient]
		public TransientOnlyMembersClass Nested { get; set; }
	}

	public class NotNullableMembersClass {

		[ReferenceMode(Nullable = true)]
		public string AllowsNull { get; set; }

		[ReferenceMode(Nullable = false)]
		public string NotAllowsNull { get; set; }
	}

	public class NoContextReferences {

		[ReferenceMode(AllowContextReference = false)]
		public string StringA { get; set; }

		[ReferenceMode(AllowContextReference = false)]
		public string StringB { get; set; }
	}


	public class RecursiveNoContextReference {

		[ReferenceMode(AllowContextReference = false)]
		public RecursiveNoContextReference Item { get; set; }
	}
}
