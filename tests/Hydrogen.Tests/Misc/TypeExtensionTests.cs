// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NUnit.Framework;

namespace Hydrogen.Tests;

[TestFixture]
public class TypeExtensionTests {

	[Test]
	public void IsCrossAssemblyType_False(
		[Values(typeof(Type), typeof(AssemblySerializer), typeof(Account), typeof(IBijectiveDictionary<,>), typeof(IBijectiveDictionary<Scope,Scope>), typeof(IDictionary<int,string>))]
		Type type
	) => Assert.That(type.IsCrossAssemblyType(), Is.False);

	[Test]
	public void IsCrossAssemblyType_True(
		[Values(typeof(IBijectiveDictionary<string,int>), typeof(IDictionary<Account,int>))]
	    Type type
	) => Assert.That(type.IsCrossAssemblyType(), Is.True);

	[Test]
	public void PartialGenericTypeDefinitionIsPartial() {
		var type = typeof(IItemSerializer<>).MakeGenericType(typeof(List<>));
		Assert.That(type.IsGenericTypeDefinition, Is.False);
		Assert.That(type.IsPartialTypeDefinition(), Is.True);
	}

	[Test]
	public void ConstructedGenericTypeDefinitionIsNotPartial() {
		var type = typeof(IItemSerializer<>).MakeGenericType(typeof(List<int>));
		Assert.That(type.IsGenericTypeDefinition, Is.False);
		Assert.That(type.IsPartialTypeDefinition(), Is.False);
	}

	[Test]
	public void GenericTypeDefinitionIsNotPartial() {
		var type = typeof(IItemSerializer<>);
		Assert.That(type.IsGenericTypeDefinition, Is.True);
		Assert.That(type.IsPartialTypeDefinition(), Is.False);
	}

	[Test] 
	public void ToStringCS() {
		Assert.That(typeof(KeyValuePair<,>).ToStringCS(), Is.EqualTo("KeyValuePair<TKey, TValue>"));
		Assert.That(typeof(KeyValuePair<int, float>).ToStringCS(), Is.EqualTo("KeyValuePair<Int32, Single>"));
		Assert.That(typeof(int).ToStringCS(), Is.EqualTo("Int32"));
		Assert.That(typeof(Dictionary<,>).ToStringCS(), Is.EqualTo("Dictionary<TKey, TValue>"));
        
		// Creating a partially constructed type
		Type partiallyConstructedType = typeof(KeyValuePair<,>).MakeGenericType(typeof(string), typeof(KeyValuePair<,>).GetGenericArguments()[1]);
		Assert.That(partiallyConstructedType.ToStringCS(), Is.EqualTo("KeyValuePair<String, TValue>"));

	}

}
