// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Linq;
using Hydrogen.ObjectSpaces;
using NUnit.Framework;


namespace Hydrogen.Tests;

[TestFixture]
public class ObjectSpacesBuilderTests {

	
	[Test]
	public void CannotRegisterSerializerWhenUsingCustomSerializerFactory_1() {
		var customSerializerFactory = new SerializerFactory(SerializerFactory.Default);

		var objectSpaceBuilder = new ObjectSpaceBuilder();
		objectSpaceBuilder
			.UsingSerializerFactory(customSerializerFactory)
			.AddDimension<Account>();

		var serializer = SerializerBuilder.For<Account>().Build();
		Assert.That(() => objectSpaceBuilder.Configure<Account>().UsingSerializer(serializer), Throws.InvalidOperationException);
	}

	[Test]
	public void CannotRegisterSerializerWhenUsingCustomSerializerFactory_2() {
		var customSerializerFactory = new SerializerFactory(SerializerFactory.Default);

		var objectSpaceBuilder = new ObjectSpaceBuilder();
		objectSpaceBuilder
			.UsingSerializerFactory(customSerializerFactory)
			.AddDimension<Account>();

		Assert.That(() => objectSpaceBuilder.Configure<Account>().UsingSerializer<DummyAccountSerializer>(), Throws.InvalidOperationException);
	}

	[Test]
	public void CannotRegisterCustomSerializerFactoryWhenRegisteredAnySerializer() {

		var objectSpaceBuilder = new ObjectSpaceBuilder();
		objectSpaceBuilder
			.UsingSerializer<Account, DummyAccountSerializer>();
			
		var customSerializerFactory = new SerializerFactory(SerializerFactory.Default);

		Assert.That(() => objectSpaceBuilder.UsingSerializerFactory(customSerializerFactory), Throws.InvalidOperationException);
	}


	[Test]
	public void CannotRegisterComparerWhenUsingCustomComparerFactory_1() {
		var customComparerFactory = new ComparerFactory(ComparerFactory.Default);

		var objectSpaceBuilder = new ObjectSpaceBuilder();
		objectSpaceBuilder
			.UseComparerFactory(customComparerFactory)
			.AddDimension<Account>();

		var comparer = ComparerBuilder.For<Account>();
		Assert.That(() => objectSpaceBuilder.Configure<Account>().UsingComparer(comparer), Throws.InvalidOperationException);
	}

	[Test]
	public void CannotRegisterComparerWhenUsingCustomComparerFactory_2() {
		var customComparerFactory = new ComparerFactory(ComparerFactory.Default);

		var objectSpaceBuilder = new ObjectSpaceBuilder();
		objectSpaceBuilder
			.UseComparerFactory(customComparerFactory)
			.AddDimension<Account>();

		Assert.That(() => objectSpaceBuilder.Configure<Account>().UsingComparer<DummyAccountComparer>(), Throws.InvalidOperationException);
	}

	[Test]
	public void CannotRegisterEqualityComparerWhenUsingCustomComparerFactory_1() {
		var customComparerFactory = new ComparerFactory(ComparerFactory.Default);

		var objectSpaceBuilder = new ObjectSpaceBuilder();
		objectSpaceBuilder
			.UseComparerFactory(customComparerFactory)
			.AddDimension<Account>();

		var comparer = EqualityComparerBuilder.For<Account>();
		Assert.That(() => objectSpaceBuilder.Configure<Account>().UsingEqualityComparer(comparer), Throws.InvalidOperationException);
	}

	[Test]
	public void CannotRegisterEqualityComparerWhenUsingCustomComparerFactory_2() {
		var customComparerFactory = new ComparerFactory(ComparerFactory.Default);

		var objectSpaceBuilder = new ObjectSpaceBuilder();
		objectSpaceBuilder
			.UseComparerFactory(customComparerFactory)
			.AddDimension<Account>();

		Assert.That(() => objectSpaceBuilder.Configure<Account>().UsingEqualityComparer<DummyAccountEqualityComparer>(), Throws.InvalidOperationException);
	}

	internal class DummyAccountSerializer : ItemSerializerBase<Account> {

		public override long CalculateSize(SerializationContext context, Account item) {
			throw new NotSupportedException();
		}

		public override void Serialize(Account item, EndianBinaryWriter writer, SerializationContext context) {
			throw new NotSupportedException();
		}

		public override Account Deserialize(EndianBinaryReader reader, SerializationContext context) {
			throw new NotSupportedException();
		}
	}

	internal class DummyAccountComparer : IComparer<Account> {
		public int Compare(Account x, Account y) {
			throw new NotSupportedException();
		}
	}

	internal class DummyAccountEqualityComparer : IEqualityComparer<Account> {
		public bool Equals(Account x, Account y) {
			throw new NotSupportedException();
		}

		public int GetHashCode(Account obj) {
			throw new NotSupportedException();
		}
	}
}
