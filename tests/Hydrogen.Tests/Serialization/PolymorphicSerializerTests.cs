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
using FluentAssertions;
using NUnit.Framework;

namespace Hydrogen.Tests;

[TestFixture]
[Parallelizable]
public class PolymorphicSerializerTests {
	
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
		
		public List<Animal> Animals { get; set; } = new();

		public object[] Objects { get; set; }

	}


	[Test]
	public void PolymorphicCase() {
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

		zoo.Animals.AddRange([ dog1, dog2, dog3, cat1, cat2 ]);
		
		zoo.Objects = ["alpha", null, 1L, 1M, dog1, zoo ];


		var serializer = ItemSerializer<Zoo>.Default;

		var size = serializer.CalculateSize(zoo);
		var bytes = serializer.SerializeBytesLE(zoo);
		var deserializedZoo = serializer.DeserializeBytesLE(bytes);

		Assert.That(bytes.Length, Is.EqualTo(size));

		deserializedZoo.Should().BeEquivalentTo(zoo, x=> x.IgnoringCyclicReferences());

	}

}
