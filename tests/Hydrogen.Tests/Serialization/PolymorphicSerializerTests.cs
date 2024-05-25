// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace Hydrogen.Tests;

[TestFixture]
[Parallelizable]
public class PolymorphicSerializerTests {
	
	[Test]
	public void AbstractSerializer() {
		var serializer = ItemSerializer<Animal>.Default;
		var dog = new Dog { Name = "Rex", Breed = "German Shepherd" };
		var size = serializer.CalculateSize(dog);
		var serialized = serializer.SerializeBytesLE(dog);
		var deserialized = serializer.DeserializeBytesLE(serialized);
		Assert.That(size, Is.EqualTo(serialized.Length));
		Assert.That(deserialized, Is.EqualTo(dog).UsingPropertiesComparer());
	}

	[Test]
	public void ListOfAbstractSerializer() {
		var serializer = ItemSerializer<List<Animal>>.Default;
		var dog = new Dog { Name = "Rex", Breed = "German Shepherd" };
		var cat = new Cat { Name = "Whiskers", Color = "White" };
		var animals = new List<Animal> { dog, null, cat };
		var size = serializer.CalculateSize(animals);
		var serialized = serializer.SerializeBytesLE(animals);
		var deserialized = serializer.DeserializeBytesLE(serialized);
		Assert.That(size, Is.EqualTo(serialized.Length));
		deserialized.Should().BeEquivalentTo(animals, x=> x.IgnoringCyclicReferences());
	}

	[Test]
	public void ListOfAbstractSerializer_CyclicReferences() {
		var serializer = ItemSerializer<List<Animal>>.Default;
		var dog = new Dog { Name = "Rex", Breed = "German Shepherd" };
		var cat = new Cat { Name = "Whiskers", Color = "White" };
		var animals = new List<Animal> { dog, null, cat };
		dog.Tag = animals;
		cat.Tag = dog;
		var size = serializer.CalculateSize(animals);
		var serialized = serializer.SerializeBytesLE(animals);
		var deserialized = serializer.DeserializeBytesLE(serialized);
		Assert.That(size, Is.EqualTo(serialized.Length));
		deserialized.Should().BeEquivalentTo(animals, x=> x.IgnoringCyclicReferences());
	}

	[Test]
	public void InterfaceTests_1() {
		var serializer = ItemSerializer<IList<int>>.Default;
		Assert.That(serializer, Is.InstanceOf<ReferenceSerializer<IList<int>>>());
		serializer =(IItemSerializer<IList<int>>) ((ReferenceSerializer<IList<int>>)serializer).InternalSerializer;
		Assert.That(serializer, Is.InstanceOf<PolymorphicSerializer<IList<int>>>());
		var list = new List<int> { 1, 2, 3, 4, 5 };
		var serialized = serializer.SerializeBytesLE(list);
		var deserialized = serializer.DeserializeBytesLE(serialized);
		Assert.That(list, Is.EqualTo(deserialized));
	}

	[Test]
	public void ComplexInterface() {
		var dataType = typeof(IList<KeyValuePair<IDictionary<IEnumerable<int>, ICollection<float>>, ISet<int>>>);
		var serializer = ItemSerializer<IList<KeyValuePair<IDictionary<IEnumerable<int>, ICollection<float>>, ISet<int>>>>.Default;
		Assert.That(serializer, Is.InstanceOf<ReferenceSerializer<IList<KeyValuePair<IDictionary<IEnumerable<int>, ICollection<float>>, ISet<int>>>>>());
		Assert.That(serializer.ItemType, Is.EqualTo(dataType));
	}

	[Test]
	public void ComplexPolymorphicCase() {
		var zoo =  new Zoo();

		var dog1 = new Dog { Name = "Rex", Breed = "German Shepherd" };
		dog1.Tag = null;

		var dog2 = new Dog { Name = "Fido", Breed = "Poodle" };
		dog2.Tag = dog1;

		var dog3 = new Dog { Name = "Spot", Breed = "Dalmatian" };
		dog3.Tag = dog3;


		var cat1 = new Cat { Name = "Whiskers", Color = "White" };
		cat1.Tag = zoo;

		var cat2 = new Cat { Name = "Mittens", Color = "Black" };
		cat2.Tag = null;

		zoo.Animals.AddRangeSequentially([ dog1, dog2, null, dog3, cat1, cat2 ]);
		
		zoo.Objects = ["alpha", null, 1L, 1M, dog1, zoo ];


		var serializer = ItemSerializer<Zoo>.Default;

		var size = serializer.CalculateSize(zoo);
		var bytes = serializer.SerializeBytesLE(zoo);
		var deserializedZoo = serializer.DeserializeBytesLE(bytes);

		Assert.That(bytes.Length, Is.EqualTo(size));

		deserializedZoo.Should().BeEquivalentTo(zoo, x=> x.IgnoringCyclicReferences());

	}


	[KnownSubType<Dog>]
	[KnownSubType<Cat>]
	public abstract class Animal {
		public string Name { get; set; }

		public object Tag { get; set; }
	}

	public sealed class Dog : Animal {
		public string Breed { get; set; }
	}

	public sealed class Cat : Animal {
		public string Color { get; set; }
	}

	public sealed class Zoo {
		
		public IList<Animal> Animals { get; set; } = new ExtendedList<Animal>();

		public object[] Objects { get; set; }

	}

}
